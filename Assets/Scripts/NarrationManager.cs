using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 旁白管理器
/// 负责队列播放、打字机效果、字幕显示
/// </summary>
public class NarrationManager : MonoBehaviour
{
    public static NarrationManager Instance;

    [Header("UI")]
    public Text narrationText;
    public GameObject narrationPanel;

    [Header("默认打字速度")]
    public float defaultTypingSpeed = 0.05f;

    [Header("配音")]
    public AudioSource voiceSource;

    private Queue<NarrationData> queue = new Queue<NarrationData>();
    private bool isPlaying = false;
    private Coroutine currentCoroutine;

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
        if (narrationPanel != null)
            narrationPanel.SetActive(false);
    }

    /// <summary>
    /// 播放一条旁白，如果正在播放则加入队列
    /// </summary>
    public void PlayNarration(NarrationData data)
    {
        if (data == null || string.IsNullOrEmpty(data.text)) return;

        queue.Enqueue(data);

        if (!isPlaying)
            currentCoroutine = StartCoroutine(PlayQueue());
    }

    /// <summary>
    /// 清空队列并停止当前旁白
    /// </summary>
    public void StopNarration()
    {
        queue.Clear();
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        isPlaying = false;

        if (narrationPanel != null)
            narrationPanel.SetActive(false);
    }

    IEnumerator PlayQueue()
    {
        isPlaying = true;

        while (queue.Count > 0)
        {
            NarrationData data = queue.Dequeue();
            yield return StartCoroutine(ShowNarration(data));
            yield return new WaitForSeconds(0.5f);
        }

        isPlaying = false;
        currentCoroutine = null;
    }

    IEnumerator ShowNarration(NarrationData data)
    {
        if (narrationPanel != null)
            narrationPanel.SetActive(true);

        if (narrationText != null)
            narrationText.text = "";

        if (voiceSource != null && data.voice != null)
            voiceSource.PlayOneShot(data.voice);

        float speed = data.typingSpeed > 0 ? data.typingSpeed : defaultTypingSpeed;

        foreach (char c in data.text)
        {
            if (narrationText != null)
                narrationText.text += c;
            yield return new WaitForSeconds(speed);
        }

        yield return new WaitForSeconds(data.duration);

        if (narrationPanel != null)
            narrationPanel.SetActive(false);
    }
}
