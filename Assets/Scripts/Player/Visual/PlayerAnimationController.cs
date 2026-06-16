using UnityEngine;

/// <summary>
/// 玩家动画控制器
/// 封装 Animator 操作，支持按外观切换动画控制器
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 切换动画控制器
    /// </summary>
    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        if (animator != null)
            animator.runtimeAnimatorController = controller;
    }

    /// <summary>
    /// 设置浮点参数
    /// </summary>
    public void SetFloat(string name, float value)
    {
        if (animator != null)
            animator.SetFloat(name, value);
    }

    /// <summary>
    /// 设置布尔参数
    /// </summary>
    public void SetBool(string name, bool value)
    {
        if (animator != null)
            animator.SetBool(name, value);
    }

    /// <summary>
    /// 设置整数参数
    /// </summary>
    public void SetInteger(string name, int value)
    {
        if (animator != null)
            animator.SetInteger(name, value);
    }

    /// <summary>
    /// 设置触发器
    /// </summary>
    public void SetTrigger(string name)
    {
        if (animator != null && !string.IsNullOrEmpty(name))
            animator.SetTrigger(name);
    }
}
