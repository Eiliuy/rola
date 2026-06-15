using UnityEngine;

/// <summary>
/// 旁白数据资产
/// 可在 Project 窗口右键 Create → Rola → Narration 创建
/// </summary>
[CreateAssetMenu(fileName = "Narration", menuName = "Rola/Narration")]
public class NarrationData : ScriptableObject
{
    [Tooltip("唯一标识")]
    public string id;

    [Tooltip("旁白文本"), TextArea(3, 6)]
    public string text = "……";

    [Tooltip("显示持续时间（打字完成后停留多久）")]
    public float duration = 3f;

    [Tooltip("配音")]
    public AudioClip voice;

    [Tooltip("打字间隔（秒）")]
    public float typingSpeed = 0.05f;

    [Tooltip("是否可重复触发")]
    public bool canRepeat = false;
}
