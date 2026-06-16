using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单局肉鸽运行数据
/// </summary>
[System.Serializable]
public class RunData
{
    public PlayerClassData selectedClass;
    public PlayerBuild playerBuild;

    public int currentLevel = 1;
    public int currentRoom = 1;

    public int gold = 0;
    public int exp = 0;
    public int expToNextLevel = 100;

    public bool isRunActive = false;

    public void Reset()
    {
        selectedClass = null;
        playerBuild = null;
        currentLevel = 1;
        currentRoom = 1;
        gold = 0;
        exp = 0;
        expToNextLevel = 100;
        isRunActive = false;
    }
}
