using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装备部位
/// </summary>
public enum EquipmentSlot
{
    Weapon,
    Armor,
    Accessory,
    Trinket
}

/// <summary>
/// 装备数据资产
/// </summary>
[CreateAssetMenu(fileName = "Equipment_", menuName = "Rola/Equipment Data", order = 3)]
public class EquipmentData : ScriptableObject
{
    [Header("基础信息")]
    public string equipmentName = "装备";
    public string description = "";
    public Sprite icon;
    public EquipmentSlot slot = EquipmentSlot.Accessory;
    public int rarity = 1;

    [Header("属性加成")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    [Header("特殊效果")]
    public List<EquipmentEffectEntry> effects = new List<EquipmentEffectEntry>();
}

/// <summary>
/// 装备特殊效果条目
/// </summary>
[System.Serializable]
public class EquipmentEffectEntry
{
    public EquipmentEffectType effectType;
    public float chance = 1f;
    public float value = 0f;
    public float cooldown = 0f;
    public ElementType elementType = ElementType.None;
    public GameObject visualEffect;
}

/// <summary>
/// 装备特效类型
/// </summary>
public enum EquipmentEffectType
{
    None,
    IgniteOnHit,
    BleedOnHit,
    StunOnHit,
    HealOnDodge,
    InvincibleOnDodge,
    ShieldOnLowHP,
    HealOnLowHP,
    HealOnKill,
    GoldOnKill,
    AttackSpeedOnKill
}
