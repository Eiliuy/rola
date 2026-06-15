using UnityEngine;

/// <summary>
/// 玩家出生点管理器
/// 场景加载后将玩家移动到指定出生点
/// </summary>
public class PlayerSpawnManager : MonoBehaviour
{
    public static string nextSpawnPoint;

    [Header("出生点")]
    public Transform defaultSpawnPoint;
    public Transform[] spawnPoints;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Transform target = GetSpawnPoint(nextSpawnPoint);
        if (target != null)
        {
            player.transform.position = target.position;

            // 同步更新玩家重生点
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
                stats.SetRespawnPoint(target.position);
        }

        // 清空，避免后续场景被错误设置
        nextSpawnPoint = null;
    }

    Transform GetSpawnPoint(string name)
    {
        if (string.IsNullOrEmpty(name)) return defaultSpawnPoint;

        foreach (var point in spawnPoints)
        {
            if (point != null && point.name == name)
                return point;
        }

        Debug.LogWarning($"PlayerSpawnManager: 找不到出生点 {name}，使用默认出生点");
        return defaultSpawnPoint;
    }

    public static void SetSpawnPoint(string name)
    {
        nextSpawnPoint = name;
    }
}
