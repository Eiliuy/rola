using UnityEngine;

/// <summary>
/// 死亡特效：播放粒子、音效，一段时间后销毁
/// </summary>
public class DeathEffect : MonoBehaviour
{
    [Header("粒子")]
    public ParticleSystem deathParticles;

    [Header("生命周期")]
    public float lifetime = 1.5f;

    [Header("音效")]
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;

    void Start()
    {
        if (deathParticles == null)
            deathParticles = GetComponent<ParticleSystem>();

        deathParticles?.Play();

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position, soundVolume);

        Destroy(gameObject, lifetime);
    }
}
