using System;
using UnityEngine;

/// <summary>
/// 属性修改方式
/// </summary>
public enum StatModType
{
    /// <summary>
    /// 加值，直接加到基础值上
    /// </summary>
    FlatAdd,

    /// <summary>
    /// 百分比加成，基于基础值提升
    /// </summary>
    PercentAdd,

    /// <summary>
    /// 最终乘区，所有加成后整体缩放
    /// </summary>
    FinalMultiplier
}

/// <summary>
/// 可修改的属性类型
/// </summary>
public enum StatType
{
    None,
    MaxHP,
    AttackPower,
    MoveSpeed,
    JumpForce,
    AttackSpeed,
    CritChance,
    CritDamage,
    CooldownReduction,
    AttackRangeMultiplier,
    DamageReduction
}

/// <summary>
/// 单条属性修改器
/// </summary>
[Serializable]
public class StatModifier
{
    [Tooltip("目标属性")]
    public StatType statType;

    [Tooltip("修改方式")]
    public StatModType modType;

    [Tooltip("数值")]
    public float value;

    [Tooltip("来源描述，用于调试")]
    public string source = "";

    public StatModifier() { }

    public StatModifier(StatType statType, StatModType modType, float value, string source = "")
    {
        this.statType = statType;
        this.modType = modType;
        this.value = value;
        this.source = source;
    }
}
