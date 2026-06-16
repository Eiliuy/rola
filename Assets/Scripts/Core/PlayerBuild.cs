using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家本局 Build
/// 汇总职业、技能、装备、升级，计算最终属性，管理装备特效
/// </summary>
public class PlayerBuild
{
    public PlayerClassData ClassData { get; private set; }
    public CharacterStats CharacterStats { get; private set; } = new CharacterStats();

    public List<SkillData> Skills { get; private set; } = new List<SkillData>();
    public List<EquipmentData> Equipments { get; private set; } = new List<EquipmentData>();
    public List<UpgradeData> Upgrades { get; private set; } = new List<UpgradeData>();

    public CharacterAppearanceData appearanceData;

    private List<IEquipmentEffect> activeEffects = new List<IEquipmentEffect>();
    private List<EquipmentEffectEntry> specialEffectEntries = new List<EquipmentEffectEntry>();

    public System.Action OnStatsChanged;
    public System.Action OnAppearanceChanged;

    /// <summary>
    /// 用职业初始化 Build
    /// </summary>
    public void Initialize(PlayerClassData classData)
    {
        ClassData = classData;
        Skills.Clear();
        Equipments.Clear();
        Upgrades.Clear();
        specialEffectEntries.Clear();

        if (classData != null)
        {
            CharacterStats.BaseMaxHP = classData.baseMaxHP;
            CharacterStats.BaseAttackPower = classData.baseAttackPower;
            CharacterStats.BaseMoveSpeed = classData.baseMoveSpeed;
            CharacterStats.BaseJumpForce = classData.baseJumpForce;
            CharacterStats.BaseAttackSpeed = classData.baseAttackSpeed;
            CharacterStats.BaseCritChance = classData.baseCritChance;
            CharacterStats.BaseCritDamage = classData.baseCritDamage;
            CharacterStats.BaseCooldownReduction = classData.baseCooldownReduction;
            CharacterStats.BaseAttackRangeMultiplier = classData.baseAttackRangeMultiplier;

            foreach (var skill in classData.startingSkills)
                if (skill != null) Skills.Add(skill);

            foreach (var equipment in classData.startingEquipments)
                if (equipment != null) Equipments.Add(equipment);

            appearanceData = classData.defaultAppearance;
        }

        RebuildEffects();
        RecalculateStats();
    }

    /// <summary>
    /// 重新计算属性
    /// </summary>
    public void RecalculateStats()
    {
        CharacterStats.ClearModifiers();

        if (ClassData != null)
        {
            foreach (var mod in ClassData.classModifiers)
                CharacterStats.AddModifier(mod);
        }

        foreach (var equipment in Equipments)
        {
            if (equipment == null) continue;
            foreach (var mod in equipment.modifiers)
                CharacterStats.AddModifier(mod);
        }

        foreach (var upgrade in Upgrades)
        {
            if (upgrade == null || upgrade.upgradeType != UpgradeType.StatBonus) continue;
            if (upgrade.statModifier != null && upgrade.statModifier.statType != StatType.None)
                CharacterStats.AddModifier(upgrade.statModifier);
        }

        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    public void AddSkill(SkillData skill)
    {
        if (skill == null || Skills.Contains(skill)) return;
        Skills.Add(skill);
    }

    /// <summary>
    /// 升级技能：用新技能替换旧技能
    /// </summary>
    public bool UpgradeSkill(SkillData oldSkill, SkillData newSkill)
    {
        if (oldSkill == null || newSkill == null) return false;
        int index = Skills.IndexOf(oldSkill);
        if (index < 0) return false;
        Skills[index] = newSkill;
        return true;
    }

    /// <summary>
    /// 添加装备
    /// </summary>
    public void AddEquipment(EquipmentData equipment)
    {
        if (equipment == null || Equipments.Contains(equipment)) return;
        Equipments.Add(equipment);
        RebuildEffects();
        RecalculateStats();
    }

    /// <summary>
    /// 移除装备
    /// </summary>
    public void RemoveEquipment(EquipmentData equipment)
    {
        if (equipment == null) return;
        if (Equipments.Remove(equipment))
        {
            RebuildEffects();
            RecalculateStats();
        }
    }

    /// <summary>
    /// 添加升级
    /// </summary>
    public void AddUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null) return;
        Upgrades.Add(upgrade);

        bool shouldRecalculate = true;

        switch (upgrade.upgradeType)
        {
            case UpgradeType.NewSkill:
                AddSkill(upgrade.skillData);
                break;
            case UpgradeType.SkillUpgrade:
                UpgradeSkill(upgrade.skillData, upgrade.upgradedSkill);
                break;
            case UpgradeType.NewEquipment:
                AddEquipment(upgrade.equipmentData);
                shouldRecalculate = false;
                break;
            case UpgradeType.SpecialEffect:
                AddSpecialEffect(upgrade.specialEffect);
                break;
        }

        if (shouldRecalculate)
            RecalculateStats();
    }

    /// <summary>
    /// 添加特殊效果（来自升级奖励）
    /// </summary>
    void AddSpecialEffect(EquipmentEffectEntry entry)
    {
        if (entry == null || entry.effectType == EquipmentEffectType.None) return;
        specialEffectEntries.Add(entry);

        var effect = CreateEffect(entry);
        if (effect != null)
        {
            effect.Initialize(this);
            activeEffects.Add(effect);
        }
    }

    /// <summary>
    /// 重新构建装备特效列表
    /// </summary>
    void RebuildEffects()
    {
        activeEffects.Clear();

        foreach (var equipment in Equipments)
        {
            if (equipment == null) continue;
            foreach (var entry in equipment.effects)
            {
                if (entry == null) continue;
                var effect = CreateEffect(entry);
                if (effect != null)
                {
                    effect.Initialize(this);
                    activeEffects.Add(effect);
                }
            }
        }

        foreach (var entry in specialEffectEntries)
        {
            if (entry == null) continue;
            var effect = CreateEffect(entry);
            if (effect != null)
            {
                effect.Initialize(this);
                activeEffects.Add(effect);
            }
        }
    }

    /// <summary>
    /// 根据配置条目创建装备特效实例
    /// </summary>
    IEquipmentEffect CreateEffect(EquipmentEffectEntry entry)
    {
        if (entry == null) return null;

        switch (entry.effectType)
        {
            case EquipmentEffectType.IgniteOnHit:
            case EquipmentEffectType.BleedOnHit:
            case EquipmentEffectType.StunOnHit:
                return new OnHitEffect(entry.effectType, entry.chance, entry.value, entry.cooldown, entry.visualEffect);

            case EquipmentEffectType.HealOnDodge:
            case EquipmentEffectType.InvincibleOnDodge:
                return new OnDodgeEffect(entry.effectType, entry.chance, entry.value, entry.cooldown, entry.visualEffect);

            case EquipmentEffectType.ShieldOnLowHP:
            case EquipmentEffectType.HealOnLowHP:
                return new OnLowHPEffect(entry.effectType, entry.chance, entry.value, entry.cooldown, entry.visualEffect);

            case EquipmentEffectType.HealOnKill:
            case EquipmentEffectType.GoldOnKill:
            case EquipmentEffectType.AttackSpeedOnKill:
                return new OnKillEffect(entry.effectType, entry.chance, entry.value, entry.cooldown, entry.visualEffect);

            default:
                return null;
        }
    }

    #region 事件转发

    /// <summary>
    /// 命中敌人时触发
    /// </summary>
    public void TriggerOnHitEnemy(IDamageable target, Vector3 hitPosition)
    {
        foreach (var effect in activeEffects)
            effect.OnHitEnemy(target, hitPosition);
    }

    /// <summary>
    /// 闪避时触发
    /// </summary>
    public void TriggerOnDodge()
    {
        foreach (var effect in activeEffects)
            effect.OnDodge();
    }

    /// <summary>
    /// 受到伤害时触发
    /// </summary>
    public void TriggerOnDamaged(int damage, Vector2 attackerPosition)
    {
        foreach (var effect in activeEffects)
            effect.OnDamaged(damage, attackerPosition);
    }

    /// <summary>
    /// 击杀敌人时触发
    /// </summary>
    public void TriggerOnKillEnemy(Vector3 killPosition)
    {
        foreach (var effect in activeEffects)
            effect.OnKillEnemy(killPosition);
    }

    /// <summary>
    /// 血量变化时触发
    /// </summary>
    public void TriggerOnHPChanged(int currentHP, int maxHP)
    {
        foreach (var effect in activeEffects)
            effect.OnHPChanged(currentHP, maxHP);
    }

    #endregion

    /// <summary>
    /// 更换外观
    /// </summary>
    public void SetAppearance(CharacterAppearanceData appearance)
    {
        if (appearance == null || appearanceData == appearance) return;
        appearanceData = appearance;
        OnAppearanceChanged?.Invoke();
    }

    /// <summary>
    /// 计算技能实际冷却时间
    /// </summary>
    public float GetSkillCooldown(SkillData skill)
    {
        if (skill == null) return 0f;
        return skill.cooldown * (1f - CharacterStats.CooldownReduction);
    }

    /// <summary>
    /// 获取最终攻击伤害
    /// </summary>
    public int GetFinalAttackDamage(AttackData attackData)
    {
        if (attackData == null) return 0;
        float baseDamage = attackData.damage * attackData.damageMultiplier;
        return Mathf.RoundToInt(baseDamage * (1f + CharacterStats.AttackPower / 10f));
    }

    /// <summary>
    /// 获取最终攻击范围
    /// </summary>
    public float GetFinalAttackRange(float baseRange, AttackData attackData)
    {
        if (attackData == null) return baseRange;
        return baseRange * attackData.rangeMultiplier * CharacterStats.AttackRangeMultiplier;
    }

    /// <summary>
    /// 获取最终攻击动画时长倍率
    /// </summary>
    public float GetAttackTimeScale()
    {
        return 1f / Mathf.Max(0.1f, CharacterStats.AttackSpeed);
    }
}
