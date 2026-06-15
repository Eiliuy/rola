using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 通用血条：可绑定 PlayerStats 或 EnemyController
/// </summary>
[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;
    public Image fillImage;
    public Gradient colorGradient;

    [Header("目标")]
    public PlayerStats playerStats;
    public EnemyController enemyController;

    [Header("世界空间血条")]
    public bool isWorldSpace;
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.2f, 0);

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (fillImage == null && slider != null)
            fillImage = slider.fillRect.GetComponent<Image>();

        Subscribe();
        UpdateHealthBar();
    }

    void LateUpdate()
    {
        if (isWorldSpace && target != null)
            transform.position = target.position + offset;
    }

    void Subscribe()
    {
        if (playerStats != null)
            playerStats.OnHPChanged += OnHPChanged;

        if (enemyController != null)
            enemyController.OnHPChanged += OnHPChanged;
    }

    void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnHPChanged -= OnHPChanged;

        if (enemyController != null)
            enemyController.OnHPChanged -= OnHPChanged;
    }

    void OnHPChanged(int current, int max)
    {
        UpdateHealthBar(current, max);
    }

    void UpdateHealthBar(int current, int max)
    {
        if (slider == null) return;

        slider.maxValue = Mathf.Max(1, max);
        slider.value = Mathf.Clamp(current, 0, max);

        if (fillImage != null && colorGradient != null && colorGradient.colorKeys.Length > 0)
            fillImage.color = colorGradient.Evaluate(slider.normalizedValue);
    }

    void UpdateHealthBar()
    {
        if (playerStats != null)
            UpdateHealthBar(playerStats.currentHP, playerStats.maxHP);
        else if (enemyController != null)
            UpdateHealthBar(enemyController.currentHP, enemyController.maxHP);
    }
}
