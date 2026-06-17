"""
统一入口
"""
import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

from asset_crawlers.config import SUPPORTED_SITES
from asset_crawlers.kenney_crawler import KenneyCrawler
from asset_crawlers.pixabay_crawler import PixabayCrawler
from asset_crawlers.itch_crawler import ItchCrawler


def main():
    parser = argparse.ArgumentParser(description="Rola Asset Crawlers")
    parser.add_argument("site", choices=SUPPORTED_SITES, help="Target site")
    parser.add_argument("--output", "-o", type=str, default=None, help="Output directory")
    parser.add_argument("--query", "-q", type=str, default="", help="Search query")
    parser.add_argument("--tag", "-t", type=str, default="cc0", help="itch.io tag filter")
    parser.add_argument("--type", type=str, default="image", help="Pixabay media type: image/video/music")
    parser.add_argument("--max-pages", type=int, default=5, help="Max pages to crawl")
    parser.add_argument("--delay", type=float, default=1.0, help="Request delay in seconds")

    args = parser.parse_args()

    output_dir = Path(args.output) if args.output else None

    if args.site == "kenney":
        crawler = KenneyCrawler(output_dir=output_dir, delay=args.delay)
        crawler.run(max_pages=args.max_pages)

    elif args.site == "pixabay":
        if not args.query:
            print("[ERROR] --query is required for Pixabay")
            sys.exit(1)
        crawler = PixabayCrawler(output_dir=output_dir, delay=args.delay)
        crawler.run(query=args.query, media_type=args.type, max_pages=args.max_pages)

    elif args.site == "itch":
        crawler = ItchCrawler(output_dir=output_dir, delay=args.delay)
        crawler.run(query=args.query, tag=args.tag, max_pages=args.max_pages)


if __name__ == "__main__":
    main()
