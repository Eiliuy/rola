using UnityEngine;

/// <summary>
/// 命中时触发的效果：点燃 / 流血 / 眩晕
/// </summary>
public class OnHitEffect : EquipmentEffectBase
{
    private EquipmentEffectType effectType;

    public OnHitEffect(EquipmentEffectType effectType, float chance, float value, float cooldown, GameObject visualEffect)
        : base(chance, value, cooldown, visualEffect)
    {
        this.effectType = effectType;
    }

    public override void OnHitEnemy(IDamageable target, Vector3 hitPosition)
    {
        if (!CanTrigger()) return;

        switch (effectType)
        {
            case EquipmentEffectType.IgniteOnHit:
                ApplyBurn(target, hitPosition);
                break;
            case EquipmentEffectType.BleedOnHit:
                ApplyBleed(target, hitPosition);
                break;
            case EquipmentEffectType.StunOnHit:
                ApplyStun(target, hitPosition);
                break;
        }
    }

    void ApplyBurn(IDamageable target, Vector3 hitPosition)
    {
        // TODO: 接入持续伤害系统（DoT）
        Debug.Log($"[装备效果] 点燃目标，每秒伤害 {value}");
        SpawnVisualEffect(hitPosition);
    }

    void ApplyBleed(IDamageable target, Vector3 hitPosition)
    {
        // TODO: 接入持续伤害系统（DoT）
        Debug.Log($"[装备效果] 流血目标，总伤害 {value}");
        SpawnVisualEffect(hitPosition);
    }

    void ApplyStun(IDamageable target, Vector3 hitPosition)
    {
        // TODO: 接入敌人眩晕状态
        Debug.Log($"[装备效果] 眩晕目标 {value} 秒");
        SpawnVisualEffect(hitPosition);
    }
}
