# Rola 当前已完成内容

> 本文档记录项目已落地系统。**新增系统或重要修改后请同步更新。**
> 长期待解决问题见 [gaps.md](gaps.md)。
> 当前 active 计划见 [swordsman-class-plan.md](swordsman-class-plan.md)。

---

## 项目基础

- Git 仓库初始化
- Unity 项目结构搭建（`Assets/Scripts`、`Assets/Prefabs`、`Assets/Scenes`、`Assets/Data` 等）
- `.gitignore`、`.gitattributes` 配置
- 基础 README 与项目规范（`AGENTS.md`、`CLAUDE.md`、`SCRIPTS_REFERENCE.md`、`ASSETS_GUIDE.md`）

## 核心战斗系统

### 玩家控制器（PlayerController）

- 地面/空中移动与转向
- 跳跃
- 闪避/冲刺，带无敌帧
- 受伤、击退、无敌帧
- 三段地面连招 + 空中连招
- 下劈攻击
- 输入缓冲机制
- 数值来源已接入 `PlayerBuild`：移速、跳跃力、攻击伤害、攻击范围、攻击速度、暴击
- 命中/受击/闪避/击杀事件转发给装备特效

### 敌人 AI（EnemyController）

- 状态机：Idle / Patrol / Chase / Attack / Hurt / Dead
- 视野检测、攻击范围检测
- 巡逻、追击、攻击前摇/命中/后摇
- 受击、击退、死亡

### 伤害系统

- `IDamageable` 接口
- 玩家与敌人都实现 `IDamageable`
- 玩家血量/死亡/重生（PlayerStats），血量上限随 Build 动态同步

## 打击感

- 镜头震动（CameraShake）
- 打击停顿（HitStopManager）
- 刀光特效（SlashEffect）
- 受击粒子（HitEffect）
- 死亡特效（DeathEffect）
- 音效管理器（AudioManager）骨架
  - 玩家攻击/受伤/跳跃/闪避音效接口
  - 敌人攻击/受伤/死亡音效接口
  - 目前**没有实际 AudioClip 资源**

## 关卡与场景

- 相机跟随（CameraFollow，带边界限制）
- 存档点/检查点（Checkpoint）
- 死亡区域（DeathZone）
- 场景切换（SceneTransition）
- 玩家出生点管理（PlayerSpawnManager）
- 可玩测试场景：`Assets/Scenes/TestArena.unity`

## UI 与游戏流程（代码层面完成，场景中未配置）

- 主菜单逻辑（MainMenu）
- 暂停菜单逻辑（PauseMenu）
- 设置面板（SettingsManager）：主音量、音效音量、音乐音量、全屏、PlayerPrefs 存档
- 血条逻辑（HealthBar）
- 升级奖励三选一逻辑（UpgradeManager）

> 注：UI 脚本存在，但 `TestArena` 场景中**没有 Canvas / HealthBar / PauseMenu GameObjects**。

## 旁白系统

- 旁白数据资产（NarrationData）
- 旁白管理器（NarrationManager）：队列播放、打字机效果、配音支持
- 区域触发（NarrationTrigger）
- 事件触发（NarrationEventTrigger）
- 行为观察器（NarrationBehaviorWatcher）：反复跳跃/攻击吐槽、挂机吐槽

## 全局管理器

- GameInitializer：自动创建所有单例管理器（空 GameObject 兜底）
- RunManager：单例，管理本局 Run 数据
- UpgradeManager：单例，管理升级奖励
- AudioManager、SettingsManager、NarrationManager 等

## 肉鸽系统框架

### 数据资产

- 职业数据（PlayerClassData）
- 技能数据（SkillData）
- 装备数据（EquipmentData）
- 升级奖励数据（UpgradeData）
- 外观数据（CharacterAppearanceData）
- 部位数据（CharacterPartData）

### 运行时属性系统

- CharacterStats：基础属性 + 修改器 = 最终属性
- StatModifier：FlatAdd / PercentAdd / FinalMultiplier
- PlayerBuild：汇总职业、技能、装备、升级
- SkillCooldown：技能冷却管理

### 单局流程

- RunManager / RunData：管理本局职业、Build、进度、金币、经验
- UpgradeManager：升级奖励三选一

### 装备特效系统

- IEquipmentEffect 接口
- 命中特效：点燃 / 流血 / 眩晕
- 闪避特效：回血 / 短暂无敌
- 低血量特效：护盾 / 回血
- 击杀特效：回血 / 金币 / 临时攻速

## 视觉系统模块化

- 角色外观模块化
  - CharacterPartSlot：部位枚举
  - CharacterPartData：单个部位资产
  - CharacterAppearanceData：完整外观资产
  - CharacterAppearanceRenderer：按部位层级批量渲染 Sprite
  - PlayerVisualController：统一控制动画与外观切换
  - PlayerAnimationController：Animator 参数唯一封装
- 旧视觉逻辑清理
  - PlayerController 不再直接操作 animator / spriteRenderer
  - 所有视觉表现统一走 PlayerVisualController
  - 玩家预制体结构：根对象逻辑 + VisualRoot 渲染子物体
- 编辑器辅助
  - PlaceholderSpriteGenerator：一键生成按部位分色的占位 Sprite
  - SwordsmanClassGenerator：自动生成剑士职业相关 ScriptableObject 资产（开发中）

## AI 辅助工具链

- 已调研并选定 Funplay Unity MCP 作为 OpenCode 与 Unity Editor 的桥接方案
- 当前服务器**无法运行 Unity Editor**（无 GPU/显示输出），MCP 不可用
- 计划在个人 PC（带 GPU/显示器）上完成 Unity + Funplay MCP 的安装与配置

## 当前数据资产状态

| 资产 | 状态 | 说明 |
|---|---|---|
| `Assets/Data/Class_Warrior.asset` | 仅基础属性 | 无初始技能、装备、外观 |
| `Assets/ScriptableObjects/Classes/` | 空 | 等待填充 |
| `Assets/ScriptableObjects/Skills/` | 空 | 等待填充 |
| `Assets/ScriptableObjects/Equipments/` | 空 | 等待填充 |
| `Assets/ScriptableObjects/Appearances/` | 空 | 等待填充 |
| `Assets/ScriptableObjects/Upgrades/` | 空 | 等待填充 |

---

## 下一步（当前 active）

见 [swordsman-class-plan.md](swordsman-class-plan.md)：创建剑士职业的 ScriptableObject 资产和占位美术资源。
