using UnityEngine;

/// <summary>
/// 死亡区域：掉入陷阱/虚空时立即击杀
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DeathZone : MonoBehaviour
{
    [Header("伤害")]
    public int damage = 999;
    public Vector2 knockback = Vector2.up * 10f;

    [Header("只对玩家生效")]
    public bool onlyAffectPlayer = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (onlyAffectPlayer && !other.CompareTag("Player"))
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !damageable.IsInvincible)
        {
            damageable.TakeDamage(damage, knockback, transform.position);
        }
    }
}
