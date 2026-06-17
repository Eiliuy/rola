# Asset Crawlers for Rola

本项目用于批量下载独立游戏开发可用的免费/CC0 素材。

## 目录结构

```
tools/asset-crawlers/
├── asset-crawlers/      # 爬虫模块包
│   ├── __init__.py
│   ├── base_crawler.py  # 基础爬虫基类
│   ├── config.py        # 通用配置
│   ├── kenney_crawler.py
│   ├── pixabay_crawler.py
│   └── itch_crawler.py
├── downloads/           # 默认下载目录
│   └── manifest.json    # 下载记录
├── requirements.txt
├── README.md
└── run.py               # 统一入口
```

## 安装依赖

```bash
cd tools/asset-crawlers
pip install -r requirements.txt
```

## 使用方法

### Kenney.nl

```bash
python run.py kenney --output ./downloads/kenney
```

### Pixabay

需要先在 [Pixabay](https://pixabay.com/api/docs/) 申请 API Key，配置到环境变量：

```bash
export PIXABAY_API_KEY=your_key_here
python run.py pixabay --query "sword warrior sprite" --output ./downloads/pixabay
```

### itch.io

```bash
python run.py itch --query "cc0 platformer" --output ./downloads/itch
```

## 法律与伦理

- 仅下载 CC0 或明确允许免费商用的素材
- 遵守目标网站的 `robots.txt` 和 Terms of Service
- 控制请求频率，默认 1 秒一次
- 详细许可证信息见 `docs/asset-sources.md`
