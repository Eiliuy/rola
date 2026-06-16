# Rola 当前已完成内容

## 项目基础

- Git 仓库初始化
- Unity 项目结构搭建
- `.gitignore` 配置
- 基础 README

## 核心战斗系统

### 玩家控制器（PlayerController）

- 移动（地面 + 空中控制）
- 跳跃
- 闪避/冲刺（带无敌帧）
- 受伤、击退、无敌帧
- 三段地面连招
- 空中连招
- 下劈攻击
- 输入缓冲机制

### 敌人 AI（EnemyController）

- 状态机：Idle / Patrol / Chase / Attack / Hurt / Dead
- 视野检测
- 攻击范围检测
- 巡逻行为
- 追击玩家
- 攻击前摇 / 命中判定 / 后摇
- 受击、击退、死亡

### 伤害系统

- `IDamageable` 接口
- 玩家实现 `IDamageable`
- 敌人实现 `IDamageable`
- 玩家血量/死亡/重生（PlayerStats）

## 打击感

- 镜头震动（CameraShake）
- 打击停顿（HitStopManager）
- 刀光特效（SlashEffect）
- 受击粒子（HitEffect）
- 死亡特效（DeathEffect）
- 音效管理器（AudioManager）
  - 玩家攻击/受伤/跳跃/闪避音效
  - 敌人攻击/受伤/死亡音效

## 关卡与场景

- 相机跟随（CameraFollow，带边界限制）
- 存档点/检查点（Checkpoint）
- 死亡区域（DeathZone）
- 场景切换（SceneTransition）
- 玩家出生点管理（PlayerSpawnManager）

## UI 与游戏流程

- 主菜单（MainMenu）
- 暂停菜单（PauseMenu）
- 设置面板（SettingsManager）
  - 主音量
  - 音效音量
  - 音乐音量占位
  - 全屏
  - PlayerPrefs 存档
- 血条（HealthBar）

## 旁白系统

- 旁白数据资产（NarrationData）
- 旁白管理器（NarrationManager）
  - 队列播放
  - 打字机效果
  - 配音支持
- 区域触发（NarrationTrigger）
- 事件触发（NarrationEventTrigger）
- 行为观察器（NarrationBehaviorWatcher）
  - 反复跳跃吐槽
  - 反复攻击吐槽
  - 挂机吐槽

## 全局管理器

- GameInitializer：自动创建所有单例管理器

## 肉鸽系统框架（新增）

- 数据资产
  - 职业数据（PlayerClassData）
  - 技能数据（SkillData）
  - 装备数据（EquipmentData）
  - 升级奖励数据（UpgradeData）
- 运行时属性系统
  - CharacterStats：基础属性 + 修改器计算最终属性
  - StatModifier：支持加值 / 百分比 / 最终乘区
  - PlayerBuild：汇总职业、技能、装备、升级
  - SkillCooldown：技能冷却管理
- 单局流程
  - RunManager / RunData：管理本局职业、Build、进度、金币、经验
  - UpgradeManager：升级奖励三选一
- 装备特效系统
  - IEquipmentEffect 接口
  - 命中特效（点燃/流血/眩晕）
  - 闪避特效（回血/短暂无敌）
  - 低血量特效（护盾/回血）
  - 击杀特效（回血/金币/临时攻速）
- 玩家系统接入
  - PlayerController 从 Build 读取移速、跳跃力、攻速、攻击范围、伤害
  - 攻击支持暴击和元素类型
  - 命中/击杀/闪避/受击事件转发给装备特效
  - PlayerStats 血量上限随 Build 变化动态同步

## 视觉系统模块化（新增）

- 角色外观模块化
  - CharacterPartSlot：定义 Hair / Body / Top / Bottom / Gloves / Shoes / Weapon / Accessory 等部位
  - CharacterPartData：单个部位 Sprite 与排序配置
  - CharacterAppearanceData：组合完整外观
  - CharacterAppearanceRenderer：按部位层级批量渲染 Sprite
  - PlayerVisualController：统一控制动画与外观切换
  - PlayerAnimationController：Animator 参数唯一封装
- 旧视觉逻辑清理
  - 移除 PlayerController 中直接操作 animator / spriteRenderer 的代码
  - 所有视觉表现统一走 PlayerVisualController
  - 玩家预制体结构改为根对象逻辑 + VisualRoot 渲染子物体
- 编辑器辅助
  - PlaceholderSpriteGenerator：一键生成按部位分色的占位 Sprite
  - SwordsmanClassGenerator：自动生成剑士职业相关的 ScriptableObject 资产

## AI 辅助工具链

- 已调研并选定 Funplay Unity MCP 作为 OpenCode 与 Unity Editor 的桥接方案
- 服务器端已下载 Unity 2022.3.50f1 Linux Editor，但因该主机无 GPU/显示输出，未继续安装激活
- 计划在个人 PC（带 GPU/显示器）上完成 Unity + Funplay MCP 的安装与配置，之后通过 OpenCode 远程驱动 Editor 操作
