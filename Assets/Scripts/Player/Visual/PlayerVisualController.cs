using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家视觉总控
/// 连接外观数据、渲染器、动画控制器与玩家状态事件
/// </summary>
public class PlayerVisualController : MonoBehaviour
{
    [Header("外观")]
    public CharacterAppearanceRenderer appearanceRenderer;
    public PlayerAnimationController animationController;

    [Header("受击反馈")]
    public Color hurtFlashColor = Color.red;
    public float hurtFlashDuration = 0.1f;

    [Header("动画")]
    public RuntimeAnimatorController defaultAnimatorController;

    private PlayerStats playerStats;
    private PlayerBuild currentBuild;

    void Start()
    {
        if (appearanceRenderer == null)
            appearanceRenderer = GetComponentInChildren<CharacterAppearanceRenderer>();
        if (animationController == null)
            animationController = GetComponentInChildren<PlayerAnimationController>();

        if (appearanceRenderer == null)
            Debug.LogError("[PlayerVisualController] 未找到 CharacterAppearanceRenderer，外观无法渲染。", this);
        if (animationController == null)
            Debug.LogError("[PlayerVisualController] 未找到 PlayerAnimationController，动画无法播放。", this);

        playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.OnHPChanged += OnHPChanged;
            playerStats.OnPlayerHurt += OnPlayerHurt;
            playerStats.OnPlayerRespawned += OnPlayerRespawned;
        }

        ApplyCurrentAppearance();
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnHPChanged -= OnHPChanged;
            playerStats.OnPlayerHurt -= OnPlayerHurt;
            playerStats.OnPlayerRespawned -= OnPlayerRespawned;
        }

        if (currentBuild != null)
        {
            currentBuild.OnStatsChanged -= OnBuildStatsChanged;
            currentBuild.OnAppearanceChanged -= OnAppearanceChanged;
        }
    }

    /// <summary>
    /// 应用当前 Build 中的外观数据
    /// </summary>
    public void ApplyCurrentAppearance()
    {
        PlayerBuild build = RunManager.Instance?.CurrentRun?.playerBuild;
        if (build == null) return;

        if (currentBuild != build)
        {
            if (currentBuild != null)
            {
                currentBuild.OnStatsChanged -= OnBuildStatsChanged;
                currentBuild.OnAppearanceChanged -= OnAppearanceChanged;
            }

            currentBuild = build;
            currentBuild.OnStatsChanged += OnBuildStatsChanged;
            currentBuild.OnAppearanceChanged += OnAppearanceChanged;
        }

        ApplyAppearance(build.appearanceData);
    }

    /// <summary>
    /// 应用指定外观数据
    /// </summary>
    public void ApplyAppearance(CharacterAppearanceData data)
    {
        if (data == null) return;

        appearanceRenderer?.ApplyAppearance(data);

        if (animationController != null)
        {
            if (data.animatorController != null)
                animationController.SetAnimatorController(data.animatorController);
            else if (defaultAnimatorController != null)
                animationController.SetAnimatorController(defaultAnimatorController);
        }
    }

    void OnBuildStatsChanged()
    {
        // 属性变化时外观一般不变，但可在这里处理体型缩放等效果
    }

    void OnAppearanceChanged()
    {
        ApplyCurrentAppearance();
    }

    void OnHPChanged(int currentHP, int maxHP)
    {
        if (currentHP <= 0)
        {
            SetAnimationBool("IsDead", true);
        }
    }

    void OnPlayerHurt()
    {
        FlashHurt();
        SetAnimationBool("IsHurt", true);
    }

    void OnPlayerRespawned()
    {
        StopAllCoroutines();
        appearanceRenderer?.ResetTint();
        SetAnimationBool("IsDead", false);
        SetAnimationBool("IsHurt", false);
        ApplyCurrentAppearance();
    }

    /// <summary>
    /// 触发受击闪烁
    /// </summary>
    public void FlashHurt()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(HurtFlashCoroutine());
    }

    IEnumerator HurtFlashCoroutine()
    {
        appearanceRenderer?.SetTint(hurtFlashColor);
        yield return new WaitForSeconds(hurtFlashDuration);
        appearanceRenderer?.ResetTint();
    }

    #region 动画代理

    public void SetAnimationFloat(string name, float value)
    {
        animationController?.SetFloat(name, value);
    }

    public void SetAnimationBool(string name, bool value)
    {
        animationController?.SetBool(name, value);
    }

    public void SetAnimationInteger(string name, int value)
    {
        animationController?.SetInteger(name, value);
    }

    public void SetAnimationTrigger(string name)
    {
        animationController?.SetTrigger(name);
    }

    /// <summary>
    /// 翻转角色朝向
    /// </summary>
    public void Flip(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    #endregion
}
