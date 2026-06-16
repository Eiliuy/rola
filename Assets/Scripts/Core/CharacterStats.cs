using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运行时角色属性容器
/// 负责汇总基础属性与所有修改器，计算最终数值
/// </summary>
public class CharacterStats
{
    public int BaseMaxHP { get; set; } = 100;
    public int BaseAttackPower { get; set; } = 10;
    public float BaseMoveSpeed { get; set; } = 5f;
    public float BaseJumpForce { get; set; } = 12f;
    public float BaseAttackSpeed { get; set; } = 1f;
    public float BaseCritChance { get; set; } = 0.05f;
    public float BaseCritDamage { get; set; } = 1.5f;
    public float BaseCooldownReduction { get; set; } = 0f;
    public float BaseAttackRangeMultiplier { get; set; } = 1f;
    public float BaseDamageReduction { get; set; } = 0f;

    private List<StatModifier> modifiers = new List<StatModifier>();

    public int MaxHP => Mathf.RoundToInt(Calculate(BaseMaxHP, StatType.MaxHP));
    public int AttackPower => Mathf.RoundToInt(Calculate(BaseAttackPower, StatType.AttackPower));
    public float MoveSpeed => Calculate(BaseMoveSpeed, StatType.MoveSpeed);
    public float JumpForce => Calculate(BaseJumpForce, StatType.JumpForce);
    public float AttackSpeed => Calculate(BaseAttackSpeed, StatType.AttackSpeed);
    public float CritChance => Mathf.Clamp01(Calculate(BaseCritChance, StatType.CritChance));
    public float CritDamage => Calculate(BaseCritDamage, StatType.CritDamage);
    public float CooldownReduction => Mathf.Clamp01(Calculate(BaseCooldownReduction, StatType.CooldownReduction));
    public float AttackRangeMultiplier => Calculate(BaseAttackRangeMultiplier, StatType.AttackRangeMultiplier);
    public float DamageReduction => Mathf.Clamp01(Calculate(BaseDamageReduction, StatType.DamageReduction));

    /// <summary>
    /// 添加修改器
    /// </summary>
    public void AddModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        modifiers.Add(modifier);
    }

    /// <summary>
    /// 移除修改器
    /// </summary>
    public void RemoveModifier(StatModifier modifier)
    {
        if (modifier == null) return;
        modifiers.Remove(modifier);
    }

    /// <summary>
    /// 清空所有修改器
    /// </summary>
    public void ClearModifiers()
    {
        modifiers.Clear();
    }

    /// <summary>
    /// 获取所有修改器
    /// </summary>
    public IReadOnlyList<StatModifier> GetModifiers()
    {
        return modifiers;
    }

    /// <summary>
    /// 计算指定属性的最终值
    /// </summary>
    float Calculate(float baseValue, StatType statType)
    {
        float flatAdd = 0f;
        float percentAdd = 0f;
        float finalMultiplier = 1f;

        foreach (var mod in modifiers)
        {
            if (mod.statType != statType) continue;

            switch (mod.modType)
            {
                case StatModType.FlatAdd:
                    flatAdd += mod.value;
                    break;
                case StatModType.PercentAdd:
                    percentAdd += mod.value;
                    break;
                case StatModType.FinalMultiplier:
                    finalMultiplier *= mod.value;
                    break;
            }
        }

        float value = (baseValue + flatAdd) * (1f + percentAdd) * finalMultiplier;
        return value;
    }
}
