using UnityEngine;

/// <summary>
/// 角色单个部位数据资产
/// 可配置部位名称、类型、Sprite、颜色、排序等
/// </summary>
[CreateAssetMenu(fileName = "Part_", menuName = "Rola/Character Part Data", order = 10)]
public class CharacterPartData : ScriptableObject
{
    [Header("基础信息")]
    public string partName = "部位";

    [Tooltip("部位类型")]
    public CharacterPartSlot slot = CharacterPartSlot.Top;

    [Tooltip("部位 Sprite")]
    public Sprite sprite;

    [Tooltip("部位颜色覆盖，白色为不覆盖")]
    public Color colorTint = Color.white;

    [Tooltip("排序偏移，数值越大越靠前")]
    public int sortingOrderOffset = 0;

    [Tooltip("局部位置偏移")]
    public Vector2 positionOffset = Vector2.zero;

    [Tooltip("局部旋转偏移")]
    public float rotationOffset = 0f;

    [Tooltip("局部缩放")]
    public Vector2 scaleOverride = Vector2.one;
}
