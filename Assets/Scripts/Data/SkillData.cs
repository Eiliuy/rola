using UnityEngine;

/// <summary>
/// 元素类型枚举
/// </summary>
public enum ElementType
{
    None,
    Fire,
    Ice,
    Thunder,
    Poison
}

/// <summary>
/// 技能数据资产
/// </summary>
[CreateAssetMenu(fileName = "Skill_", menuName = "Rola/Skill Data", order = 2)]
public class SkillData : ScriptableObject
{
    [Header("基础信息")]
    public string skillName = "技能";
    public string description = "";
    public Sprite icon;

    [Header("冷却与消耗")]
    public float cooldown = 1f;
    public float mpCost = 0f;

    [Header("伤害")]
    public int baseDamage = 10;
    public float damageMultiplier = 1f;
    public ElementType elementType = ElementType.None;
    public bool canCrit = true;

    [Header("范围与效果")]
    public float rangeMultiplier = 1f;
    public float knockbackForce = 5f;
    public float knockbackUpForce = 2f;

    [Header("动画与特效")]
    public string animationTrigger = "Skill";
    public GameObject effectPrefab;
    public AudioClip castSound;

    [Header("升级分支")]
    public SkillData[] upgradeBranches = new SkillData[0];
}
