using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 占位 Sprite 生成器
/// 自动生成简单彩色方块作为各部位的占位美术资源
/// </summary>
public class PlaceholderSpriteGenerator : EditorWindow
{
    private int textureSize = 64;

    [MenuItem("Rola/生成占位 Sprite")]
    static void OpenWindow()
    {
        GetWindow<PlaceholderSpriteGenerator>("占位 Sprite 生成器");
    }

    void OnGUI()
    {
        GUILayout.Label("生成占位 Sprite", EditorStyles.boldLabel);
        textureSize = EditorGUILayout.IntField("贴图尺寸", textureSize);

        if (GUILayout.Button("生成全部占位 Sprite"))
        {
            GenerateAll();
        }
    }

    void GenerateAll()
    {
        GenerateBody();
        GenerateHair();
        GenerateTop();
        GenerateBottom();
        GenerateShoes();
        GenerateGloves();
        GenerateWeapon();
        GenerateAccessory();

        AssetDatabase.Refresh();
        Debug.Log("[占位 Sprite 生成器] 全部生成完成");
    }

    Sprite Generate(string path, Color color)
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
        texture.SetPixels(pixels);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string fullPath = Path.Combine(Application.dataPath, "../" + path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        File.WriteAllBytes(fullPath, bytes);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    void GenerateBody()
    {
        Sprite sprite = Generate("Assets/Resources/Sprites/Placeholders/Body/Body_Swordsman.png", new Color(1f, 0.9f, 0.8f));
        Debug.Log($"[占位 Sprite] 身体: {sprite?.name}");
    }

    void GenerateHair()
    {
        Sprite sprite = Generate("Assets/Resources/Sprites/Placeholders/Hair/Hair_Swordsman_Front.png", new Color(0.95f, 0.95f, 1f));
        Debug.Log($"[占位 Sprite] 前发: {sprite?.name}");
    }

    void GenerateTop()
    {
        Sprite sprite = Generate("Assets/Resources/Sprites/Placeholders/Top/Top_Swordsman.png", new Color(0.15f, 0.15f, 0.18f));
        Debug.Log($"[占位 Sprite] 上衣: {sprite?.name}");
    }

    void GenerateBottom()
    {
        Sprite sprite = Generate("Assets/Resources/Sprites/Placeholders/Bottom/Bottom_Swordsman.png", new Color(0.12f, 0.12f, 0.15f));
        Debug.Log($"[占位 Sprite] 下装: {sprite?.name}");
    }

    void GenerateShoes()
    {
        Sprite sprite = Generate("Assets/Resources/Sprites/Placeholders/Shoes/Shoes_Swordsman.png", new Color(0.1f, 0.08f, 0.08f));
        Debug.Log($"[占位 Sprite] 鞋子: {sprite?.name}");
    }

    void GenerateGloves()
    {
        Sprite sprite = Generate("Assets/Resources/Sprites/Placeholders/Gloves/Gloves_Swordsman.png", new Color(0.18f, 0.18f, 0.2f));
        Debug.Log($"[占位 Sprite] 手套: {sprite?.name}");
    }

    void GenerateWeapon()
    {
        Sprite sprite = Generate("Assets/Resources/Sprites/Placeholders/Weapon/Weapon_Longsword.png", new Color(0.7f, 0.85f, 1f));
        Debug.Log($"[占位 Sprite] 武器: {sprite?.name}");
    }

    void GenerateAccessory()
    {
        Sprite sprite = Generate("Assets/Resources/Sprites/Placeholders/Accessory/Accessory_Empty.png", Color.clear);
        Debug.Log($"[占位 Sprite] 配饰（空）: {sprite?.name}");
    }
}
