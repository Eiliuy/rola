using UnityEngine;

/// <summary>
/// 升级类型
/// </summary>
public enum UpgradeType
{
    StatBonus,
    NewSkill,
    SkillUpgrade,
    NewEquipment,
    SpecialEffect
}

/// <summary>
/// 升级奖励数据资产
/// </summary>
[CreateAssetMenu(fileName = "Upgrade_", menuName = "Rola/Upgrade Data", order = 4)]
public class UpgradeData : ScriptableObject
{
    [Header("基础信息")]
    public string upgradeName = "升级";
    public string description = "";
    public Sprite icon;
    public UpgradeType upgradeType = UpgradeType.StatBonus;
    public int rarity = 1;

    [Header("属性加成（StatBonus 时生效）")]
    public StatModifier statModifier;

    [Header("技能相关（NewSkill / SkillUpgrade 时生效）")]
    public SkillData skillData;
    public SkillData upgradedSkill;

    [Header("装备相关（NewEquipment 时生效）")]
    public EquipmentData equipmentData;

    [Header("特殊效果（SpecialEffect 时生效）")]
    public EquipmentEffectEntry specialEffect;
}
