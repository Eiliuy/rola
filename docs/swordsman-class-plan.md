# 剑士职业设计计划

## 目标

创建第一个可玩职业「剑士」，作为默认职业验证肉鸽系统与外观模块化系统的实际运行效果。

---

## 职业定位

- **名称**：剑士 / Swordsman
- **风格**：近战快速斩击，攻守均衡，适合新手
- **主题**：轻装战士，单手长剑，灵活机敏
- **参考感觉**：艾希 ICEY 的主角艾希，但走更传统的剑士路线

---

## 基础属性

| 属性 | 数值 | 说明 |
|---|---|---|
| 最大生命 | 120 | 中等偏上 |
| 攻击力 | 12 | 中等 |
| 移动速度 | 5.0 | 标准 |
| 跳跃力 | 12.0 | 标准 |
| 攻击速度 | 1.0 | 标准 |
| 暴击率 | 5% | 较低 |
| 暴击伤害 | 150% | 标准 |
| 冷却缩减 | 0% | 无 |
| 攻击范围倍率 | 1.0 | 标准 |

---

## 初始技能

### 1. 迅斩（Quick Slash）

- **按键**：1 / 技能槽 0
- **冷却**：3 秒
- **消耗**：无 MP
- **效果**：向前方快速突进并斩击，造成 150% 攻击力伤害，击退敌人
- **动作**：短距离冲刺 + 水平斩击
- **升级分支**：
  - 分支 A：增加突进距离与伤害
  - 分支 B：斩击后追加一次回旋斩
  - 分支 C：命中后短暂提升移动速度

---

## 初始装备

### 1. 训练用长剑

- **部位**：Weapon
- **效果**：攻击力 +2
- **特殊效果**：无

### 2. 皮制护胸

- **部位**：Armor（通过 Top 部位表现）
- **效果**：最大生命 +20
- **特殊效果**：无

---

## 职业特性

```
StatModifier:
- AttackPower +2 (FlatAdd)
- AttackSpeed +0.1 (FlatAdd)
```

剑士略微偏向攻击输出，血量与机动性保持均衡。

---

## 外观方向

基于之前选定的「动漫风 + 特工/剑士」融合：

- **发型**：银白色短发，利落
- **上衣**：黑色短款战斗夹克，红色内衬
- **下装**：黑色紧身裤
- **鞋子**：黑色短靴
- **武器**：单手长剑，剑刃带淡蓝光效
- **手套**：黑色半指手套
- **配饰**：无或简单腰带

初期用占位 Sprite 或简单几何形状，后续替换为美术资源。

---

## 实施步骤

### 第一步：创建 ScriptableObject 资产

1. 在 `Assets/ScriptableObjects/Classes/` 下创建 `Class_Swordsman.asset`
2. 在 `Assets/ScriptableObjects/Appearances/` 下创建 `Appearance_Swordsman.asset`
3. 在 `Assets/ScriptableObjects/Appearances/Parts/` 下创建剑士各部位资产
4. 在 `Assets/ScriptableObjects/Skills/` 下创建 `Skill_QuickSlash.asset`
5. 在 `Assets/ScriptableObjects/Equipments/` 下创建初始装备资产

### 第二步：配置 RunManager 默认职业

将 `Class_Swordsman` 赋值给 `RunManager.defaultClass`。

### 第三步：调整玩家预制体

- 确保 `Player` 预制体包含：
  - `PlayerController`
  - `PlayerStats`
  - `PlayerVisualController`
  - `CharacterAppearanceRenderer`（含各部位 Holder）
  - `PlayerAnimationController`（含 Animator）

### 第四步：生成占位美术资源

- 用 Unity 编辑器脚本或简单 Sprite 生成占位方块
- 各部位用不同颜色区分，方便观察换装效果

### 第五步：Playtest 验证

- 进入场景，确认剑士默认外观正确显示
- 移动、跳跃、攻击、闪避、受击、重生正常
- 使用技能「迅斩」能造成伤害与动画触发
- 升级后属性变化正确反映到战斗中

---

## 产出文件

```
Assets/
├── ScriptableObjects/
│   ├── Classes/
│   │   └── Class_Swordsman.asset
│   ├── Skills/
│   │   └── Skill_QuickSlash.asset
│   ├── Equipments/
│   │   ├── Equipment_TrainingSword.asset
│   │   └── Equipment_LeatherArmor.asset
│   └── Appearances/
│       ├── Appearance_Swordsman.asset
│       └── Parts/
│           ├── Body/
│           ├── Hair/
│           ├── Top/
│           ├── Bottom/
│           ├── Shoes/
│           ├── Gloves/
│           ├── Weapon/
│           └── Accessory/
```

---

## 下一步

先创建 ScriptableObject 资产和占位 Sprite，然后配置玩家预制体，最后 Playtest 验证。

要我直接写个 Unity 编辑器脚本来自动生成这些占位资产吗？这样可以省去在 Inspector 里一个个手动创建的时间。
