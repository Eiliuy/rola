"""
Pixabay API 下载器
需要 PIXABAY_API_KEY 环境变量
"""
import os
import time
from pathlib import Path
from typing import List
from urllib.parse import urlparse

from .base_crawler import BaseCrawler, AssetEntry


class PixabayCrawler(BaseCrawler):
    """Pixabay 图片/音乐/音效下载器"""

    API_URL = "https://pixabay.com/api/"
    API_URL_VIDEO = "https://pixabay.com/api/videos/"

    @property
    def site_name(self) -> str:
        return "pixabay"

    def __init__(self, output_dir: Path = None, delay: float = 1.0):
        super().__init__(output_dir, delay)
        self.api_key = os.getenv("PIXABAY_API_KEY")
        if not self.api_key:
            raise ValueError("PIXABAY_API_KEY environment variable is required")

    def search(self, query: str, media_type: str = "image", per_page: int = 20, max_pages: int = 5) -> List[dict]:
        """
        搜索 Pixabay 资源
        media_type: image / video / music
        """
        results = []
        base_url = self.API_URL_VIDEO if media_type == "video" else self.API_URL

        for page in range(1, max_pages + 1):
            params = {
                "key": self.api_key,
                "q": query,
                "per_page": per_page,
                "page": page,
                "safesearch": "true",
            }
            print(f"[FETCH] Pixabay {media_type} page {page}: {query}")
            resp = self.session.get(base_url, params=params, timeout=30)
            resp.raise_for_status()
            data = resp.json()

            hits = data.get("hits", [])
            if not hits:
                break

            for hit in hits:
                if media_type == "video":
                    url = hit.get("videos", {}).get("medium", {}).get("url", "")
                elif media_type == "music":
                    # Pixabay music API 与 image 共享端点，但返回的是音频
                    url = hit.get("previewURL", "") or hit.get("largeImageURL", "")
                else:
                    url = hit.get("largeImageURL", "")

                if url:
                    results.append({
                        "id": hit.get("id"),
                        "title": hit.get("tags", query),
                        "url": url,
                        "page_url": hit.get("pageURL", ""),
                        "type": media_type,
                    })

            print(f"  found {len(hits)} items, total {len(results)}")
            time.sleep(self.delay)

        return results

    def run(self, query: str, media_type: str = "image", max_pages: int = 3):
        """主入口"""
        items = self.search(query, media_type, max_pages=max_pages)
        print(f"[INFO] Total items to download: {len(items)}")

        for item in items:
            url = item["url"]
            ext = Path(urlparse(url).path).suffix or ".jpg"
            filename = f"pixabay_{item['type']}_{item['id']}{ext}"

            file_path = self.download_file(url, filename=filename, subfolder=media_type)
            if file_path:
                entry = AssetEntry(
                    source_url=item["page_url"] or url,
                    local_path=str(file_path.relative_to(Path(__file__).resolve().parent.parent.parent.parent)),
                    title=item["title"],
                    license_name="Pixabay License / CC0 (pre-2019)",
                    source_site="pixabay.com",
                    tags=["pixabay", media_type],
                )
                self.record_entry(entry)
                print(f"  [OK] {filename}")


if __name__ == "__main__":
    import sys
    query = sys.argv[1] if len(sys.argv) > 1 else "game background"
    crawler = PixabayCrawler()
    crawler.run(query)
