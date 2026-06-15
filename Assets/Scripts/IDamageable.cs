using UnityEngine;

/// <summary>
/// 可受伤对象接口，玩家和敌人都可实现
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="knockback">击退方向和力度</param>
    /// <param name="attackerPosition">攻击者位置，用于判断击退方向</param>
    void TakeDamage(int damage, Vector2 knockback, Vector2 attackerPosition);

    /// <summary>
    /// 当前是否处于无敌状态
    /// </summary>
    bool IsInvincible { get; }
}
