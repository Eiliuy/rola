"""
基础爬虫基类
提供通用下载、manifest 维护、请求频率控制
"""
import json
import time
import hashlib
from abc import ABC, abstractmethod
from datetime import datetime, timezone
from pathlib import Path
from typing import Dict, List, Optional
from urllib.parse import urlparse

import requests
from tqdm import tqdm

from .config import DEFAULT_HEADERS, DEFAULT_DELAY, MANIFEST_PATH, DOWNLOAD_ROOT


class AssetEntry:
    """单条下载记录"""
    def __init__(self, source_url: str, local_path: str, title: str = "",
                 license_name: str = "", source_site: str = "", tags: List[str] = None):
        self.source_url = source_url
        self.local_path = local_path
        self.title = title
        self.license_name = license_name
        self.source_site = source_site
        self.tags = tags or []
        self.downloaded_at = datetime.now(timezone.utc).isoformat()
        self.checksum = ""

    def to_dict(self) -> Dict:
        return {
            "source_url": self.source_url,
            "local_path": self.local_path,
            "title": self.title,
            "license": self.license_name,
            "source_site": self.source_site,
            "tags": self.tags,
            "downloaded_at": self.downloaded_at,
            "checksum": self.checksum,
        }


class BaseCrawler(ABC):
    """素材爬虫基类"""

    def __init__(self, output_dir: Optional[Path] = None, delay: float = DEFAULT_DELAY):
        self.output_dir = Path(output_dir) if output_dir else DOWNLOAD_ROOT / self.site_name
        self.output_dir.mkdir(parents=True, exist_ok=True)
        self.delay = delay
        self.session = requests.Session()
        self.session.headers.update(DEFAULT_HEADERS)
        self._manifest: List[Dict] = []
        self._load_manifest()

    @property
    @abstractmethod
    def site_name(self) -> str:
        pass

    def _load_manifest(self):
        """加载已有 manifest"""
        if MANIFEST_PATH.exists():
            try:
                data = json.loads(MANIFEST_PATH.read_text(encoding="utf-8"))
                self._manifest = data.get("entries", [])
            except Exception:
                self._manifest = []

    def _save_manifest(self):
        """保存 manifest"""
        MANIFEST_PATH.parent.mkdir(parents=True, exist_ok=True)
        data = {
            "updated_at": datetime.now(timezone.utc).isoformat(),
            "entries": self._manifest,
        }
        MANIFEST_PATH.write_text(json.dumps(data, ensure_ascii=False, indent=2), encoding="utf-8")

    def _already_downloaded(self, url: str) -> bool:
        """检查 URL 是否已下载"""
        return any(entry.get("source_url") == url for entry in self._manifest)

    def _compute_checksum(self, file_path: Path) -> str:
        """计算文件 MD5"""
        h = hashlib.md5()
        with open(file_path, "rb") as f:
            for chunk in iter(lambda: f.read(8192), b""):
                h.update(chunk)
        return h.hexdigest()

    def download_file(self, url: str, filename: Optional[str] = None,
                      subfolder: str = "") -> Optional[Path]:
        """
        下载单个文件到输出目录
        """
        if self._already_downloaded(url):
            print(f"[SKIP] Already downloaded: {url}")
            return None

        save_dir = self.output_dir / subfolder
        save_dir.mkdir(parents=True, exist_ok=True)

        if not filename:
            filename = Path(urlparse(url).path).name or "unnamed"
        file_path = save_dir / filename

        try:
            resp = self.session.get(url, stream=True, timeout=60)
            resp.raise_for_status()

            total = int(resp.headers.get("content-length", 0))
            with open(file_path, "wb") as f, tqdm(
                desc=filename,
                total=total,
                unit="B",
                unit_scale=True,
                unit_divisor=1024,
            ) as pbar:
                for chunk in resp.iter_content(chunk_size=8192):
                    if chunk:
                        f.write(chunk)
                        pbar.update(len(chunk))

            return file_path
        except Exception as e:
            print(f"[ERROR] Failed to download {url}: {e}")
            if file_path.exists():
                file_path.unlink()
            return None
        finally:
            time.sleep(self.delay)

    def record_entry(self, entry: AssetEntry):
        """记录下载条目"""
        local_path = Path(entry.local_path)
        if local_path.exists():
            entry.checksum = self._compute_checksum(local_path)

        # 去重：同一 URL 只保留最新一条
        self._manifest = [e for e in self._manifest if e.get("source_url") != entry.source_url]
        self._manifest.append(entry.to_dict())
        self._save_manifest()

    @abstractmethod
    def run(self, *args, **kwargs):
        """子类实现具体的爬取逻辑"""
        pass
