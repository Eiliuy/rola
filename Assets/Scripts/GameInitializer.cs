using UnityEngine;

/// <summary>
/// 游戏初始化器：确保必要的单例管理器存在
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("管理器预制体")]
    public GameObject hitStopManagerPrefab;
    public GameObject audioManagerPrefab;
    public GameObject narrationManagerPrefab;
    public GameObject settingsManagerPrefab;
    public GameObject runManagerPrefab;
    public GameObject upgradeManagerPrefab;

    void Awake()
    {
        EnsureHitStopManager();
        EnsureAudioManager();
        EnsureNarrationManager();
        EnsureSettingsManager();
        EnsureRunManager();
        EnsureUpgradeManager();
    }

    void EnsureHitStopManager()
    {
        if (HitStopManager.Instance == null)
        {
            if (hitStopManagerPrefab != null)
            {
                Instantiate(hitStopManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("HitStopManager");
                go.AddComponent<HitStopManager>();
            }
        }
    }

    void EnsureAudioManager()
    {
        if (AudioManager.Instance == null)
        {
            if (audioManagerPrefab != null)
            {
                Instantiate(audioManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("AudioManager");
                go.AddComponent<AudioManager>();
            }
        }
    }

    void EnsureNarrationManager()
    {
        if (NarrationManager.Instance == null)
        {
            if (narrationManagerPrefab != null)
            {
                Instantiate(narrationManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("NarrationManager");
                go.AddComponent<NarrationManager>();
            }
        }
    }

    void EnsureSettingsManager()
    {
        if (SettingsManager.Instance == null)
        {
            if (settingsManagerPrefab != null)
            {
                Instantiate(settingsManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("SettingsManager");
                go.AddComponent<SettingsManager>();
            }
        }
    }

    void EnsureRunManager()
    {
        if (RunManager.Instance == null)
        {
            if (runManagerPrefab != null)
            {
                Instantiate(runManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("RunManager");
                go.AddComponent<RunManager>();
            }
        }
    }

    void EnsureUpgradeManager()
    {
        if (UpgradeManager.Instance == null)
        {
            if (upgradeManagerPrefab != null)
            {
                Instantiate(upgradeManagerPrefab);
            }
            else
            {
                GameObject go = new GameObject("UpgradeManager");
                go.AddComponent<UpgradeManager>();
            }
        }
    }
}
