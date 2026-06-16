using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 升级奖励管理器
/// 负责生成升级选项池并随机抽取奖励
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("升级选项配置")]
    public List<UpgradeData> allUpgrades = new List<UpgradeData>();
    public int optionCount = 3;

    public System.Action<List<UpgradeData>> OnUpgradeTriggered;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (RunManager.Instance != null)
            RunManager.Instance.OnLevelUp += TriggerUpgrade;
    }

    void OnDestroy()
    {
        if (RunManager.Instance != null)
            RunManager.Instance.OnLevelUp -= TriggerUpgrade;
    }

    /// <summary>
    /// 触发升级，生成选项
    /// </summary>
    public void TriggerUpgrade()
    {
        List<UpgradeData> options = GenerateOptions();
        OnUpgradeTriggered?.Invoke(options);
    }

    /// <summary>
    /// 生成不重复的升级选项
    /// </summary>
    public List<UpgradeData> GenerateOptions()
    {
        List<UpgradeData> pool = new List<UpgradeData>();
        foreach (var upgrade in allUpgrades)
        {
            if (upgrade != null)
                pool.Add(upgrade);
        }

        List<UpgradeData> result = new List<UpgradeData>();
        int count = Mathf.Min(optionCount, pool.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    /// <summary>
    /// 选择并应用升级
    /// </summary>
    public void SelectUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null) return;
        if (RunManager.Instance?.CurrentRun?.playerBuild != null)
            RunManager.Instance.CurrentRun.playerBuild.AddUpgrade(upgrade);
    }
}
