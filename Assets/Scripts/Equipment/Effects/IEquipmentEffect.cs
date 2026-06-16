using UnityEngine;

/// <summary>
/// 装备特效接口
/// </summary>
public interface IEquipmentEffect
{
    /// <summary>
    /// 初始化时调用
    /// </summary>
    void Initialize(PlayerBuild playerBuild);

    /// <summary>
    /// 命中敌人时调用
    /// </summary>
    void OnHitEnemy(IDamageable target, Vector3 hitPosition);

    /// <summary>
    /// 闪避时调用
    /// </summary>
    void OnDodge();

    /// <summary>
    /// 受到伤害时调用
    /// </summary>
    void OnDamaged(int damage, Vector2 attackerPosition);

    /// <summary>
    /// 击杀敌人时调用
    /// </summary>
    void OnKillEnemy(Vector3 killPosition);

    /// <summary>
    /// 血量变化时调用
    /// </summary>
    void OnHPChanged(int currentHP, int maxHP);
}
