using UnityEngine;

/// <summary>
/// 存档点/检查点
/// 玩家进入后更新重生位置
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("视觉")]
    public SpriteRenderer spriteRenderer;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.gray;

    [Header("特效")]
    public ParticleSystem activateEffect;
    public AudioClip activateSound;

    private bool isActivated = false;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisual();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats != null)
        {
            Activate(stats);
        }
    }

    void Activate(PlayerStats stats)
    {
        isActivated = true;
        stats.SetRespawnPoint(transform.position);

        if (activateEffect != null)
            Instantiate(activateEffect, transform.position, Quaternion.identity);

        if (activateSound != null)
            AudioSource.PlayClipAtPoint(activateSound, transform.position);

        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = isActivated ? activeColor : inactiveColor;
    }
}
