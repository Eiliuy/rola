using UnityEngine;

/// <summary>
/// 刀光/斩击特效
/// 挂载到一个带 SpriteRenderer 的 GameObject 上，会自动缩放、淡出并销毁
/// </summary>
public class SlashEffect : MonoBehaviour
{
    [Header("生命周期")]
    public float lifetime = 0.15f;

    [Header("缩放曲线")]
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("透明度曲线")]
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("旋转偏移")]
    public float rotationOffset = 0f;

    private SpriteRenderer sr;
    private float timer;
    private Vector3 startScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;
        timer = 0f;

        // 应用旋转偏移
        transform.Rotate(0, 0, rotationOffset);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifetime);

        transform.localScale = startScale * scaleCurve.Evaluate(t);

        if (sr != null)
        {
            Color c = sr.color;
            c.a = alphaCurve.Evaluate(t);
            sr.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    /// <summary>
    /// 初始化刀光：设置朝向和缩放
    /// </summary>
    public void Setup(bool facingRight, float scaleMultiplier = 1f)
    {
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        scale *= scaleMultiplier;
        transform.localScale = scale;
        startScale = transform.localScale;
    }
}
