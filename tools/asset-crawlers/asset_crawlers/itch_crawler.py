"""
itch.io 素材包爬虫
搜索免费 / CC0 素材并下载
"""
import re
import time
from pathlib import Path
from typing import List
from urllib.parse import urljoin, urlparse

from bs4 import BeautifulSoup

from .base_crawler import BaseCrawler, AssetEntry


class ItchCrawler(BaseCrawler):
    """itch.io 免费游戏素材爬虫"""

    BASE_URL = "https://itch.io/game-assets"

    @property
    def site_name(self) -> str:
        return "itch"

    def search(self, query: str = "", tag: str = "cc0", max_pages: int = 5) -> List[dict]:
        """
        搜索 itch.io 免费素材
        """
        results = []
        page = 1

        while page <= max_pages:
            url = f"{self.BASE_URL}/tag-{tag}" if tag else self.BASE_URL
            params = {"page": page}
            if query:
                params["q"] = query
            # tag 已在 URL 路径中，无需再放入 params
            print(f"[FETCH] {url} page {page}")
            resp = self.session.get(url, params=params, timeout=30)
            resp.raise_for_status()

            soup = BeautifulSoup(resp.text, "html.parser")
            games = soup.select(".game_cell")
            if not games:
                break

            for game in games:
                link = game.select_one("a.game_link")
                if not link:
                    continue
                href = link.get("href", "")
                title_tag = game.select_one(".game_title")
                title = title_tag.get_text(strip=True) if title_tag else "untitled"
                results.append({"title": title, "url": urljoin(self.BASE_URL, href)})

            print(f"  found {len(games)} items, total {len(results)}")
            page += 1
            time.sleep(self.delay)

        return results

    def get_download_url(self, asset_url: str) -> str:
        """
        进入素材页面，找到下载按钮链接
        """
        resp = self.session.get(asset_url, timeout=30)
        resp.raise_for_status()
        soup = BeautifulSoup(resp.text, "html.parser")

        # 免费素材通常有 .download_btn 或类似按钮
        for selector in [".download_btn", "a[href*='/download']"]:
            tag = soup.select_one(selector)
            if tag:
                href = tag.get("href", "")
                if href and not href.startswith("javascript:"):
                    return urljoin(asset_url, href)

        return ""

    def run(self, query: str = "", tag: str = "cc0", max_pages: int = 3):
        """主入口"""
        items = self.search(query, tag, max_pages)
        print(f"[INFO] Total items to process: {len(items)}")

        for item in items:
            print(f"[PROCESS] {item['title']}")
            download_url = self.get_download_url(item["url"])
            if not download_url:
                print(f"  [WARN] No download link")
                continue

            ext = Path(urlparse(download_url).path).suffix or ".zip"
            safe_name = re.sub(r"[^\w\-]+", "_", item["title"]).strip("_") + ext
            file_path = self.download_file(download_url, filename=safe_name)
            if file_path:
                entry = AssetEntry(
                    source_url=item["url"],
                    local_path=str(file_path.relative_to(Path(__file__).resolve().parent.parent.parent.parent)),
                    title=item["title"],
                    license_name="CC0 (verify on page)",
                    source_site="itch.io",
                    tags=["itch", "cc0"],
                )
                self.record_entry(entry)
                print(f"  [OK] {safe_name}")


if __name__ == "__main__":
    crawler = ItchCrawler()
    crawler.run()
