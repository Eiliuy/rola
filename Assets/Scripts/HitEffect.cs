using UnityEngine;

/// <summary>
/// 受击特效：播放粒子并在指定时间后销毁
/// </summary>
public class HitEffect : MonoBehaviour
{
    [Header("粒子")]
    public ParticleSystem hitParticles;

    [Header("生命周期")]
    public float lifetime = 0.5f;

    [Header("朝向")]
    public bool randomRotation = true;

    void Start()
    {
        if (hitParticles == null)
            hitParticles = GetComponent<ParticleSystem>();

        if (randomRotation)
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        hitParticles?.Play();
        Destroy(gameObject, lifetime);
    }
}
