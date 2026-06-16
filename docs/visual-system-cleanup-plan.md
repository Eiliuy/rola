# Rola 视觉系统旧逻辑清理计划

## 目标

在项目初期彻底清理 `PlayerController` 中遗留的旧视觉逻辑，统一走 `PlayerVisualController` 模块，避免两套系统并存导致维护困难和表现不同步。

---

## 清理原则

1. **单一入口**：所有玩家视觉表现必须通过 `PlayerVisualController`。
2. **移除冗余**：删除 `PlayerController` 中直接操作 `animator` 和 `spriteRenderer` 的代码。
3. **预制体适配**：玩家预制体上保留 Animator，但由 `PlayerVisualController` / `PlayerAnimationController` 持有和操作。
4. **报错明确**：如果关键视觉组件缺失，在 Start 中给出清晰错误提示，而不是静默兼容。

---

## 清理范围

### 1. PlayerController.cs

需要删除/修改的内容：

#### 删除字段

```csharp
public Animator animator;        // 删除
public SpriteRenderer spriteRenderer; // 删除
```

#### Start() 中删除初始化

```csharp
// 删除以下两行
if (animator == null) animator = GetComponent<Animator>();
if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
```

保留并强化：

```csharp
if (visualController == null)
    visualController = GetComponent<PlayerVisualController>();
if (visualController == null)
    Debug.LogError("[PlayerController] 未找到 PlayerVisualController，角色视觉将无法正常工作。", this);
```

#### 受击颜色逻辑删除

当前代码：

```csharp
if (spriteRenderer != null)
    spriteRenderer.color = Color.red;

visualController?.FlashHurt();
```

改为：

```csharp
visualController?.FlashHurt();
```

#### Respawn / EndHurt 颜色逻辑删除

当前代码：

```csharp
if (spriteRenderer != null)
    spriteRenderer.color = Color.white;
```

改为完全由 `PlayerVisualController.OnPlayerRespawned` 处理重置。

#### UpdateAnimations 删除旧 animator 分支

当前同时写入 `visualController` 和 `animator`，改为只写 `visualController`。

```csharp
void UpdateAnimations()
{
    if (visualController == null) return;

    visualController.SetAnimationFloat("Speed", Mathf.Abs(rb.velocity.x));
    visualController.SetAnimationBool("IsGrounded", isGrounded);
    visualController.SetAnimationFloat("VerticalVelocity", rb.velocity.y);
    visualController.SetAnimationBool("IsAttacking", attackPhase != AttackPhase.None);
    visualController.SetAnimationInteger("ComboIndex", currentComboIndex);
    visualController.SetAnimationBool("IsDashing", isDashing);
    visualController.SetAnimationBool("IsHurt", isHurt);
    if (stats != null)
        visualController.SetAnimationBool("IsDead", stats.IsDead);
}
```

#### TryUseSkill 删除旧 animator 分支

当前代码：

```csharp
if (!string.IsNullOrEmpty(skill.animationTrigger))
{
    visualController?.SetAnimationTrigger(skill.animationTrigger);
    if (animator != null)
        animator.SetTrigger(skill.animationTrigger);
}
```

改为：

```csharp
if (!string.IsNullOrEmpty(skill.animationTrigger))
    visualController?.SetAnimationTrigger(skill.animationTrigger);
```

---

### 2. PlayerVisualController.cs

强化为视觉唯一入口：

- `Start()` 中如果找不到 `appearanceRenderer` 或 `animationController`，报错误而非静默跳过。
- 提供 `ClearVisualComponents()` 等调试辅助（可选）。
- 确保 `ApplyCurrentAppearance()` 在 Start 时强制应用一次。

---

### 3. PlayerAnimationController.cs

无改动，已经是 Animator 的唯一封装。

---

### 4. CharacterAppearanceRenderer.cs

无改动，已经是 Sprite 渲染的唯一封装。

---

### 5. 预制体要求

玩家预制体结构应变为：

```
Player
├── Rigidbody2D
├── PlayerController
├── PlayerStats
├── PlayerVisualController
├── PlayerAnimationController （可挂在同一物体或子物体）
├── CharacterAppearanceRenderer （可挂在同一物体或子物体）
└── VisualRoot
    ├── BackHair (SpriteRenderer)
    ├── Body (SpriteRenderer)
    ├── Bottom (SpriteRenderer)
    ├── Top (SpriteRenderer)
    ├── Gloves (SpriteRenderer)
    ├── Shoes (SpriteRenderer)
    ├── Weapon (SpriteRenderer)
    ├── FrontHair (SpriteRenderer)
    └── Accessory (SpriteRenderer)
```

根对象上**不再**需要 `SpriteRenderer` 和 `Animator` 直接挂载；Animator 可以挂在 `PlayerAnimationController` 所在物体上。

---

## 实施步骤

1. 修改 `PlayerController.cs`，删除 `animator` 和 `spriteRenderer` 字段及相关逻辑。
2. 修改 `PlayerVisualController.cs`，强化错误提示和唯一入口职责。
3. 运行静态检查（括号匹配、语法审查）。
4. 在 Unity 中调整玩家预制体：
   - 移除根对象 `SpriteRenderer`
   - 将 `Animator` 移到 `PlayerAnimationController` 同一物体
   - 配置 `PlayerVisualController` 引用
   - 配置 `CharacterAppearanceRenderer` 的各部位 Holder
5. 创建默认外观数据资产并赋值给 `PlayerClassData.defaultAppearance`。
6. Playtest 验证：移动、攻击、受击、死亡重生、技能释放、换装。

---

## 风险与回退

| 风险 | 说明 | 回退方式 |
|---|---|---|
| 预制体配置遗漏 | 删除旧字段后，旧预制体上的引用丢失 | 重新拖拽配置 `PlayerVisualController` 和 `CharacterAppearanceRenderer` |
| 动画不触发 | `PlayerAnimationController` 没拿到 Animator | 检查 Animator 是否挂在同一物体并正确引用 |
| 受击不闪烁 | `CharacterAppearanceRenderer` 没配置部位 Holder | 检查各部位 Holder 是否赋值 |
| 根对象 SpriteRenderer 残留 | 旧代码删除了，但预制体上还有，会和新系统重叠显示 | 删除根对象 SpriteRenderer 或清空其 Sprite |

---

## 预期结果

`PlayerController` 只保留战斗逻辑：`Move`、`Jump`、`Attack`、`Dash`、`TakeDamage`、`TryUseSkill` 等。
所有视觉相关逻辑：`Sprite 渲染`、`Animator 参数`、`受击闪烁`、`外观切换`，全部收敛到 `PlayerVisualController` 及其子模块。

这样代码边界清晰，后续做 Mod、换装、新外观资产时不会互相影响。
