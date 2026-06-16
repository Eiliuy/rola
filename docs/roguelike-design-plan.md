# Rola 肉鸽元素改造计划

## 目标

在不破坏现有核心战斗原型的前提下，引入轻量肉鸽（Roguelike）框架，提升单局 Build 多样性和重玩价值。

核心原则：
- **数据驱动**：大量内容通过 ScriptableObject 配置，减少代码改动。
- **运行时计算**：最终属性由基础属性 + 职业 + 装备 + 升级共同决定。
- **事件解耦**：装备特殊效果通过事件监听实现，不侵入 `PlayerController`。
- **小步快跑**：先搭框架和扩展接口，再逐步填充内容。

---

## 改造范围

### 1. 新增 ScriptableObject 数据资产

位置：`Assets/ScriptableObjects/`

| 资产类型 | 文件 | 用途 |
|---|---|---|
| 职业数据 | `PlayerClassData` | 定义职业基础属性、初始技能、初始装备、职业特性 |
| 技能数据 | `SkillData` | 定义技能的伤害、范围、冷却、动画、特效、可升级分支 |
| 装备数据 | `EquipmentData` | 定义装备部位、属性加成、特殊效果 ID |
| 升级选项 | `UpgradeData` | 定义升级奖励的描述、图标、类型、数值、稀有度 |

**理由**：肉鸽的核心是大量可配置内容。ScriptableObject 是 Unity 最自然的配表方式，策划后续只需在 Inspector 里拖拽配置，无需修改代码。

---

### 2. 新增运行时属性系统

位置：`Assets/Scripts/Core/`

| 类型 | 文件 | 用途 |
|---|---|---|
| 角色属性 | `CharacterStats` | 最终运行时属性：最大生命、攻击力、移动速度、跳跃力、攻击速度、暴击率、暴击伤害、冷却缩减、攻击范围倍率等 |
| 属性修改器 | `StatModifier` | 描述一条属性加成：目标属性、修改方式（加值 / 百分比 / 最终乘区）、数值、来源 |
| 玩家 Build | `PlayerBuild` | 本局肉鸽 Build：职业 + 技能列表 + 装备列表 + 已选升级 + 动态属性 |
| 冷却管理 | `SkillCooldown` | 管理技能冷却时间，支持普通冷却和按比例缩减 |

**理由**：装备和升级都会叠加属性，必须有一个统一计算入口，避免在 `PlayerController` 里到处改字段。后续职业、Buff、装备效果都能通过修改器影响最终数值。

---

### 3. 修改 PlayerStats

**改造点**：
- `Start()` 时从 `PlayerBuild` 初始化血量上限。
- `maxHP` 不再硬编码，改为读取 Build 的 `MaxHP`。
- 提供 `RecalculateStats()` 接口，在装备/升级变化时重新计算。

**保留行为**：
- `TakeDamage`、`Heal` 逻辑不变。
- 无敌帧、重生、事件回调保持不变。

**理由**：血量上限会被装备和升级影响，必须接入运行时属性系统。

---

### 4. 修改 PlayerController

**改造点**：

#### 4.1 数值来源调整

| 当前字段 | 新来源 |
|---|---|
| `moveSpeed` | `PlayerBuild.CharacterStats.MoveSpeed` |
| `jumpForce` | `PlayerBuild.CharacterStats.JumpForce` |
| `dashSpeed` / `dashDuration` / `dashCooldown` | 保留基础值，可被冷却缩减 / 移速倍率影响 |
| 攻击伤害 | `AttackData.damage * PlayerBuild.CharacterStats.AttackPowerMultiplier + AttackPowerBonus` |
| 攻击范围 | `attackRange * data.rangeMultiplier * PlayerBuild.CharacterStats.AttackRangeMultiplier` |
| 攻击速度 | `AttackData.startupTime / AttackSpeedMultiplier` 等 |

#### 4.2 新增机制

- `TryUseSkill(int index)`：根据技能槽位释放技能。
- 技能冷却：通过 `SkillCooldown` 管理。
- 命中事件：在 `PerformAttackHit` 命中敌人时触发 `OnPlayerHitEnemy`。
- 受击事件：触发 `OnPlayerDamaged`。
- 击杀事件：触发 `OnPlayerKilledEnemy`。

**理由**：`PlayerController` 是核心战斗入口，必须改成“读取 Build，输出事件”，后续职业技能、装备特效才能接上来。

**注意**：动画状态机暂时不动，等美术资源到位再统一配置。

---

### 5. 修改 AttackData

**新增字段**：

| 字段 | 用途 |
|---|---|
| `damageMultiplier` | 该段攻击的伤害倍率 |
| `elementType` | 元素类型：无 / 火 / 冰 / 雷（占位） |
| `skillId` | 关联 `SkillData` 的 ID |
| `canCrit` | 是否可暴击 |
| `critChanceOverride` | 覆盖暴击率（-1 表示使用 Build 默认值） |

**理由**：后续技能升级分支、元素反应、暴击特效都需要从攻击数据里拿信息。

---

### 6. 新增装备特效系统

位置：`Assets/Scripts/Equipment/Effects/`

| 组件 | 用途 |
|---|---|
| `IEquipmentEffect` | 接口：定义触发时机和生效逻辑 |
| `EquipmentEffectBase` | 抽象基类，提供触发概率、冷却等通用逻辑 |
| `OnHitEffect` | 命中时概率触发：点燃 / 流血 / 眩晕 |
| `OnDodgeEffect` | 闪避后触发：短暂无敌 / 回血 / 加攻速 |
| `OnLowHPEffect` | 低血量时触发：护盾 / 回血 |
| `OnKillEffect` | 击杀敌人时触发：回血 / 获得货币 / 临时加攻 |

**触发方式**：
- `PlayerController` 在命中、受击、闪避、击杀时触发事件。
- `PlayerBuild` 持有已激活的装备效果列表，统一转发事件。

**理由**：装备特殊效果是肉鸽 Build 的核心乐趣来源，用事件驱动解耦，不要在 `PlayerController` 里写 if-else。

---

### 7. 新增升级奖励系统

位置：`Assets/Scripts/Roguelike/`

| 组件 | 用途 |
|---|---|
| `UpgradeManager` | 管理升级选项池，随机生成三选一奖励 |
| `UpgradeType` | 枚举：属性加成 / 新技能 / 技能升级 / 装备 / 特殊效果 |
| `UpgradeOptionUI` | 升级面板 UI，展示三个选项 |
| `UpgradeRewardPanel` | 控制奖励面板的显示与选择 |

**触发方式**：
- 敌人死亡时掉落经验 / 货币。
- 经验达到阈值时调用 `UpgradeManager.GenerateOptions()`。
- 玩家选择后应用到 `PlayerBuild`。

**理由**：这是肉鸽循环的关键节点，每局通过升级让角色变强。

---

### 8. 新增 Run 数据管理

位置：`Assets/Scripts/Roguelike/`

| 组件 | 用途 |
|---|---|
| `RunManager` | 单例，保存本局所有肉鸽数据 |
| `RunData` | 数据结构：当前职业、持有技能、装备、已选升级、关卡进度、局内货币 |

**RunData 内容**：

```csharp
public class RunData
{
    public PlayerClassData selectedClass;
    public List<SkillData> skills;
    public List<EquipmentData> equipments;
    public List<UpgradeData> upgrades;
    public int currentLevel;
    public int currentRoom;
    public int gold;
    public int exp;
    public int expToNextLevel;
}
```

**理由**：后续关卡选择、商店、Boss 战都需要读取本局状态。

---

## 不建议现在做的

| 不做 | 理由 |
|---|---|
| 敌人 AI 整体重构 | 现有状态机够用，新敌人类型通过扩展配置实现 |
| 动画系统重做 | 等美术资源到位再统一配置 Animator |
| 关卡随机生成 | 先做固定关卡 + 局内 Build，再考虑地图随机 |
| 复杂天赋树 UI | 先实现三选一升级面板，天赋树后补 |
| 网络 / 云存档 | 单机本地运行优先 |
| 多人联机 | 超出当前阶段范围 |

---

## 实施顺序

### 第一阶段：数据与属性骨架

1. 创建 `ScriptableObjects/` 目录。
2. 实现 `PlayerClassData`、`SkillData`、`EquipmentData`、`UpgradeData`。
3. 实现 `StatModifier`、`CharacterStats`、`PlayerBuild`。
4. 实现 `RunManager` 和 `RunData`。

### 第二阶段：接入玩家系统

5. 修改 `PlayerStats` 从 Build 读取血量。
6. 修改 `PlayerController` 从 Build 读取属性，并触发事件。
7. 扩展 `AttackData` 支持倍率和元素类型。

### 第三阶段：装备与升级

8. 实现 `IEquipmentEffect` 和若干示例效果。
9. 实现 `UpgradeManager` 和三选一 UI。
10. 敌人死亡掉落经验，升级时弹出选择面板。

### 第四阶段：职业与技能

11. 实现职业选择界面。
12. 实现技能释放槽位和冷却。
13. 实现技能升级分支（同一技能的多种强化方向）。

### 第五阶段：测试与打磨

14. 编写自动化测试覆盖属性计算、Build 组装、事件触发。
15. 实际 Playtest 调手感和 Build 平衡。

---

## 自动化测试建议

可以编写 Unity Test Framework 测试覆盖以下部分：

| 测试目标 | 说明 |
|---|---|
| 属性计算 | 验证 `CharacterStats` 在多个 `StatModifier` 叠加后结果正确 |
| Build 组装 | 验证职业 + 装备 + 升级组装后的属性正确 |
| 冷却计算 | 验证冷却缩减对技能冷却的影响 |
| 装备效果 | 验证事件触发时效果正确生效 |
| 升级选项生成 | 验证 `UpgradeManager` 能生成指定数量的不重复选项 |

手感测试（跳跃、打击感、战斗节奏）仍需人工 Playtest。

---

## 文件结构预期

```
Assets/
├── ScriptableObjects/
│   ├── Classes/
│   ├── Skills/
│   ├── Equipments/
│   └── Upgrades/
├── Scripts/
│   ├── Core/
│   │   ├── CharacterStats.cs
│   │   ├── StatModifier.cs
│   │   ├── PlayerBuild.cs
│   │   └── SkillCooldown.cs
│   ├── Data/
│   │   ├── PlayerClassData.cs
│   │   ├── SkillData.cs
│   │   ├── EquipmentData.cs
│   │   └── UpgradeData.cs
│   ├── Equipment/
│   │   ├── Effects/
│   │   │   ├── IEquipmentEffect.cs
│   │   │   ├── EquipmentEffectBase.cs
│   │   │   ├── OnHitEffect.cs
│   │   │   ├── OnDodgeEffect.cs
│   │   │   ├── OnLowHPEffect.cs
│   │   │   └── OnKillEffect.cs
│   ├── Roguelike/
│   │   ├── RunManager.cs
│   │   ├── RunData.cs
│   │   ├── UpgradeManager.cs
│   │   └── UpgradeRewardPanel.cs
│   ├── Player/
│   │   ├── PlayerController.cs （修改）
│   │   └── PlayerStats.cs （修改）
│   └── UI/
│       └── UpgradeOptionUI.cs
```

---

## 风险点

1. **属性计算复杂化**：如果修改器类型过多，可能出现叠加顺序问题。建议限制为：基础值 + 加值 + 百分比加成 + 最终乘区。
2. **PlayerController 膨胀**：接入事件后注意不要让控制器代码过多。只负责触发事件，具体效果由 Build 和装备效果处理。
3. **升级平衡**：先做纯数值升级，等框架稳定后再加入改变机制的升级。
4. **动画绑定滞后**：技能系统初期可能只能先复用现有攻击动画，动画差异通过特效和参数区分。

---

## 下一步建议

从第一阶段开始：先实现数据资产和属性系统骨架，再逐步接入玩家系统。这是后续所有肉鸽内容的底层依赖，值得先投入。
