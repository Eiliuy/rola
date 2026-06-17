"""
Kenney.nl 爬虫
下载所有免费 CC0 asset packs
"""
import re
import time
from pathlib import Path
from typing import List, Optional
from urllib.parse import urljoin

from bs4 import BeautifulSoup

from .base_crawler import BaseCrawler, AssetEntry
from .config import DEFAULT_DELAY


class KenneyCrawler(BaseCrawler):
    """Kenney.nl 免费素材包爬虫"""

    BASE_URL = "https://kenney.nl/assets"

    @property
    def site_name(self) -> str:
        return "kenney"

    def list_packs(self, max_pages: int = 50) -> List[dict]:
        """
        遍历 Kenney.nl/assets 分页，收集所有素材包信息
        """
        packs = []
        page = 1

        while page <= max_pages:
            url = f"{self.BASE_URL}/page:{page}"
            print(f"[FETCH] {url}")
            try:
                resp = self.session.get(url, timeout=30)
                resp.raise_for_status()
            except Exception as e:
                print(f"  [END] Failed to fetch page {page}: {e}")
                break

            soup = BeautifulSoup(resp.text, "html.parser")
            cards = soup.select("div.asset")
            found_on_page = 0
            for card in cards:
                # 找素材包链接（排除 category/tag/series）
                link = card.find("a", href=re.compile(r"^https://kenney\.nl/assets/[a-z0-9\-]+$"))
                if not link:
                    continue

                pack_url = link.get("href")
                if any(p["url"] == pack_url for p in packs):
                    continue

                title_tag = card.find("h2")
                title = title_tag.get_text(strip=True) if title_tag else ""
                if not title:
                    continue

                packs.append({"title": title, "url": pack_url})
                found_on_page += 1

            if found_on_page == 0:
                break

            print(f"  found {found_on_page} packs on page {page}, total {len(packs)}")
            page += 1
            time.sleep(self.delay)

        return packs

    def get_pack_download_url(self, pack_url: str) -> Optional[str]:
        """
        进入单个素材包页面，解析下载链接
        """
        resp = self.session.get(pack_url, timeout=30)
        resp.raise_for_status()
        soup = BeautifulSoup(resp.text, "html.parser")

        # 常见下载按钮 selector
        for selector in ["a.download", ".download a", "a[href$='.zip']", "a[href*='download']"]:
            tag = soup.select_one(selector)
            if tag:
                href = tag.get("href", "")
                if href:
                    return urljoin(pack_url, href)

        # 兜底：找所有 zip 链接
        for link in soup.find_all("a", href=re.compile(r"\.zip$")):
            return urljoin(pack_url, link["href"])

        return None

    def run(self, max_pages: int = 50):
        """
        主入口：列出所有 pack 并下载
        """
        packs = self.list_packs(max_pages)
        print(f"[INFO] Total packs to process: {len(packs)}")

        for pack in packs:
            title = pack["title"]
            pack_url = pack["url"]

            print(f"[PROCESS] {title} -> {pack_url}")
            download_url = self.get_pack_download_url(pack_url)
            if not download_url:
                print(f"  [WARN] No download link found for {title}")
                continue

            safe_name = re.sub(r"[^\w\-]+", "_", title).strip("_") + ".zip"
            file_path = self.download_file(download_url, filename=safe_name)
            if file_path:
                entry = AssetEntry(
                    source_url=pack_url,
                    local_path=str(file_path.relative_to(Path(__file__).resolve().parent.parent.parent.parent)),
                    title=title,
                    license_name="CC0",
                    source_site="kenney.nl",
                    tags=["kenney", "cc0"],
                )
                self.record_entry(entry)
                print(f"  [OK] Saved to {file_path}")


if __name__ == "__main__":
    crawler = KenneyCrawler()
    crawler.run()
