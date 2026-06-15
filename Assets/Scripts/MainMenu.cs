using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单逻辑
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("关卡")]
    public string firstLevelName = "SampleScene";

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void OpenSettings()
    {
        SettingsManager.Instance?.OpenSettings();
    }

    public void CloseSettings()
    {
        SettingsManager.Instance?.CloseSettings();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
