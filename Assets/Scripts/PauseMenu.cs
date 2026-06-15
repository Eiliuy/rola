using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 暂停菜单逻辑
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("面板")]
    public GameObject pausePanel;

    [Header("设置")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        SetPause(isPaused);
    }

    void SetPause(bool pause)
    {
        isPaused = pause;
        if (pausePanel != null)
            pausePanel.SetActive(pause);
        Time.timeScale = pause ? 0f : 1f;
    }

    public void Resume()
    {
        SetPause(false);
    }

    public void Restart()
    {
        SetPause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SetPause(false);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        SetPause(false);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
