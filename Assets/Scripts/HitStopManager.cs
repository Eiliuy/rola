using UnityEngine;
using System.Collections;

/// <summary>
/// 打击停顿管理器：命中瞬间让时间短暂变慢，增强打击感
/// </summary>
public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 触发打击停顿
    /// </summary>
    /// <param name="duration">停顿时间（秒）</param>
    /// <param name="timeScale">停顿期间的时间缩放，默认 0.05</param>
    public void TriggerHitStop(float duration, float timeScale = 0.05f)
    {
        StopAllCoroutines();
        StartCoroutine(HitStopCoroutine(duration, timeScale));
    }

    IEnumerator HitStopCoroutine(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
