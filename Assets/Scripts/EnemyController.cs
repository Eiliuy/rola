using UnityEngine;

/// <summary>
/// 简单敌人控制器
/// 包含：巡逻、受击、击退、死亡
/// </summary>
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("基础属性")]
    public int maxHP = 30;
    public int currentHP;

    [Header("巡逻")]
    public float patrolSpeed = 1.5f;
    public float patrolDistance = 3f;

    [Header("受击")]
    public float hurtDuration = 0.3f;
    public float hurtInvincibleDuration = 0.2f;
    public float knockbackForce = 5f;
    public float knockbackUpForce = 2f;

    [Header("组件")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    private Vector2 startPosition;
    private int patrolDirection = 1;
    private bool isHurt = false;
    private bool isDead = false;

    public bool IsInvincible => isHurt || isDead;

    void Start()
    {
        currentHP = maxHP;
        startPosition = transform.position;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return;

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (isDead || isHurt) return;

        Patrol();
    }

    void Patrol()
    {
        if (patrolDistance <= 0) return;

        rb.velocity = new Vector2(patrolDirection * patrolSpeed, rb.velocity.y);

        float distanceFromStart = transform.position.x - startPosition.x;
        if (Mathf.Abs(distanceFromStart) >= patrolDistance)
        {
            patrolDirection *= -1;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(int damage, Vector2 knockback, Vector2 attackerPosition)
    {
        if (IsInvincible) return;

        currentHP -= damage;

        // 触发打击反馈
        HitStopManager.Instance?.TriggerHitStop(0.05f);
        CameraShake.Instance?.Shake(0.1f, 0.1f);

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // 计算击退方向
        float dir = transform.position.x >= attackerPosition.x ? 1f : -1f;
        rb.velocity = new Vector2(dir * knockbackForce, knockbackUpForce);

        isHurt = true;
        Invoke(nameof(EndHurt), hurtDuration);

        // 受伤闪烁
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
    }

    void EndHurt()
    {
        isHurt = false;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("Die");

        // 简单做法：1 秒后销毁
        Destroy(gameObject, 1f);
    }

    void UpdateAnimation()
    {
        if (animator == null) return;
        animator.SetBool("IsHurt", isHurt);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }
}
