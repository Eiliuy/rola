using UnityEngine;

/// <summary>
/// 低血量时触发的效果：护盾 / 回血
/// </summary>
public class OnLowHPEffect : EquipmentEffectBase
{
    private EquipmentEffectType effectType;
    private bool hasTriggered = false;

    public OnLowHPEffect(EquipmentEffectType effectType, float chance, float value, float cooldown, GameObject visualEffect)
        : base(chance, value, cooldown, visualEffect)
    {
        this.effectType = effectType;
    }

    public override void Initialize(PlayerBuild playerBuild)
    {
        base.Initialize(playerBuild);
    }

    public override void OnHPChanged(int currentHP, int maxHP)
    {
        if (maxHP <= 0) return;

        float hpRatio = (float)currentHP / maxHP;
        if (hpRatio > value)
        {
            hasTriggered = false;
            return;
        }

        if (hasTriggered) return;
        if (!CanTrigger()) return;

        hasTriggered = true;

        switch (effectType)
        {
            case EquipmentEffectType.ShieldOnLowHP:
                GrantShield();
                break;
            case EquipmentEffectType.HealOnLowHP:
                HealPlayer(maxHP);
                break;
        }
    }

    void GrantShield()
    {
        // TODO: 接入护盾系统
        Debug.Log($"[装备效果] 低血量获得护盾 {value}");
        PlayerController playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
        SpawnVisualEffect(playerController != null ? playerController.transform.position : Vector3.zero);
    }

    void HealPlayer(int maxHP)
    {
        PlayerController playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
        if (playerController == null) return;
        PlayerStats stats = playerController.GetComponent<PlayerStats>();
        if (stats != null) stats.Heal(Mathf.RoundToInt(maxHP * value));
        SpawnVisualEffect(playerController.transform.position);
    }
}
