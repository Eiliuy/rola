using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 剑士职业资产生成器
/// 一键生成剑士职业所需的所有 ScriptableObject 资产
/// </summary>
public class SwordsmanClassGenerator : EditorWindow
{
    [MenuItem("Rola/生成剑士职业资产")]
    static void OpenWindow()
    {
        GetWindow<SwordsmanClassGenerator>("剑士资产生成器");
    }

    void OnGUI()
    {
        GUILayout.Label("生成剑士职业资产", EditorStyles.boldLabel);
        GUILayout.Label("将自动创建职业、技能、装备、外观等 ScriptableObject 资产", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("生成剑士职业资产"))
        {
            Generate();
        }
    }

    void Generate()
    {
        string basePath = "Assets/ScriptableObjects";

        // 确保目录存在
        EnsureDirectory(basePath + "/Classes");
        EnsureDirectory(basePath + "/Skills");
        EnsureDirectory(basePath + "/Equipments");
        EnsureDirectory(basePath + "/Appearances/Parts/Body");
        EnsureDirectory(basePath + "/Appearances/Parts/Hair");
        EnsureDirectory(basePath + "/Appearances/Parts/Top");
        EnsureDirectory(basePath + "/Appearances/Parts/Bottom");
        EnsureDirectory(basePath + "/Appearances/Parts/Shoes");
        EnsureDirectory(basePath + "/Appearances/Parts/Gloves");
        EnsureDirectory(basePath + "/Appearances/Parts/Weapon");
        EnsureDirectory(basePath + "/Appearances/Parts/Accessory");

        // 创建部位资产
        CharacterPartData bodyPart = CreatePartAsset(basePath + "/Appearances/Parts/Body", "Part_Body_Swordsman", CharacterPartSlot.Body);
        CharacterPartData frontHairPart = CreatePartAsset(basePath + "/Appearances/Parts/Hair", "Part_Hair_Swordsman_Front", CharacterPartSlot.FrontHair);
        CharacterPartData backHairPart = CreatePartAsset(basePath + "/Appearances/Parts/Hair", "Part_Hair_Swordsman_Back", CharacterPartSlot.BackHair);
        CharacterPartData topPart = CreatePartAsset(basePath + "/Appearances/Parts/Top", "Part_Top_Swordsman", CharacterPartSlot.Top);
        CharacterPartData bottomPart = CreatePartAsset(basePath + "/Appearances/Parts/Bottom", "Part_Bottom_Swordsman", CharacterPartSlot.Bottom);
        CharacterPartData shoesPart = CreatePartAsset(basePath + "/Appearances/Parts/Shoes", "Part_Shoes_Swordsman", CharacterPartSlot.Shoes);
        CharacterPartData glovesPart = CreatePartAsset(basePath + "/Appearances/Parts/Gloves", "Part_Gloves_Swordsman", CharacterPartSlot.Gloves);
        CharacterPartData weaponPart = CreatePartAsset(basePath + "/Appearances/Parts/Weapon", "Part_Weapon_Longsword", CharacterPartSlot.Weapon);
        CharacterPartData accessoryPart = CreatePartAsset(basePath + "/Appearances/Parts/Accessory", "Part_Accessory_Empty", CharacterPartSlot.Accessory);

        // 创建外观资产
        CharacterAppearanceData appearance = CreateInstance<CharacterAppearanceData>();
        appearance.appearanceName = "剑士";
        appearance.description = "擅长近战斩击的均衡型职业";
        appearance.skinColor = new Color(1f, 0.9f, 0.8f);
        appearance.eyeColor = new Color(0.7f, 0.9f, 1f);
        appearance.hairColor = new Color(0.95f, 0.95f, 1f);
        appearance.backHair = backHairPart;
        appearance.frontHair = frontHairPart;
        appearance.top = topPart;
        appearance.bottom = bottomPart;
        appearance.gloves = glovesPart;
        appearance.shoes = shoesPart;
        appearance.weapon = weaponPart;
        appearance.accessory = accessoryPart;
        SaveAsset(appearance, basePath + "/Appearances/Appearance_Swordsman.asset");

        // 创建技能资产
        SkillData quickSlash = CreateInstance<SkillData>();
        quickSlash.skillName = "迅斩";
        quickSlash.description = "向前方快速突进并斩击，造成150%攻击力伤害";
        quickSlash.cooldown = 3f;
        quickSlash.mpCost = 0f;
        quickSlash.baseDamage = 15;
        quickSlash.damageMultiplier = 1.5f;
        quickSlash.elementType = ElementType.None;
        quickSlash.canCrit = true;
        quickSlash.rangeMultiplier = 1.2f;
        quickSlash.knockbackForce = 6f;
        quickSlash.knockbackUpForce = 2f;
        quickSlash.animationTrigger = "Skill";
        SaveAsset(quickSlash, basePath + "/Skills/Skill_QuickSlash.asset");

        // 创建装备资产
        EquipmentData trainingSword = CreateInstance<EquipmentData>();
        trainingSword.equipmentName = "训练用长剑";
        trainingSword.description = "新手剑士的标准装备";
        trainingSword.slot = EquipmentSlot.Weapon;
        trainingSword.modifiers.Add(new StatModifier(StatType.AttackPower, StatModType.FlatAdd, 2f, "训练用长剑"));
        SaveAsset(trainingSword, basePath + "/Equipments/Equipment_TrainingSword.asset");

        EquipmentData leatherArmor = CreateInstance<EquipmentData>();
        leatherArmor.equipmentName = "皮制护胸";
        leatherArmor.description = "轻便的皮甲，提供基础防护";
        leatherArmor.slot = EquipmentSlot.Armor;
        leatherArmor.modifiers.Add(new StatModifier(StatType.MaxHP, StatModType.FlatAdd, 20f, "皮制护胸"));
        SaveAsset(leatherArmor, basePath + "/Equipments/Equipment_LeatherArmor.asset");

        // 创建职业资产
        PlayerClassData swordsman = CreateInstance<PlayerClassData>();
        swordsman.className = "剑士";
        swordsman.description = "擅长近战斩击的均衡型职业，以速度和技巧见长";
        swordsman.baseMaxHP = 120;
        swordsman.baseAttackPower = 12;
        swordsman.baseMoveSpeed = 5f;
        swordsman.baseJumpForce = 12f;
        swordsman.baseAttackSpeed = 1f;
        swordsman.baseCritChance = 0.05f;
        swordsman.baseCritDamage = 1.5f;
        swordsman.baseCooldownReduction = 0f;
        swordsman.baseAttackRangeMultiplier = 1f;
        swordsman.startingSkills.Add(quickSlash);
        swordsman.startingEquipments.Add(trainingSword);
        swordsman.startingEquipments.Add(leatherArmor);
        swordsman.defaultAppearance = appearance;
        swordsman.classModifiers.Add(new StatModifier(StatType.AttackPower, StatModType.FlatAdd, 2f, "剑士特性"));
        swordsman.classModifiers.Add(new StatModifier(StatType.AttackSpeed, StatModType.FlatAdd, 0.1f, "剑士特性"));
        SaveAsset(swordsman, basePath + "/Classes/Class_Swordsman.asset");

        AssetDatabase.Refresh();
        Debug.Log("[剑士资产生成器] 剑士职业资产生成完成");
    }

    void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    CharacterPartData CreatePartAsset(string folder, string name, CharacterPartSlot slot)
    {
        CharacterPartData part = CreateInstance<CharacterPartData>();
        part.partName = name;
        part.slot = slot;
        string path = folder + "/" + name + ".asset";
        SaveAsset(part, path);
        return part;
    }

    void SaveAsset(Object asset, string path)
    {
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        AssetDatabase.CreateAsset(asset, path);
        EditorUtility.SetDirty(asset);
    }
}
