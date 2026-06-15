using UnityEngine;

/// <summary>
/// 简单音效管理器单例
/// 管理玩家和敌人的攻击、受伤、死亡等音效
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("玩家音效")]
    public AudioClip[] attackSounds;
    public AudioClip hurtSound;
    public AudioClip dashSound;
    public AudioClip jumpSound;

    [Header("敌人音效")]
    public AudioClip enemyAttackSound;
    public AudioClip enemyHurtSound;
    public AudioClip enemyDeathSound;

    [Header("通用")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySFX(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume * volumeScale);
    }

    /// <summary>
    /// 根据连招段数播放玩家攻击音效
    /// </summary>
    public void PlayAttackSound(int comboIndex, Vector3 position)
    {
        if (attackSounds == null || attackSounds.Length == 0) return;

        int index = Mathf.Clamp(comboIndex, 0, attackSounds.Length - 1);
        PlaySFX(attackSounds[index], position);
    }

    public void PlayHurtSound(Vector3 position) => PlaySFX(hurtSound, position);
    public void PlayDashSound(Vector3 position) => PlaySFX(dashSound, position);
    public void PlayJumpSound(Vector3 position) => PlaySFX(jumpSound, position);
    public void PlayEnemyAttackSound(Vector3 position) => PlaySFX(enemyAttackSound, position);
    public void PlayEnemyHurtSound(Vector3 position) => PlaySFX(enemyHurtSound, position);
    public void PlayEnemyDeathSound(Vector3 position) => PlaySFX(enemyDeathSound, position);
}
