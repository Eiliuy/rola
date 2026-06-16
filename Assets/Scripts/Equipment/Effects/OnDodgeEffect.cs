using UnityEngine;

/// <summary>
/// 闪避时触发的效果：回血 / 短暂无敌
/// </summary>
public class OnDodgeEffect : EquipmentEffectBase
{
    private EquipmentEffectType effectType;

    public OnDodgeEffect(EquipmentEffectType effectType, float chance, float value, float cooldown, GameObject visualEffect)
        : base(chance, value, cooldown, visualEffect)
    {
        this.effectType = effectType;
    }

    public override void Initialize(PlayerBuild playerBuild)
    {
        base.Initialize(playerBuild);
    }

    public override void OnDodge()
    {
        if (!CanTrigger()) return;

        switch (effectType)
        {
            case EquipmentEffectType.HealOnDodge:
                HealPlayer();
                break;
            case EquipmentEffectType.InvincibleOnDodge:
                GrantInvincibility();
                break;
        }
    }

    PlayerController GetPlayer()
    {
        return UnityEngine.Object.FindObjectOfType<PlayerController>();
    }

    void HealPlayer()
    {
        PlayerController playerController = GetPlayer();
        if (playerController == null) return;
        PlayerStats stats = playerController.GetComponent<PlayerStats>();
        if (stats != null) stats.Heal(Mathf.RoundToInt(value));
        SpawnVisualEffect(playerController.transform.position);
    }

    void GrantInvincibility()
    {
        PlayerController playerController = GetPlayer();
        // TODO: 接入玩家无敌状态
        Debug.Log($"[装备效果] 闪避后无敌 {value} 秒");
        SpawnVisualEffect(playerController != null ? playerController.transform.position : Vector3.zero);
    }
}
