using UnityEngine;
using System.Collections;

/// <summary>
/// 镜头震动（挂载到 Main Camera 上）
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 触发镜头震动
    /// </summary>
    /// <param name="duration">持续时间</param>
    /// <param name="magnitude">震动幅度</param>
    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
}
