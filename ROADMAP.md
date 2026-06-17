# Rola 项目路线图

**项目**：Rola（2D 横版动作 Roguelike）  
**Unity 版本**：2022.3.62f3c1（2D URP）  
**当前状态**：核心战斗与肉鸽框架完成，美术全占位，MCP 不可用  
**最后更新**：2026-06-17

---

## 当前阶段

**美术填充与职业内容落地。**

TestArena 已可玩：移动、跳跃、冲刺、地面/空中连招、下劈、敌人 AI、打击感反馈均已实现。现在需要把占位资源替换为真实内容，并创建第一个可玩职业「剑士」。

当前 active 计划见 `docs/swordsman-class-plan.md`。

---

## 阶段 1：美术资源（高优先级）

**目标**：替换所有占位 Sprite。  
**阻塞**：后续动画、UI、 polish 都依赖此阶段。

- [ ] 玩家角色 Sprite Sheet
  - Idle、Run（6+ 帧）、Jump、Fall、Attack1/2/3、AirAttack1/2、AirSlam、Dash、Hurt、Dead
  - 导入 `Assets/Sprites/Player/`
- [ ] 敌人 Sprite Sheet
  - Idle、Patrol/Run、Attack、Hurt、Dead
  - 导入 `Assets/Sprites/Enemy/`
- [ ] 场景 Tileset
  - Ground_Main、Ground_Left、Ground_Right
  - 导入 `Assets/Sprites/Environment/`
- [ ] VFX Sprite
  - SlashEffect、HitEffect、DeathEffect
  - 导入 `Assets/Sprites/VFX/`
- [ ] UI Sprite
  - 血条框/填充、4 个技能图标占位、职业图标
  - 导入 `Assets/Sprites/UI/`

---

## 阶段 2：动画系统

**目标**：创建 AnimatorController 并绑定到 Player/Enemy。  
**前置**：阶段 1 的 Sprite 已导入。

- [ ] 创建 `PlayerAnimatorController`
  - 状态：Idle、Run、Jump、Fall、Attack1/2/3、AirAttack1/2、AirSlam、Dash、Hurt、Dead
  - 参数：Speed、IsGrounded、VerticalVelocity、IsAttacking、ComboIndex、IsDashing、IsHurt、IsDead
- [ ] 创建 `EnemyAnimatorController`
  - 状态：Idle、Patrol、Chase、Attack、Hurt、Dead
- [ ] 配置攻击动画事件，确保命中帧与 `AttackData` 对齐
- [ ] 赋值到 Player.prefab / Enemy.prefab

---

## 阶段 3：音频

**目标**：为所有游戏行为添加音效。  
**前置**：无，可与美术并行。

- [ ] 导入 SFX：Jump、Attack（3 变体）、Dash、Hurt、Enemy Attack、Enemy Hurt、Enemy Death
- [ ] 配置 `AudioManager` 并赋值 AudioClip
- [ ] 在 `PlayerController` / `EnemyController` 中指定对应音效字段

---

## 阶段 4：UI

**目标**：把 UI 脚本真正挂到场景里。  
**前置**：阶段 1 的 UI Sprite。

- [ ] 在 TestArena 创建 Canvas
- [ ] 创建 HealthBar，绑定 `PlayerStats.OnHPChanged`
- [ ] 创建 PauseMenu，绑定 Escape 键
- [ ] 创建 MainMenu 场景，加入 Build Settings

---

## 阶段 5：内容填充

**目标**：创建第一个可玩职业与相关数据资产。  
**前置**：阶段 1 的外观 Sprite。  
**当前 active 计划**：`docs/swordsman-class-plan.md`

- [ ] 创建 `Appearance_Swordsman` CharacterAppearanceData
- [ ] 创建剑士各部位 CharacterPartData 占位资产
- [ ] 创建 `Skill_QuickSlash`
- [ ] 创建初始装备 `Equipment_TrainingSword`、`Equipment_LeatherArmor`
- [ ] 填充 `Class_Warrior` / 新建 `Class_Swordsman`
- [ ] 配置 `RunManager.defaultClass`

---

## 阶段 6：打磨

**目标**：手感、性能、视觉 polish。  
**前置**：前述阶段基本完成。

- [ ] 相机跟随参数调优
- [ ] 打击停顿时长调优
- [ ] 粒子系统配置
- [ ] 2D 光照与后处理
- [ ] 扩展 TestArena：平台、敌人出生点、Checkpoint、DeathZone
- [ ] 独立构建测试

---

## 当前服务器限制

- 当前环境**无法运行 Unity Editor**，MCP 不可用。
- 所有需要 Unity Editor 的操作（场景编辑、Prefab 修改、动画状态机、Sprite 导入设置）必须到本地 PC 完成。
- 当前环境适合做的事：
  - 编写 C# 脚本与编辑器辅助工具
  - 创建/修改 YAML 格式的 `.asset` 数据资产
  - 文档整理
  - 代码审查与静态检查

---

## 快速参考：谁做什么

| 任务类型 | 负责人/环境 | 示例 |
|---|---|---|
| 代码与数据资产 | 当前服务器 / OpenCode | 编辑器脚本、.asset YAML、C# 逻辑 |
| 场景/Prefab/动画/导入 | 本地 Unity Editor + MCP | Canvas 创建、AnimatorController、Sprite 设置 |
| 美术资源产出 | 美术工具 | Sprite Sheet、Tileset、UI 素材 |
| 音频资源 | 音频工具 | WAV/OGG 音效、BGM |

---

## 文档索引

- 项目规范：`AGENTS.md`
- 当前完成：`docs/current-state.md`
- 长期欠缺：`docs/gaps.md`
- 当前计划：`docs/swordsman-class-plan.md`
- 脚本速查：`SCRIPTS_REFERENCE.md`
- 美术规范：`ASSETS_GUIDE.md`
- MCP 工作流：`CLAUDE.md`
