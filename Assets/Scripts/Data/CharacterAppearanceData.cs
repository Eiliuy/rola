using UnityEngine;

/// <summary>
/// 角色整体外观数据资产
/// 汇总所有部位配置，供运行时组装角色形象
/// </summary>
[CreateAssetMenu(fileName = "Appearance_", menuName = "Rola/Character Appearance Data", order = 11)]
public class CharacterAppearanceData : ScriptableObject
{
    [Header("基础信息")]
    public string appearanceName = "默认外观";
    public string description = "";

    [Header("基础形象")]
    [Tooltip("身体基础 Sprite")]
    public Sprite bodySprite;

    [Tooltip("肤色")]
    public Color skinColor = new Color(1f, 0.9f, 0.8f);

    [Tooltip("眼睛颜色")]
    public Color eyeColor = new Color(0.7f, 0.9f, 1f);

    [Tooltip("发色")]
    public Color hairColor = Color.white;

    [Header("部位")]
    public CharacterPartData backHair;
    public CharacterPartData frontHair;
    public CharacterPartData top;
    public CharacterPartData bottom;
    public CharacterPartData gloves;
    public CharacterPartData shoes;
    public CharacterPartData weapon;
    public CharacterPartData accessory;

    [Header("动画")]
    [Tooltip("外观专属动画控制器，为空则使用默认")]
    public RuntimeAnimatorController animatorController;
}
