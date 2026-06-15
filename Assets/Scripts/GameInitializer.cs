using UnityEngine;

/// <summary>
/// 游戏初始化器：确保必要的单例管理器存在
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("管理器预制体")]
    public GameObject hitStopManagerPrefab;

    void Awake()
    {
        EnsureHitStopManager();
        EnsureAudioManager();
    }

    void EnsureHitStopManager()
    {
        if (HitStopManager.Instance == null)
        {
            GameObject go = new GameObject("HitStopManager");
            go.AddComponent<HitStopManager>();
        }
    }

    void EnsureAudioManager()
    {
        if (AudioManager.Instance == null)
        {
            GameObject go = new GameObject("AudioManager");
            go.AddComponent<AudioManager>();
        }
    }
}
