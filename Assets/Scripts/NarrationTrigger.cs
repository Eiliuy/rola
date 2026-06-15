using UnityEngine;

/// <summary>
/// 区域型旁白触发器
/// 玩家进入触发区域后播放旁白
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NarrationTrigger : MonoBehaviour
{
    [Header("旁白")]
    public NarrationData narration;

    [Header("触发设置")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && hasTriggered) return;

        NarrationManager.Instance?.PlayNarration(narration);
        hasTriggered = true;
    }

    /// <summary>
    /// 手动重置触发状态
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
