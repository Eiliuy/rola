using UnityEngine;

/// <summary>
/// 单局肉鸽流程管理器
/// 负责维护当前 Run 数据：职业、Build、关卡进度、资源
/// </summary>
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    public RunData CurrentRun { get; private set; } = new RunData();

    public System.Action<int> OnGoldChanged;
    public System.Action<int, int> OnExpChanged;
    public System.Action OnLevelUp;

    [Header("默认职业")]
    public PlayerClassData defaultClass;

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

    /// <summary>
    /// 开始新一局游戏
    /// </summary>
    public void StartNewRun(PlayerClassData classData)
    {
        if (classData == null)
            classData = defaultClass;

        CurrentRun.Reset();
        CurrentRun.selectedClass = classData;
        CurrentRun.playerBuild = new PlayerBuild();
        CurrentRun.playerBuild.Initialize(classData);
        CurrentRun.isRunActive = true;
    }

    /// <summary>
    /// 结束当前 Run
    /// </summary>
    public void EndRun()
    {
        CurrentRun.isRunActive = false;
    }

    /// <summary>
    /// 增加金币
    /// </summary>
    public void AddGold(int amount)
    {
        CurrentRun.gold += Mathf.Max(0, amount);
        OnGoldChanged?.Invoke(CurrentRun.gold);
    }

    /// <summary>
    /// 消耗金币
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (CurrentRun.gold < amount) return false;
        CurrentRun.gold -= amount;
        OnGoldChanged?.Invoke(CurrentRun.gold);
        return true;
    }

    /// <summary>
    /// 增加经验
    /// </summary>
    public void AddExp(int amount)
    {
        CurrentRun.exp += Mathf.Max(0, amount);
        OnExpChanged?.Invoke(CurrentRun.exp, CurrentRun.expToNextLevel);

        while (CurrentRun.exp >= CurrentRun.expToNextLevel)
        {
            CurrentRun.exp -= CurrentRun.expToNextLevel;
            LevelUp();
        }
    }

    /// <summary>
    /// 升级
    /// </summary>
    void LevelUp()
    {
        CurrentRun.expToNextLevel = Mathf.RoundToInt(CurrentRun.expToNextLevel * 1.2f);
        OnLevelUp?.Invoke();
    }

    /// <summary>
    /// 推进房间
    /// </summary>
    public void AdvanceRoom()
    {
        CurrentRun.currentRoom++;
    }

    /// <summary>
    /// 推进关卡
    /// </summary>
    public void AdvanceLevel()
    {
        CurrentRun.currentLevel++;
        CurrentRun.currentRoom = 1;
    }
}
