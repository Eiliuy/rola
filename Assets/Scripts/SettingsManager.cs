using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置管理器：音量、全屏等
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("音量")]
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;

    [Header("显示")]
    public Toggle fullscreenToggle;

    [Header("面板")]
    public GameObject settingsPanel;

    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadSettings();
        BindUI();
    }

    void BindUI()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
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

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.sfxVolume = value;

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
    }

    public void SetMusicVolume(float value)
    {
        // TODO: 接入背景音乐播放器
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, value ? 1 : 0);
    }

    void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f);
        float music = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;

        AudioListener.volume = master;

        if (AudioManager.Instance != null)
            AudioManager.Instance.sfxVolume = sfx;

        Screen.fullScreen = fullscreen;

        if (masterVolumeSlider != null) masterVolumeSlider.value = master;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfx;
        if (musicVolumeSlider != null) musicVolumeSlider.value = music;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
    }
}
