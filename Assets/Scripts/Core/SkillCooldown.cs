using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能冷却管理器
/// </summary>
public class SkillCooldown
{
    private Dictionary<SkillData, float> cooldownTimers = new Dictionary<SkillData, float>();

    /// <summary>
    /// 更新所有冷却
    /// </summary>
    public void Update(float deltaTime)
    {
        List<SkillData> keys = new List<SkillData>(cooldownTimers.Keys);
        foreach (var skill in keys)
        {
            cooldownTimers[skill] -= deltaTime;
            if (cooldownTimers[skill] <= 0f)
                cooldownTimers.Remove(skill);
        }
    }

    /// <summary>
    /// 开始冷却
    /// </summary>
    public void StartCooldown(SkillData skill, float cooldown)
    {
        if (skill == null) return;
        cooldownTimers[skill] = Mathf.Max(0f, cooldown);
    }

    /// <summary>
    /// 是否可用
    /// </summary>
    public bool IsReady(SkillData skill)
    {
        if (skill == null) return false;
        return !cooldownTimers.ContainsKey(skill);
    }

    /// <summary>
    /// 获取剩余冷却时间
    /// </summary>
    public float GetRemainingCooldown(SkillData skill)
    {
        if (skill == null) return 0f;
        return cooldownTimers.ContainsKey(skill) ? cooldownTimers[skill] : 0f;
    }

    /// <summary>
    /// 重置所有冷却
    /// </summary>
    public void ResetAll()
    {
        cooldownTimers.Clear();
    }
}
