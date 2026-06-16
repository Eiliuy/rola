using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家职业数据资产
/// 定义职业基础属性、初始技能与装备
/// </summary>
[CreateAssetMenu(fileName = "Class_", menuName = "Rola/Player Class Data", order = 1)]
public class PlayerClassData : ScriptableObject
{
    [Header("基础信息")]
    public string className = "战士";
    public string description = "";
    public Sprite icon;

    [Header("基础属性")]
    public int baseMaxHP = 100;
    public int baseAttackPower = 10;
    public float baseMoveSpeed = 5f;
    public float baseJumpForce = 12f;
    public float baseAttackSpeed = 1f;
    public float baseCritChance = 0.05f;
    public float baseCritDamage = 1.5f;
    public float baseCooldownReduction = 0f;
    public float baseAttackRangeMultiplier = 1f;

    [Header("初始技能")]
    public List<SkillData> startingSkills = new List<SkillData>();

    [Header("初始装备")]
    public List<EquipmentData> startingEquipments = new List<EquipmentData>();

    [Header("默认外观")]
    public CharacterAppearanceData defaultAppearance;

    [Header("职业特性")]
    public List<StatModifier> classModifiers = new List<StatModifier>();
}
