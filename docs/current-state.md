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
