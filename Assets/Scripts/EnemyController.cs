using UnityEngine;

/// <summary>
/// 敌人状态机控制器
/// 状态：Idle、Patrol、Chase、Attack、Hurt、Dead
/// </summary>
public class EnemyController : MonoBehaviour, IDamageable
{
    public enum EnemyState { Idle, Patrol, Chase, Attack, Hurt, Dead }

    [Header("基础属性")]
    public int maxHP = 30;
    public int currentHP;

    [Header("检测")]
    public float sightRange = 6f;
    public float attackRange = 1.2f;
    public LayerMask playerLayer;

    [Header("巡逻")]
    public float patrolSpeed = 1.5f;
    public float patrolDistance = 3f;
    public float idleWaitTime = 1f;

    [Header("追击")]
    public float chaseSpeed = 3.5f;

    [Header("攻击")]
    public AttackData attackData;
    public Transform attackPoint;

    [Header("受击")]
    public float hurtDuration = 0.3f;
    public float knockbackForce = 5f;
    public float knockbackUpForce = 2f;

    [Header("组件")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    private EnemyState currentState = EnemyState.Patrol;
    private Vector2 startPosition;
    private int patrolDirection = 1;
    private float idleTimer;
    private float stateTimer;
    private bool hasHit = false;
    private bool isFacingRight = true;

    private Transform target;

    public bool IsInvincible => currentState == EnemyState.Hurt || currentState == EnemyState.Dead;

    void Start()
    {
        currentHP = maxHP;
        startPosition = transform.position;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        InitAttackData();
        FindTarget();
    }

    void InitAttackData()
    {
        if (attackData == null)
            attackData = new AttackData();
    }

    void FindTarget()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
            target = player.transform;
    }

    void Update()
    {
        if (currentState == EnemyState.Dead) return;

        HandleCooldowns();
        HandleState();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.Hurt) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
        }
    }

    void HandleCooldowns()
    {
        if (currentState == EnemyState.Attack)
        {
            stateTimer -= Time.deltaTime;

            switch (attackPhase)
            {
                case AttackPhase.Startup:
                    if (stateTimer <= 0)
                        EnterAttackPhase(AttackPhase.Active, attackData.activeTime);
                    break;
                case AttackPhase.Active:
                    PerformAttackHit();
                    if (stateTimer <= 0)
                        EnterAttackPhase(AttackPhase.Recovery, attackData.recoveryTime);
                    break;
                case AttackPhase.Recovery:
                    if (stateTimer <= 0)
                        TransitionTo(EnemyState.Idle);
                    break;
            }
        }
        else if (currentState == EnemyState.Idle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
                TransitionTo(EnemyState.Patrol);
        }
    }

    void HandleState()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (currentState != EnemyState.Attack && currentState != EnemyState.Hurt)
        {
            if (distanceToTarget <= attackRange)
            {
                TransitionTo(EnemyState.Attack);
                return;
            }

            if (distanceToTarget <= sightRange)
            {
                TransitionTo(EnemyState.Chase);
                return;
            }

            if (currentState == EnemyState.Chase)
                TransitionTo(EnemyState.Patrol);
        }
    }

    private enum AttackPhase { Startup, Active, Recovery }
    private AttackPhase attackPhase = AttackPhase.Startup;

    void TransitionTo(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                idleTimer = idleWaitTime;
                break;
            case EnemyState.Patrol:
                patrolDirection = Mathf.RoundToInt(Mathf.Sign(startPosition.x - transform.position.x));
                if (patrolDirection == 0) patrolDirection = 1;
                break;
            case EnemyState.Attack:
                hasHit = false;
                EnterAttackPhase(AttackPhase.Startup, attackData.startupTime);
                FaceTarget();
                break;
        }
    }

    void EnterAttackPhase(AttackPhase phase, float duration)
    {
        attackPhase = phase;
        stateTimer = duration;
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

    void Chase()
    {
        if (target == null) return;

        FaceTarget();
        float dir = target.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
    }

    void FaceTarget()
    {
        if (target == null) return;

        bool shouldFaceRight = target.position.x > transform.position.x;
        if (shouldFaceRight != isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void PerformAttackHit()
    {
        if (hasHit || attackPoint == null || currentState != EnemyState.Attack || attackPhase != AttackPhase.Active)
            return;

        Collider2D[] players = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (var player in players)
        {
            IDamageable damageable = player.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsInvincible)
            {
                Vector2 knockback = new Vector2(
                    isFacingRight ? attackData.knockbackForce : -attackData.knockbackForce,
                    attackData.knockbackUpForce
                );
                damageable.TakeDamage(attackData.damage, knockback, transform.position);
            }
        }

        hasHit = true;
    }

    public void TakeDamage(int damage, Vector2 knockback, Vector2 attackerPosition)
    {
        if (IsInvincible) return;

        currentHP -= damage;

        // 打击反馈
        HitStopManager.Instance?.TriggerHitStop(attackData.hitStopDuration);
        CameraShake.Instance?.Shake(0.1f, 0.1f);

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // 进入受伤状态
        currentState = EnemyState.Hurt;
        CancelInvoke(nameof(EndHurt));
        Invoke(nameof(EndHurt), hurtDuration);

        // 击退
        float dir = transform.position.x >= attackerPosition.x ? 1f : -1f;
        rb.velocity = new Vector2(dir * knockbackForce, knockbackUpForce);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
    }

    void EndHurt()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        if (currentState != EnemyState.Dead)
            TransitionTo(EnemyState.Idle);
    }

    void Die()
    {
        currentState = EnemyState.Dead;
        rb.velocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("Die");

        Destroy(gameObject, 1f);
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetInteger("State", (int)currentState);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsHurt", currentState == EnemyState.Hurt);
        animator.SetBool("IsDead", currentState == EnemyState.Dead);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        else
            Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
