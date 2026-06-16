using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色外观渲染器
/// 根据 CharacterAppearanceData 组装多层 SpriteRenderer
/// </summary>
public class CharacterAppearanceRenderer : MonoBehaviour
{
    [Header("部位挂载点")]
    public Transform bodyHolder;
    public Transform backHairHolder;
    public Transform bottomHolder;
    public Transform topHolder;
    public Transform glovesHolder;
    public Transform shoesHolder;
    public Transform weaponHolder;
    public Transform frontHairHolder;
    public Transform accessoryHolder;

    private Dictionary<CharacterPartSlot, Transform> slotHolders;
    private Dictionary<CharacterPartSlot, SpriteRenderer> slotRenderers;

    private CharacterAppearanceData currentAppearance;
    private Dictionary<CharacterPartSlot, Color> baseColors = new Dictionary<CharacterPartSlot, Color>();

    [Header("基础排序")]
    public int baseSortingOrder = 0;
    public string sortingLayerName = "Default";

    void Awake()
    {
        InitializeSlotDictionary();
        EnsureRenderers();
    }

    void InitializeSlotDictionary()
    {
        slotHolders = new Dictionary<CharacterPartSlot, Transform>
        {
            { CharacterPartSlot.Body, bodyHolder },
            { CharacterPartSlot.BackHair, backHairHolder },
            { CharacterPartSlot.Bottom, bottomHolder },
            { CharacterPartSlot.Top, topHolder },
            { CharacterPartSlot.Gloves, glovesHolder },
            { CharacterPartSlot.Shoes, shoesHolder },
            { CharacterPartSlot.Weapon, weaponHolder },
            { CharacterPartSlot.FrontHair, frontHairHolder },
            { CharacterPartSlot.Accessory, accessoryHolder }
        };
    }

    void EnsureRenderers()
    {
        slotRenderers = new Dictionary<CharacterPartSlot, SpriteRenderer>();

        foreach (var pair in slotHolders)
        {
            if (pair.Value == null) continue;

            SpriteRenderer sr = pair.Value.GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = pair.Value.gameObject.AddComponent<SpriteRenderer>();

            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = baseSortingOrder + GetDefaultSortingOrder(pair.Key);
            slotRenderers[pair.Key] = sr;
        }
    }

    /// <summary>
    /// 应用外观数据
    /// </summary>
    public void ApplyAppearance(CharacterAppearanceData data)
    {
        if (data == null) return;
        currentAppearance = data;
        baseColors.Clear();

        // 身体
        ApplyPart(CharacterPartSlot.Body, data.bodySprite, data.skinColor, null);

        // 各部位
        ApplyPart(CharacterPartSlot.BackHair, data.backHair, data.hairColor);
        ApplyPart(CharacterPartSlot.Bottom, data.bottom, Color.white);
        ApplyPart(CharacterPartSlot.Top, data.top, Color.white);
        ApplyPart(CharacterPartSlot.Gloves, data.gloves, Color.white);
        ApplyPart(CharacterPartSlot.Shoes, data.shoes, Color.white);
        ApplyPart(CharacterPartSlot.Weapon, data.weapon, Color.white);
        ApplyPart(CharacterPartSlot.FrontHair, data.frontHair, data.hairColor);
        ApplyPart(CharacterPartSlot.Accessory, data.accessory, Color.white);
    }

    /// <summary>
    /// 应用单个部位配置
    /// </summary>
    void ApplyPart(CharacterPartSlot slot, CharacterPartData partData, Color defaultColor)
    {
        if (!slotRenderers.TryGetValue(slot, out SpriteRenderer sr) || sr == null) return;

        Color baseColor;
        if (partData == null)
        {
            sr.sprite = null;
            baseColor = defaultColor;
            sr.color = baseColor;
            baseColors[slot] = baseColor;
            return;
        }

        baseColor = partData.colorTint * defaultColor;
        baseColors[slot] = baseColor;

        sr.sprite = partData.sprite;
        sr.color = baseColor;
        sr.sortingOrder = baseSortingOrder + GetDefaultSortingOrder(slot) + partData.sortingOrderOffset;

        if (slotHolders.TryGetValue(slot, out Transform holder) && holder != null)
        {
            holder.localPosition = partData.positionOffset;
            holder.localRotation = Quaternion.Euler(0, 0, partData.rotationOffset);
            holder.localScale = new Vector3(partData.scaleOverride.x, partData.scaleOverride.y, 1f);
        }
    }

    /// <summary>
    /// 直接设置某个部位的 Sprite 和颜色
    /// </summary>
    void ApplyPart(CharacterPartSlot slot, Sprite sprite, Color color, CharacterPartData partData)
    {
        if (!slotRenderers.TryGetValue(slot, out SpriteRenderer sr) || sr == null) return;

        baseColors[slot] = color;
        sr.sprite = sprite;
        sr.color = color;

        if (partData != null)
        {
            sr.sortingOrder = baseSortingOrder + GetDefaultSortingOrder(slot) + partData.sortingOrderOffset;
            if (slotHolders.TryGetValue(slot, out Transform holder) && holder != null)
            {
                holder.localPosition = partData.positionOffset;
                holder.localRotation = Quaternion.Euler(0, 0, partData.rotationOffset);
                holder.localScale = new Vector3(partData.scaleOverride.x, partData.scaleOverride.y, 1f);
            }
        }
    }

    /// <summary>
    /// 获取部位默认排序
    /// </summary>
    int GetDefaultSortingOrder(CharacterPartSlot slot)
    {
        switch (slot)
        {
            case CharacterPartSlot.BackHair: return 0;
            case CharacterPartSlot.Body: return 1;
            case CharacterPartSlot.Bottom: return 2;
            case CharacterPartSlot.Top: return 3;
            case CharacterPartSlot.Shoes: return 4;
            case CharacterPartSlot.Gloves: return 5;
            case CharacterPartSlot.Weapon: return 6;
            case CharacterPartSlot.FrontHair: return 7;
            case CharacterPartSlot.Accessory: return 8;
            default: return 1;
        }
    }

    /// <summary>
    /// 统一设置所有 SpriteRenderer 的颜色
    /// </summary>
    public void SetTint(Color color)
    {
        foreach (var pair in slotRenderers)
        {
            if (pair.Value != null)
                pair.Value.color = color;
        }
    }

    /// <summary>
    /// 重置为各部位基础颜色
    /// </summary>
    public void ResetTint()
    {
        foreach (var pair in slotRenderers)
        {
            if (pair.Value == null) continue;
            if (baseColors.TryGetValue(pair.Key, out Color baseColor))
                pair.Value.color = baseColor;
        }
    }

    /// <summary>
    /// 获取所有 SpriteRenderer
    /// </summary>
    public IEnumerable<SpriteRenderer> GetAllRenderers()
    {
        return slotRenderers.Values;
    }

    /// <summary>
    /// 清除所有部位
    /// </summary>
    public void Clear()
    {
        foreach (var sr in slotRenderers.Values)
        {
            if (sr != null)
                sr.sprite = null;
        }
    }
}
