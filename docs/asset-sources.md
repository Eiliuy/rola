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

## 注意事项

1. 优先使用 **CC0** 素材，避免署名/授权纠纷。
2. 使用 `tools/asset-crawlers/` 下载时，会自动记录来源和许可证到 `manifest.json`。
3. 手动下载的素材请在本文件追加一行记录。
4. 不要把原始素材文件直接提交到 Git；素材包应作为本地资源或放到统一存储。

---

## 已下载素材

| 文件名 | 来源 URL | 许可证 | 用途 | 下载时间 |
|---|---|---|---|---|
| （空） |  |  |  |  |
