using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换触发器
/// 玩家进入后切换场景，可配置按键触发或自动触发
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SceneTransition : MonoBehaviour
{
    [Header("目标")]
    public string targetSceneName;
    public string spawnPointName;

    [Header("触发条件")]
    public bool requireInput = true;
    public KeyCode inputKey = KeyCode.E;

    [Header("特效")]
    public ParticleSystem transitionEffect;
    public AudioClip transitionSound;

    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (!requireInput)
            Transition();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void Update()
    {
        if (requireInput && playerInRange && Input.GetKeyDown(inputKey))
            Transition();
    }

    void Transition()
    {
        if (transitionEffect != null)
            Instantiate(transitionEffect, transform.position, Quaternion.identity);

        if (transitionSound != null)
            AudioSource.PlayClipAtPoint(transitionSound, transform.position);

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            PlayerSpawnManager.SetSpawnPoint(spawnPointName);
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("SceneTransition: targetSceneName 为空");
        }
    }
}
