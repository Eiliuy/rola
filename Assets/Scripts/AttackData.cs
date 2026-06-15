using UnityEngine;

/// <summary>
/// 单段攻击数据，用于配置连招的每一击
/// </summary>
[System.Serializable]
public class AttackData
{
    [Tooltip("攻击名称，用于调试")]
    public string attackName = "Attack";

    [Tooltip("伤害值")]
    public int damage = 10;

    [Tooltip("前摇时间：出手前无法命中")]
    public float startupTime = 0.1f;

    [Tooltip("命中判定持续时间")]
    public float activeTime = 0.15f;

    [Tooltip("后摇时间：可输入下一段攻击的窗口")]
    public float recoveryTime = 0.25f;

    [Tooltip("可预输入下一段攻击的时间窗口（后摇内）")]
    public float inputBufferTime = 0.2f;

    [Tooltip("水平击退力度")]
    public float knockbackForce = 5f;

    [Tooltip("垂直击退力度")]
    public float knockbackUpForce = 2f;

    [Tooltip("命中停顿时间")]
    public float hitStopDuration = 0.05f;

    [Tooltip("镜头震动强度")]
    public float cameraShakeMagnitude = 0.08f;

    [Tooltip("镜头震动时间")]
    public float cameraShakeDuration = 0.08f;

    [Tooltip("攻击范围缩放，基于 PlayerController.attackRange")]
    public float rangeMultiplier = 1f;

    [Tooltip("空中是否可用")]
    public bool usableInAir = true;
}
