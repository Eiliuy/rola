using UnityEngine;

/// <summary>
/// 装备特效基类
/// </summary>
public abstract class EquipmentEffectBase : IEquipmentEffect
{
    protected PlayerBuild playerBuild;
    protected float chance;
    protected float value;
    protected float cooldown;
    protected float lastTriggerTime = -999f;
    protected GameObject visualEffect;

    public EquipmentEffectBase(float chance, float value, float cooldown, GameObject visualEffect)
    {
        this.chance = chance;
        this.value = value;
        this.cooldown = cooldown;
        this.visualEffect = visualEffect;
    }

    public virtual void Initialize(PlayerBuild playerBuild)
    {
        this.playerBuild = playerBuild;
    }

    public virtual void OnHitEnemy(IDamageable target, Vector3 hitPosition) { }
    public virtual void OnDodge() { }
    public virtual void OnDamaged(int damage, Vector2 attackerPosition) { }
    public virtual void OnKillEnemy(Vector3 killPosition) { }
    public virtual void OnHPChanged(int currentHP, int maxHP) { }

    /// <summary>
    /// 检查触发概率和冷却
    /// </summary>
    protected bool CanTrigger()
    {
        if (Time.time - lastTriggerTime < cooldown) return false;
        if (Random.value >= chance) return false;
        lastTriggerTime = Time.time;
        return true;
    }

    /// <summary>
    /// 生成特效
    /// </summary>
    protected void SpawnVisualEffect(Vector3 position)
    {
        if (visualEffect == null) return;
        Object.Instantiate(visualEffect, position, Quaternion.identity);
    }
}
