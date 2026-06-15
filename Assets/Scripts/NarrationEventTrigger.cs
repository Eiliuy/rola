using UnityEngine;

/// <summary>
/// 事件型旁白触发器
/// 其他脚本可调用 Trigger() 来播放旁白
/// </summary>
public class NarrationEventTrigger : MonoBehaviour
{
    [Header("旁白")]
    public NarrationData narration;

    [Header("触发设置")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    /// <summary>
    /// 触发旁白
    /// </summary>
    public void Trigger()
    {
        if (triggerOnce && hasTriggered) return;

        NarrationManager.Instance?.PlayNarration(narration);
        hasTriggered = true;
    }

    /// <summary>
    /// 手动重置
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
