using UnityEngine;

/// <summary>
/// 玩家角色控制器（基础版）
/// 包含：移动、跳跃、攻击、闪避、受伤
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("移动")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("地面检测")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("闪避")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public bool isInvincibleDuringDash = true;

    [Header("攻击")]
    public float attackDuration = 0.3f;
    public float attackCooldown = 0.4f;
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public int attackDamage = 10;

    [Header("组件")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    // 输入
    private float horizontalInput;
    private bool jumpInput;
    private bool attackInput;
    private bool dashInput;

    // 状态
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isDashing = false;
    private bool isAttacking = false;

    // 计时器
    private float dashTimer;
    private float dashCooldownTimer;
    private float attackTimer;
    private float attackCooldownTimer;

    private enum PlayerState { Idle, Run, Jump, Fall, Attack, Dash, Hurt }
    private PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GatherInput();
        HandleCooldowns();
        HandleState();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        Move();
        CheckGround();
    }

    /// <summary>
    /// 收集玩家输入（后续可替换为 Unity Input System）
    /// </summary>
    void GatherInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        attackInput = Input.GetButtonDown("Fire1");
        dashInput = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }

    void HandleCooldowns()
    {
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) EndDash();
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0) EndAttack();
        }
    }

    void HandleState()
    {
        if (currentState == PlayerState.Hurt) return;

        if (isDashing) { currentState = PlayerState.Dash; return; }
        if (isAttacking) { currentState = PlayerState.Attack; return; }

        if (!isGrounded)
            currentState = rb.velocity.y > 0 ? PlayerState.Jump : PlayerState.Fall;
        else if (Mathf.Abs(horizontalInput) > 0.01f)
            currentState = PlayerState.Run;
        else
            currentState = PlayerState.Idle;

        if (jumpInput && isGrounded) Jump();
        if (attackInput && attackCooldownTimer <= 0 && !isAttacking) StartAttack();
        if (dashInput && dashCooldownTimer <= 0 && !isDashing && isGrounded) StartDash();
    }

    void Move()
    {
        if (isAttacking) return; // 攻击时不能移动，可调整

        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        else if (horizontalInput < 0 && isFacingRight) Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        attackCooldownTimer = attackCooldown;
        currentState = PlayerState.Attack;
        rb.velocity = new Vector2(0, rb.velocity.y);

        PerformAttackHit();
    }

    void PerformAttackHit()
    {
        if (attackPoint == null) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        bool hasHit = false;

        foreach (var enemy in enemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsInvincible)
            {
                Vector2 knockback = new Vector2(isFacingRight ? 1f : -1f, 0.5f);
                damageable.TakeDamage(attackDamage, knockback, transform.position);
                hasHit = true;
            }
        }

        if (hasHit)
        {
            HitStopManager.Instance?.TriggerHitStop(0.05f);
            CameraShake.Instance?.Shake(0.08f, 0.08f);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        currentState = PlayerState.Dash;

        float dashDir = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(dashDir * dashSpeed, 0);
        rb.gravityScale = 0;
    }

    void EndDash()
    {
        isDashing = false;
        rb.gravityScale = 1;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsDashing", isDashing);
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isDashing && isInvincibleDuringDash) return;

        currentState = PlayerState.Hurt;
        rb.velocity = knockback;
        // TODO: 扣血、无敌帧、受伤动画
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
