# Rola 项目素材来源清单

> 本文件记录 Rola 项目使用或参考的所有外部素材来源、许可证和本地路径。  
> 新增素材后请同步更新。  
> 自动化下载记录见：`tools/asset-crawlers/downloads/manifest.json`

---

## 来源总览

| 来源网站 | 用途 | 许可证 | 批量下载工具 |
|---|---|---|---|
| [Kenney.nl](https://kenney.nl/) | 2D/3D/UI/音效占位素材 | CC0 | `tools/asset-crawlers/run.py kenney` |
| [Pixabay](https://pixabay.com/) | BGM、音效、图片 | Pixabay License / CC0 (pre-2019) | `tools/asset-crawlers/run.py pixabay` |
| [itch.io / game-assets](https://itch.io/game-assets) | 2D 动作/像素素材 | 各异，优先 CC0 | `tools/asset-crawlers/run.py itch` |
| [OpenGameArt](https://opengameart.org/) | 2D/3D/音频 | CC0/CC-BY/OGA-BY | `oga` CLI |
| [Freesound](https://freesound.org/) | 游戏音效 | CC0/CC-BY/CC-BY-NC | Freesound API |
| [Game-Icons.net](https://game-icons.net/) | UI 图标 | CC-BY | GitHub 镜像 |
| [Poly Haven](https://polyhaven.com/) | 3D/HDRI/纹理 | CC0 | `polydown` |
| [Sketchfab](https://sketchfab.com/) | 3D 模型 | CC0/CC-BY | Sketchfab Download API |

---

## Rola 当前阶段推荐来源

### 美术素材

| 类别 | 推荐来源 | 搜索关键词 |
|---|---|---|
| 玩家角色 Sprite Sheet | itch.io (CC0) + Kenney | `sci fi warrior sprite sheet`, `anime swordsman sprites` |
| 敌人 Sprite Sheet | itch.io (CC0) + Kenney | `2d enemy sprite pack`, `monster sprite sheet` |
| 场景 Tileset / 平台 | Kenney + itch.io | `platformer tileset sci fi`, `industrial platform tiles` |
| 刀光 / 特效 | itch.io + Pixabay | `slash effect sprite`, `sword slash vfx` |
| UI 素材 | Kenney UI packs + Game-Icons | `pixel ui pack`, `game ui buttons` |

### 音频素材

| 类别 | 推荐来源 | 搜索关键词 |
|---|---|---|
| BGM | Pixabay / Freesound | `action cyberpunk music`, `epic battle bgm` |
| 攻击音效 | Freesound / Kenney | `sword swing`, `slash sound` |
| 受击/死亡音效 | Freesound / Kenney | `hurt sound`, `enemy death` |
| UI 音效 | Kenney UI Audio | `ui click`, `menu confirm` |

---

## 许可证说明

- **CC0**：可自由商用、修改、无需署名。
- **CC-BY**：可商用，但必须注明作者。
- **CC-BY-NC**：仅非商业用途。
- **Pixabay License**：可免费商用，无需署名；2019 年 1 月 9 日前上传的内容多为 CC0。
- **OGA-BY**：OpenGameArt 版 CC-BY，需署名。

---

## 状态备注

- **Kenney.nl**：✅ 已批量下载 207 个 CC0 包，清单见 `tools/asset-crawlers/downloads/manifest.json`。
- **Pixabay**：✅ 已用 API Key 下载约 80 张参考图片（关键词：sword slash / cyberpunk character / sci fi background / explosion effect / ui button / game icon）。Pixabay 标准 API 无独立 music 端点，音频需额外处理。
- **itch.io**：⏸️ 已实现列表抓取，但免费资源下载按钮多为 JS 动态加载，需要进一步用 API/headless 处理；目前仅修复 URL 和异常链接过滤。

---

## 已下载素材

### Kenney.nl（已批量下载）

- **下载时间**：2026-06-17
- **总包数**：207
- **总大小**：约 433 MB
- **许可证**：CC0
- **本地路径**：`tools/asset-crawlers/downloads/kenney/`
- **清单文件**：`tools/asset-crawlers/downloads/manifest.json`

按标题关键词分类：

| 类别 | 数量 |
|---|---|
| Platformer | 27 |
| Tile / Environment | 24 |
| Vehicle / Combat | 19 |
| UI / Icon / Font | 19 |
| Roguelike / RPG | 16 |
| Character | 16 |
| Audio | 10 |
| VFX | 3 |

Rola 战斗原型可优先使用的包：

| 包名 | 用途 |
|---|---|
| Platformer Kit | 横版平台场景元件 |
| Platformer Pack Remastered | 高清平台/敌人/收集物 |
| Platformer Characters | 玩家/敌人角色 Sprite |
| Modular Characters | 可拼装角色部件 |
| Roguelike Characters | 像素风角色 |
| Monster Builder Pack | 敌人组合部件 |
| Particle Pack | 通用粒子特效 |
| Smoke Particles | 烟雾/受击特效 |
| UI Pack / UI Pack (RPG Expansion) / Pixel UI Pack | 游戏 UI |
| UI Audio / Digital Audio / RPG Audio | 音效与 BGM |

（完整 207 条记录见 manifest.json）

### Pixabay（已用 API Key 下载图片）

- **下载时间**：2026-06-17
- **API Key**：已配置（环境变量 `PIXABAY_API_KEY`）
- **图片数量**：约 80 张
- **总大小**：约 37 MB
- **许可证**：Pixabay License / CC0（pre-2019）
- **本地路径**：`tools/asset-crawlers/downloads/pixabay/image/`
- **清单文件**：`tools/asset-crawlers/downloads/manifest.json`

搜索关键词：

| 关键词 | 用途 |
|---|---|
| `sword slash` / `sword` | 刀剑、挥砍参考 |
| `cyberpunk character` | 赛博朋克角色参考 |
| `sci fi background` | 科幻场景/背景 |
| `explosion effect` | 爆炸/特效参考 |
| `ui button` / `game icon` | UI 元素参考 |

- **注意**：Pixabay 标准 API 不区分 music 类型，`--type music` 实际返回图片结果；如需音频需用 Pixabay Music 独立端点或搜索 `music` 关键词的图片/视频封面。
