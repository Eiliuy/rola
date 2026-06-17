# Rola 项目规范（Agent 指南）

## 项目概述

- **名称**：Rola
- **类型**：2D 横版动作游戏（ACT），参考《艾希》（ICEY）
- **引擎**：Unity
- **目标平台**：PC（Windows/macOS/Linux）、移动端（iOS/Android）
- **当前阶段**：核心战斗原型验证完成，进入内容与美术填充阶段
- **必读入口**：`docs/README.md`、`ROADMAP.md`

## 技术栈

- **引擎**：Unity 2022.3 LTS 或更新 LTS 版本
- **语言**：C#
- **版本控制**：Git
- **编辑器**：Visual Studio Code + Visual Studio Editor 包
- **UI**：Unity uGUI
- **2D 物理**：Rigidbody2D + Collider2D
- **动画**：Unity Animator
- **关卡**：Unity Tilemap / Tiled（可选）

## 项目结构

```
Assets/
├── Scripts/          # C# 脚本
│   ├── Core/         # 核心系统（待补充）
│   ├── Player/       # 玩家相关（待整理）
│   ├── Enemy/        # 敌人相关（待整理）
│   ├── UI/           # UI 脚本
│   └── Effects/      # 特效脚本
├── Art/              # 美术资源
│   ├── Sprites/      # 精灵图
│   ├── Tilesets/     # 瓦片集
│   ├── Effects/      # 特效贴图
│   └── UI/           # UI 素材
├── Audio/            # 音效与音乐
├── Prefabs/          # 预制体
├── Scenes/           # 场景文件
├── Resources/        # 动态加载资源
└── ScriptableObjects/# 数据资产（旁白、攻击数据等）

docs/                 # 项目文档
Packages/             # Unity 包配置
ProjectSettings/      # Unity 项目设置
```

## 编码规范

### 命名

| 类型 | 规范 | 示例 |
|---|---|---|
| 类名 | PascalCase | `PlayerController` |
| 方法名 | PascalCase | `TakeDamage()` |
| 字段/属性 | camelCase | `currentHP`, `IsInvincible` |
| 公共字段 | camelCase + 必要时 `[Header]` | `moveSpeed` |
| 常量 | ALL_CAPS | `MAX_HP` |
| 枚举 | PascalCase | `PlayerState` |
| 接口 | 以 I 开头 | `IDamageable` |

### 代码风格

- 使用 4 空格缩进
- 类、方法、复杂逻辑块必须加中文或英文 XML 注释
- 公共 API 必须注释说明用途和参数
- 避免魔法数字，使用 `SerializeField` 暴露到 Inspector
- 优先使用 `SerializeField` 而非 public 字段暴露内部状态
- 单例模式统一使用 `Awake()` + `DontDestroyOnLoad`
- 物理更新放在 `FixedUpdate()`，输入读取放在 `Update()`

### 状态机

- 敌人 AI 使用显式枚举状态机
- 玩家状态尽量通过 Animator 参数驱动
- 避免大量 `bool` 标志，优先使用枚举或状态类

### 伤害与反馈

- 所有可受伤对象必须实现 `IDamageable`
- 打击感反馈（震动、停顿、粒子、音效）集中在命中时触发
- 不要直接在 `PlayerController` 里写敌人逻辑，也不要在 `EnemyController` 里写玩家逻辑

## Git 提交规范

使用 Angular 风格的提交信息：

```
<type>(<scope>): <subject>
```

常用类型：

| 类型 | 含义 |
|---|---|
| `feat` | 新功能 |
| `fix` | 修复 Bug |
| `docs` | 文档更新 |
| `chore` | 杂项/配置 |
| `refactor` | 重构 |
| `style` | 格式调整（不影响功能） |
| `test` | 测试相关 |

示例：

```
feat: 添加敌人远程攻击状态
fix: 修复玩家闪避后无法移动的问题
docs: 更新关卡设计文档
chore: 添加 Visual Studio Editor 包
```

## 开发工作流

1. 每次开发前确认当前 `main` 分支是最新的
2. 小步快跑，每个功能一个提交
3. 不要提交 `Library/`、`Temp/`、`Obj/`、`Builds/` 等自动生成的文件（已配置 `.gitignore`）
4. 提交前运行 Unity，确保没有编译错误
5. 重大修改前先更新 `docs/current-state.md` 和 `ROADMAP.md`

## 美术与资源规范

- 美术资源格式优先使用 PNG（带透明通道）
- Sprite 导入后必须设置 `Texture Type` 为 `Sprite (2D and UI)`
- 动画帧图使用 Sprite Sheet 或独立 Sprite
- 音频资源优先使用 WAV（短音效）或 OGG（长音乐）
- 所有资源按类型放入 `Assets/Art/`、`Assets/Audio/` 等目录
- 占位资源使用简单几何图形，命名加 `_Placeholder`

## 场景与 Prefab 规范

- 场景文件命名清晰：`MainMenu`、`Level_01`、`Level_02`
- 玩家、敌人、UI 等重要对象必须做成 Prefab
- Prefab 变体用于同类型不同配置的敌人
- 不要直接在场景中硬编码过多参数，优先通过 ScriptableObject 或 Prefab 配置

## 性能与最佳实践

- 避免在 `Update()` 中使用 `FindObjectOfType` 或 `GetComponent`
- 频繁创建的特效/敌人使用对象池（后续实现）
- 粒子效果播放后自动销毁
- 2D 碰撞器尽量使用 `CompositeCollider2D` 合并静态地形
- 移动平台/对象按需设置 `Rigidbody2D` 为 Kinematic
- 谨慎使用 `DontDestroyOnLoad`，避免单例重复

## 测试与构建

- 开发中经常 Play 测试手感
- 打包测试至少包含一次 PC 构建
- 移动端测试前检查 UI 缩放和输入适配
- 提交前确认没有未保存的场景或 Prefab 覆盖

## 文档维护

- 新增系统或重要修改时，更新 `docs/current-state.md`
- 长期计划与阶段优先级更新 `ROADMAP.md`
- 遇到的问题或技术债务记录到 `docs/gaps.md`
- 当前正在执行的短期计划写到 `docs/swordsman-class-plan.md` 或类似 active plan 文档
- 已完成的计划文档及时删除，避免与现状混淆

## 沟通原则

- 保持简单，不要过度工程化
- 优先验证核心玩法，再 polishing
- 美术占位即可，不要过早追求完美
