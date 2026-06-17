"""
通用配置
"""
import os
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parent.parent.parent
DOWNLOAD_ROOT = PROJECT_ROOT / "tools" / "asset-crawlers" / "downloads"
MANIFEST_PATH = DOWNLOAD_ROOT / "manifest.json"

# 默认请求头
DEFAULT_HEADERS = {
    "User-Agent": "RolaAssetBot/1.0 (+https://github.com/Eiliuy/rola)"
}

# 默认请求间隔（秒）
DEFAULT_DELAY = 1.0

# Pixabay API Key
PIXABAY_API_KEY = os.getenv("PIXABAY_API_KEY", "")

# itch.io API Key（可选，用于下载已购买/拥有的资源）
ITCH_IO_API_KEY = os.getenv("ITCH_IO_API_KEY", "")

# Freesound API Key
FREESOUND_API_KEY = os.getenv("FREESOUND_API_KEY", "")

# 支持的下载站点
SUPPORTED_SITES = ["kenney", "pixabay", "itch"]
