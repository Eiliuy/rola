using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单逻辑
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("关卡")]
    public string firstLevelName = "SampleScene";

    [Header("设置面板")]
    public GameObject settingsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
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
