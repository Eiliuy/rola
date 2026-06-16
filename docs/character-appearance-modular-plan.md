# Rola 角色外观模块化改造计划

## 目标

将玩家角色外观拆分为独立的数据资产和渲染模块，支持：
- 角色服装、发型、肤色、眼睛、武器等部位自由组合
- 职业默认外观配置
- 后续 Mod 替换单个 Sprite 即可改变角色形象
- 为玩家提供自定义外观界面预留接口

---

## 改造原则

1. **数据驱动**：每个外观部位使用 `ScriptableObject` 配置，策划/Mod 作者只需换图。
2. **层级渲染**：使用多个 `SpriteRenderer` 分层叠加，避免单一 Sprite 导致换装困难。
3. **向后兼容**：保留当前 `PlayerController` 的核心战斗逻辑，视觉部分独立出去。
4. **可扩展**：新增部位或覆盖动画时尽量不动核心代码。

---

## 新增文件

### 1. 数据资产

位置：`Assets/Scripts/Data/`

| 文件 | 用途 |
|---|---|
| `CharacterAppearanceData.cs` | 角色整体外观数据资产，汇总各部位配置 |
| `CharacterPartData.cs` | 单个部位数据资产（上衣、裤子、发型等），支持多版本/变体 |

### 2. 渲染系统

位置：`Assets/Scripts/Player/Visual/`

| 文件 | 用途 |
|---|---|
| `CharacterAppearanceRenderer.cs` | 根据外观数据组装 Sprite 层级 |
| `CharacterPartSlot.cs` | 枚举部位类型，定义各部位的默认排序层级 |
| `PlayerVisualController.cs` | 连接 PlayerController 与外观渲染，处理朝向/动画事件 |

### 3. 动画系统

位置：`Assets/Scripts/Player/Visual/`

| 文件 | 用途 |
|---|---|
| `PlayerAnimationController.cs` | 统一播放攻击、受击、死亡等动画事件，支持按外观覆盖动画 |

---

## 修改文件

| 文件 | 改造点 |
|---|---|
| `Assets/Scripts/Data/PlayerClassData.cs` | 新增默认外观 `defaultAppearance` 字段 |
| `Assets/Scripts/Core/PlayerBuild.cs` | 新增 `CharacterAppearanceData appearanceData` 字段，提供换装接口 |
| `Assets/Scripts/PlayerController.cs` | 移除对 `spriteRenderer` 的直接颜色控制，改为通过 `PlayerVisualController` 触发事件 |
| `Assets/Scripts/PlayerStats.cs` | 受击/重生时触发视觉事件，不再直接改 SpriteRenderer.color |
| `Assets/Scripts/GameInitializer.cs` | 无改动，已满足需求 |

---

## 角色部位拆分

层级顺序（从后到前）：

```
BackHair          // 后发
Body              // 身体/肤色
Bottom            // 下装
Top               // 上衣/外套
Shoes             // 鞋子
Gloves            // 手套
Weapon            // 武器
FrontHair         // 前发
Accessory         // 配饰（围巾、披风、眼镜等）
```

每个部位对应一个独立的 `SpriteRenderer`，挂载在玩家预制体下。

---

## 数据资产设计

### CharacterPartData

```csharp
[CreateAssetMenu(fileName = "Part_", menuName = "Rola/Character Part Data")]
public class CharacterPartData : ScriptableObject
{
    public string partName;
    public CharacterPartSlot slot;
    public Sprite sprite;
    public Color colorTint = Color.white;
    public Vector2 pivotOverride = new Vector2(0.5f, 0.5f);
}
```

### CharacterAppearanceData

```csharp
[CreateAssetMenu(fileName = "Appearance_", menuName = "Rola/Character Appearance Data")]
public class CharacterAppearanceData : ScriptableObject
{
    public string appearanceName;
    public Sprite bodySprite;
    public Color skinColor;
    public Color eyeColor;
    public CharacterPartData backHair;
    public CharacterPartData frontHair;
    public CharacterPartData top;
    public CharacterPartData bottom;
    public CharacterPartData gloves;
    public CharacterPartData shoes;
    public CharacterPartData accessory;
    public CharacterPartData weapon;
    public RuntimeAnimatorController animatorController;
}
```

---

## 渲染流程

1. `PlayerVisualController.Start()` 从 `PlayerBuild` 获取当前外观数据。
2. 调用 `CharacterAppearanceRenderer.ApplyAppearance(data)`。
3. `CharacterAppearanceRenderer` 遍历所有部位槽位，设置对应 `SpriteRenderer` 的 sprite 和 color。
4. 后续换装时，再次调用 `ApplyAppearance` 即可局部刷新。

---

## 与现有系统的对接

### 受击红色闪烁

当前 `PlayerController` 直接修改 `spriteRenderer.color = Color.red`。改造后：

- `PlayerVisualController` 监听 `PlayerStats.OnHPChanged` 或暴露 `FlashColor(Color color, float duration)`。
- 受击时调用 `PlayerVisualController.FlashColor(Color.red, 0.1f)`，对所有部位 SpriteRenderer 统一做材质/颜色闪烁。

### 朝向翻转

当前 `PlayerController.Flip()` 直接缩放 `transform.localScale.x *= -1`。此方式仍可复用，会正确翻转整个角色层级。

### 动画系统

当前 `PlayerController` 通过 `animator.SetFloat/SetBool/SetTrigger` 驱动。改造后：

- 如果外观配置了独立的 `RuntimeAnimatorController`，由 `PlayerAnimationController` 切换 Animator 控制器。
- `PlayerController` 不再直接持有 `Animator` 引用，改为持有 `PlayerAnimationController`。
- `PlayerAnimationController` 提供 `SetFloat`、`SetBool`、`SetTrigger` 接口，转发到当前 Animator。

---

## Mod 支持

### 最小 Mod 形式

Mod 作者只需要创建新的 `CharacterPartData` 和 `CharacterAppearanceData` ScriptableObject，并替换 Sprite 资源：

```
Mods/MyMod/
├── Sprites/
│   ├── my_hair.png
│   ├── my_top.png
│   └── my_weapon.png
└── Data/
    ├── MyHair.asset
    ├── MyTop.asset
    ├── MyWeapon.asset
    └── MyAppearance.asset
```

### 加载方式（远期）

- 使用 `Resources.Load` 或 Addressables 动态加载 `CharacterAppearanceData`。
- 启动时扫描 `Mods/` 目录下的外观数据，加入可选列表。

---

## 实施顺序

### 第一阶段：数据与渲染骨架

1. 创建 `CharacterPartSlot.cs` 枚举。
2. 创建 `CharacterPartData.cs` 数据资产。
3. 创建 `CharacterAppearanceData.cs` 数据资产。
4. 创建 `CharacterAppearanceRenderer.cs`，实现按数据装配 Sprite。
5. 创建 `PlayerAnimationController.cs`，封装 Animator 操作。
6. 创建 `PlayerVisualController.cs`，连接数据和渲染。

### 第二阶段：接入玩家系统

7. 在 `PlayerClassData` 中新增 `defaultAppearance`。
8. 在 `PlayerBuild` 中新增外观字段和换装接口。
9. 创建新的玩家预制体 `Player_WithVisual.prefab`，添加各部位 SpriteRenderer。
10. 修改 `PlayerController`：移除直接对 spriteRenderer 的引用和颜色修改。
11. 修改 `PlayerStats`：受击/重生时通过事件触发视觉反馈。

### 第三阶段：测试与示例

12. 创建示例外观资产（默认特工外观 + 几个变体）。
13. 创建示例部位资产（几件上衣、裤子、发型、武器）。
14. 运行测试：切换外观、受击闪烁、朝向翻转、动画正常。
15. 编写外观切换测试脚本（可选编辑器窗口）。

---

## 文件结构预期

```
Assets/
├── Scripts/
│   ├── Data/
│   │   ├── CharacterAppearanceData.cs
│   │   ├── CharacterPartData.cs
│   │   └── PlayerClassData.cs （修改）
│   ├── Core/
│   │   └── PlayerBuild.cs （修改）
│   ├── Player/
│   │   ├── PlayerController.cs （修改）
│   │   ├── PlayerStats.cs （修改）
│   │   └── Visual/
│   │       ├── CharacterPartSlot.cs
│   │       ├── CharacterAppearanceRenderer.cs
│   │       ├── PlayerAnimationController.cs
│   │       └── PlayerVisualController.cs
├── Prefabs/
│   └── Player/
│       └── Player_WithVisual.prefab
└── ScriptableObjects/
    └── Appearances/
        ├── DefaultAgent.asset
        ├── AgentVariantA.asset
        └── Parts/
            ├── Hair/
            ├── Top/
            ├── Bottom/
            ├── Weapon/
            └── Accessory/
```

---

## 风险点

1. **动画覆盖复杂**：如果每套外观动画差异大，需要统一骨骼或动作命名。初期建议所有外观共用同一套 Animator 状态机，只换 Sprite。
2. **Sorting Layer 冲突**：多个 SpriteRenderer 叠加时，需要正确设置 `sortingOrder`，避免头发盖住脸或武器被身体挡住。
3. **命中框与视觉对不齐**：换装后攻击点位置可能变化，建议 `attackPoint` 是独立 Transform，不随部位 Sprite 偏移。
4. **性能**：部位过多时 DrawCall 会增加。初期角色部位控制在 8 个以内，后续可用 Sprite Atlas 或合并贴图优化。

---

## 下一步建议

先实现第一阶段：数据资产和渲染骨架。这是后续所有外观内容的基础。完成后即可在 Unity 里用占位方块测试换装逻辑，不需要等最终美术资源。
