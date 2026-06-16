using UnityEngine;

/// <summary>
/// 击杀敌人时触发的效果：回血 / 金币 / 临时攻速
/// </summary>
public class OnKillEffect : EquipmentEffectBase
{
    private EquipmentEffectType effectType;

    public OnKillEffect(EquipmentEffectType effectType, float chance, float value, float cooldown, GameObject visualEffect)
        : base(chance, value, cooldown, visualEffect)
    {
        this.effectType = effectType;
    }

    public override void Initialize(PlayerBuild playerBuild)
    {
        base.Initialize(playerBuild);
    }

    public override void OnKillEnemy(Vector3 killPosition)
    {
        if (!CanTrigger()) return;

        switch (effectType)
        {
            case EquipmentEffectType.HealOnKill:
                HealPlayer();
                break;
            case EquipmentEffectType.GoldOnKill:
                AddGold();
                break;
            case EquipmentEffectType.AttackSpeedOnKill:
                AddAttackSpeed();
                break;
        }

        SpawnVisualEffect(killPosition);
    }

    void HealPlayer()
    {
        PlayerController playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
        if (playerController == null) return;
        PlayerStats stats = playerController.GetComponent<PlayerStats>();
        if (stats != null) stats.Heal(Mathf.RoundToInt(value));
    }

    void AddGold()
    {
        RunManager.Instance?.AddGold(Mathf.RoundToInt(value));
    }

    void AddAttackSpeed()
    {
        // TODO: 接入临时 Buff 系统
        Debug.Log($"[装备效果] 击杀后临时攻速 +{value}");
    }
}
