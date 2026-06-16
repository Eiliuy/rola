using UnityEngine;

/// <summary>
/// 玩家角色控制器（连招版）
/// 包含：移动、跳跃、攻击连招、闪避、受伤、下劈
/// </summary>
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("重力")]
    public float defaultGravityScale = 1f;

    [Header("移动")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float airControlFactor = 0.7f;

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
    public Transform attackPoint;
    public Transform airSlamAttackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public SlashEffect slashEffectPrefab;
    public float slashScaleMultiplier = 1f;
    public HitEffect hitEffectPrefab;

    [Header("地面连招")]
    public AttackData[] groundCombo = new AttackData[3];

    [Header("空中连招")]
    public AttackData[] airCombo = new AttackData[2];

    [Header("下劈")]
    public AttackData airSlam;
    public float airSlamSpeed = 20f;

    [Header("组件")]
    public Rigidbody2D rb;
    public PlayerStats stats;
    public PlayerVisualController visualController;

    /// <summary>
    /// 当前玩家 Build
    /// </summary>
    public PlayerBuild CurrentBuild => RunManager.Instance?.CurrentRun?.playerBuild;

    private SkillCooldown skillCooldown = new SkillCooldown();

    // 输入
    private float horizontalInput;
    private bool jumpInput;
    private bool attackInput;
    private bool slamInput;
    private bool dashInput;
    private bool[] skillInputs = new bool[4];

    // 状态
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isDashing = false;
    private bool isHurt = false;

    // 连招
    private enum AttackPhase { None, Startup, Active, Recovery }
    private AttackPhase attackPhase = AttackPhase.None;
    private int currentComboIndex = 0;
    private float phaseTimer = 0f;
    private bool hasHit = false;
    private bool inputBuffered = false;

    private enum PlayerState { Idle, Run, Jump, Fall, Attack, Dash, Hurt }
    private PlayerState currentState = PlayerState.Idle;

    // 计时器
    private float dashTimer;
    private float dashCooldownTimer;
    private float hurtTimer;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (stats == null) stats = GetComponent<PlayerStats>();
        if (visualController == null) visualController = GetComponent<PlayerVisualController>();

        if (rb != null)
            defaultGravityScale = rb.gravityScale;

        if (visualController == null)
            Debug.LogError("[PlayerController] 未找到 PlayerVisualController，角色视觉将无法正常工作。", this);

        if (RunManager.Instance != null && !RunManager.Instance.CurrentRun.isRunActive)
        {
            RunManager.Instance.StartNewRun(RunManager.Instance.defaultClass);
            if (stats != null)
                stats.SubscribeToBuild(CurrentBuild);
            if (visualController != null)
                visualController.ApplyCurrentAppearance();
        }

        InitDefaultComboData();
    }

    /// <summary>
    /// 如果没有配置数据，填充默认连招
    /// </summary>
    void InitDefaultComboData()
    {
        if (groundCombo == null || groundCombo.Length == 0)
        {
            groundCombo = new AttackData[3];
            for (int i = 0; i < groundCombo.Length; i++)
                groundCombo[i] = new AttackData();
        }

        if (airCombo == null || airCombo.Length == 0)
        {
            airCombo = new AttackData[2];
            for (int i = 0; i < airCombo.Length; i++)
                airCombo[i] = new AttackData();
        }

        if (airSlam == null)
            airSlam = new AttackData();
    }

    void Update()
    {
        GatherInput();
        HandleCooldowns();
        HandleState();
        HandleSkillInput();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        CheckGround();

        if (attackPhase != AttackPhase.Startup && attackPhase != AttackPhase.Active)
            Move();

        if (attackPhase == AttackPhase.Active)
            PerformAttackHit();
    }

    void GatherInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        attackInput = Input.GetButtonDown("Fire1");
        slamInput = Input.GetButtonDown("Fire2");
        dashInput = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);

        skillInputs[0] = Input.GetKeyDown(KeyCode.Alpha1);
        skillInputs[1] = Input.GetKeyDown(KeyCode.Alpha2);
        skillInputs[2] = Input.GetKeyDown(KeyCode.Alpha3);
        skillInputs[3] = Input.GetKeyDown(KeyCode.Alpha4);
    }

    void HandleCooldowns()
    {
        skillCooldown.Update(Time.deltaTime);

        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) EndDash();
        }

        if (isHurt)
        {
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0) EndHurt();
        }

        HandleAttackPhase();
    }

    void HandleState()
    {
        if (stats != null && stats.IsDead) { currentState = PlayerState.Hurt; return; }
        if (isHurt) { currentState = PlayerState.Hurt; return; }
        if (isDashing) { currentState = PlayerState.Dash; return; }
        if (attackPhase != AttackPhase.None) { currentState = PlayerState.Attack; return; }

        if (!isGrounded)
            currentState = rb.velocity.y > 0 ? PlayerState.Jump : PlayerState.Fall;
        else if (Mathf.Abs(horizontalInput) > 0.01f)
            currentState = PlayerState.Run;
        else
            currentState = PlayerState.Idle;

        if (jumpInput && isGrounded) Jump();
        if (attackInput) TryStartAttack();
        if (slamInput && !isGrounded) TryStartAirSlam();
        if (dashInput && dashCooldownTimer <= 0 && isGrounded) StartDash();
    }

    void Move()
    {
        float speed = isGrounded ? GetMoveSpeed() : GetMoveSpeed() * airControlFactor;
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        else if (horizontalInput < 0 && isFacingRight) Flip();
    }

    /// <summary>
    /// 获取当前移动速度
    /// </summary>
    float GetMoveSpeed()
    {
        return CurrentBuild != null ? CurrentBuild.CharacterStats.MoveSpeed : moveSpeed;
    }

    /// <summary>
    /// 获取当前跳跃力度
    /// </summary>
    float GetJumpForce()
    {
        return CurrentBuild != null ? CurrentBuild.CharacterStats.JumpForce : jumpForce;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        visualController?.Flip(isFacingRight);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, GetJumpForce());
        AudioManager.Instance?.PlayJumpSound(transform.position);
    }

    void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    #region 连招系统

    void TryStartAttack()
    {
        if (attackPhase != AttackPhase.None)
        {
            // 在输入缓冲窗口内预输入下一段
            if (CanBufferInput())
                inputBuffered = true;
            return;
        }

        currentComboIndex = 0;
        hasHit = false;
        StartAttackPhase(GetCurrentAttackData());
    }

    void TryStartAirSlam()
    {
        if (attackPhase != AttackPhase.None) return;

        currentComboIndex = -1; // -1 表示下劈
        hasHit = false;
        StartAttackPhase(airSlam);
    }

    AttackData GetCurrentAttackData()
    {
        if (currentComboIndex < 0)
            return airSlam;

        AttackData[] combo = isGrounded ? groundCombo : airCombo;
        int index = Mathf.Clamp(currentComboIndex, 0, combo.Length - 1);
        return combo[index];
    }

    void StartAttackPhase(AttackData data)
    {
        if (data == null) return;

        if (!isGrounded && !data.usableInAir)
            return;

        float timeScale = CurrentBuild != null ? CurrentBuild.GetAttackTimeScale() : 1f;

        attackPhase = AttackPhase.Startup;
        phaseTimer = data.startupTime * timeScale;

        // 攻击瞬间短暂停顿移动
        rb.velocity = new Vector2(0, rb.velocity.y);

        // 下劈直接向下加速
        if (currentComboIndex < 0)
            rb.velocity = new Vector2(0, -airSlamSpeed);
    }

    void HandleAttackPhase()
    {
        if (attackPhase == AttackPhase.None) return;

        phaseTimer -= Time.deltaTime;
        if (phaseTimer > 0) return;

        AttackData data = GetCurrentAttackData();
        float timeScale = CurrentBuild != null ? CurrentBuild.GetAttackTimeScale() : 1f;

        switch (attackPhase)
        {
            case AttackPhase.Startup:
                attackPhase = AttackPhase.Active;
                phaseTimer = data.activeTime * timeScale;
                SpawnSlashEffect();
                break;

            case AttackPhase.Active:
                attackPhase = AttackPhase.Recovery;
                phaseTimer = data.recoveryTime * timeScale;
                hasHit = false;
                break;

            case AttackPhase.Recovery:
                if (inputBuffered && CanContinueCombo())
                {
                    inputBuffered = false;
                    currentComboIndex++;
                    hasHit = false;
                    StartAttackPhase(GetCurrentAttackData());
                }
                else
                {
                    EndAttack();
                }
                break;
        }
    }

    bool CanBufferInput()
    {
        AttackData data = GetCurrentAttackData();
        return attackPhase == AttackPhase.Recovery && phaseTimer <= data.inputBufferTime;
    }

    bool CanContinueCombo()
    {
        AttackData[] combo = isGrounded ? groundCombo : airCombo;
        return currentComboIndex >= 0 && currentComboIndex < combo.Length - 1;
    }

    void PerformAttackHit()
    {
        AttackData data = GetCurrentAttackData();
        Transform currentAttackPoint = currentComboIndex < 0 ? airSlamAttackPoint : attackPoint;

        if (hasHit || currentAttackPoint == null) return;

        float range = CurrentBuild != null
            ? CurrentBuild.GetFinalAttackRange(attackRange, data)
            : attackRange * data.rangeMultiplier;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(currentAttackPoint.position, range, enemyLayer);
        bool hitThisFrame = false;
        Vector3 hitPosition = currentAttackPoint.position;

        foreach (var enemy in enemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsInvincible)
            {
                int damage = CurrentBuild != null
                    ? CurrentBuild.GetFinalAttackDamage(data)
                    : Mathf.RoundToInt(data.damage * data.damageMultiplier);

                damage = ApplyCrit(damage, data);

                Vector2 knockback = new Vector2(
                    isFacingRight ? data.knockbackForce : -data.knockbackForce,
                    data.knockbackUpForce
                );

                bool wasAlive = !IsDeadDamageable(damageable);
                damageable.TakeDamage(damage, knockback, transform.position);
                hitThisFrame = true;
                hitPosition = enemy.transform.position;

                CurrentBuild?.TriggerOnHitEnemy(damageable, hitPosition);

                if (wasAlive && IsDeadDamageable(damageable))
                    CurrentBuild?.TriggerOnKillEnemy(hitPosition);
            }
        }

        if (hitThisFrame)
        {
            hasHit = true;
            SpawnHitEffect(hitPosition);
            HitStopManager.Instance?.TriggerHitStop(data.hitStopDuration);
            CameraShake.Instance?.Shake(data.cameraShakeDuration, data.cameraShakeMagnitude);
        }
    }

    /// <summary>
    /// 检查目标是否已经死亡
    /// </summary>
    bool IsDeadDamageable(IDamageable damageable)
    {
        PlayerStats playerStats = (damageable as MonoBehaviour)?.GetComponent<PlayerStats>();
        if (playerStats != null) return playerStats.IsDead;

        EnemyController enemy = (damageable as MonoBehaviour)?.GetComponent<EnemyController>();
        if (enemy != null) return enemy.currentHP <= 0;

        return false;
    }

    /// <summary>
    /// 应用暴击
    /// </summary>
    int ApplyCrit(int damage, AttackData data)
    {
        if (!data.canCrit) return damage;

        float critChance = data.critChanceOverride >= 0f ? data.critChanceOverride : (CurrentBuild?.CharacterStats.CritChance ?? 0.05f);
        if (Random.value <= critChance)
        {
            float critDamage = CurrentBuild?.CharacterStats.CritDamage ?? 1.5f;
            return Mathf.RoundToInt(damage * critDamage);
        }
        return damage;
    }

    void SpawnHitEffect(Vector3 position)
    {
        if (hitEffectPrefab == null) return;
        Instantiate(hitEffectPrefab, position, Quaternion.identity);
    }

    void SpawnSlashEffect()
    {
        if (slashEffectPrefab == null) return;

        Transform currentAttackPoint = currentComboIndex < 0 ? airSlamAttackPoint : attackPoint;
        if (currentAttackPoint == null) return;

        Vector3 spawnPos = currentAttackPoint.position;
        Quaternion rot = Quaternion.identity;
        SlashEffect effect = Instantiate(slashEffectPrefab, spawnPos, rot, transform);

        // 下劈时旋转刀光朝下
        if (currentComboIndex < 0)
            effect.transform.Rotate(0, 0, -90f);

        effect.Setup(isFacingRight, slashScaleMultiplier);

        // 播放攻击音效，下劈用第一段音效
        int soundIndex = currentComboIndex < 0 ? 0 : currentComboIndex;
        AudioManager.Instance?.PlayAttackSound(soundIndex, transform.position);
    }

    void EndAttack()
    {
        attackPhase = AttackPhase.None;
        currentComboIndex = 0;
        inputBuffered = false;
        hasHit = false;
    }

    #endregion

    void StartDash()
    {
        EndAttack();
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        currentState = PlayerState.Dash;

        float dashDir = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(dashDir * dashSpeed, 0);
        rb.gravityScale = 0;

        CurrentBuild?.TriggerOnDodge();
        AudioManager.Instance?.PlayDashSound(transform.position);
    }

    void EndDash()
    {
        isDashing = false;
        rb.gravityScale = defaultGravityScale;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public bool IsInvincible => (isDashing && isInvincibleDuringDash) || (stats != null && stats.IsInvincible);

    public void TakeDamage(int damage, Vector2 knockback, Vector2 attackerPosition)
    {
        if (IsInvincible) return;

        isHurt = true;
        hurtTimer = 0.3f;
        currentState = PlayerState.Hurt;
        rb.velocity = knockback;
        EndAttack();

        AudioManager.Instance?.PlayHurtSound(transform.position);
        stats?.TakeDamage(damage);

        if (stats != null && stats.IsDead)
        {
            HandleDeath();
            return;
        }

        visualController?.FlashHurt();
    }

    void HandleDeath()
    {
        // 简单处理：1 秒后重生
        rb.velocity = Vector2.zero;
        Invoke(nameof(Respawn), 1f);
    }

    void Respawn()
    {
        stats?.Respawn();
        isHurt = false;
        isDashing = false;
        currentState = PlayerState.Idle;
        rb.gravityScale = defaultGravityScale;
        EndAttack();
    }

    void EndHurt()
    {
        isHurt = false;
    }

    #region 技能系统

    /// <summary>
    /// 处理技能输入
    /// </summary>
    void HandleSkillInput()
    {
        if (CurrentBuild == null) return;

        for (int i = 0; i < skillInputs.Length; i++)
        {
            if (skillInputs[i] && i < CurrentBuild.Skills.Count)
            {
                TryUseSkill(CurrentBuild.Skills[i]);
                skillInputs[i] = false;
            }
        }
    }

    /// <summary>
    /// 尝试释放技能
    /// </summary>
    public void TryUseSkill(SkillData skill)
    {
        if (skill == null) return;
        if (CurrentBuild == null) return;
        if (!skillCooldown.IsReady(skill)) return;
        if (attackPhase != AttackPhase.None) return;
        if (isHurt) return;

        // 技能释放：创建效果、播放动画、进入冷却
        skillCooldown.StartCooldown(skill, CurrentBuild.GetSkillCooldown(skill));

        if (skill.effectPrefab != null)
        {
            Vector3 spawnPos = attackPoint != null ? attackPoint.position : transform.position;
            Instantiate(skill.effectPrefab, spawnPos, Quaternion.identity);
        }

        if (!string.IsNullOrEmpty(skill.animationTrigger))
            visualController?.SetAnimationTrigger(skill.animationTrigger);

        if (skill.castSound != null)
            AudioManager.Instance?.PlaySFX(skill.castSound, transform.position);

        // 简单技能伤害：在攻击点范围造成伤害
        float range = attackRange * skill.rangeMultiplier * CurrentBuild.CharacterStats.AttackRangeMultiplier;
        int damage = Mathf.RoundToInt(skill.baseDamage * skill.damageMultiplier * (1f + CurrentBuild.CharacterStats.AttackPower / 10f));

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint != null ? attackPoint.position : transform.position,
            range,
            enemyLayer
        );

        foreach (var enemy in enemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsInvincible)
            {
                Vector2 knockback = new Vector2(
                    isFacingRight ? skill.knockbackForce : -skill.knockbackForce,
                    skill.knockbackUpForce
                );

                bool wasAlive = !IsDeadDamageable(damageable);
                damageable.TakeDamage(damage, knockback, transform.position);
                CurrentBuild.TriggerOnHitEnemy(damageable, enemy.transform.position);

                if (wasAlive && IsDeadDamageable(damageable))
                    CurrentBuild.TriggerOnKillEnemy(enemy.transform.position);
            }
        }
    }

    #endregion

    void UpdateAnimations()
    {
        if (visualController == null) return;

        visualController.SetAnimationFloat("Speed", Mathf.Abs(rb.velocity.x));
        visualController.SetAnimationBool("IsGrounded", isGrounded);
        visualController.SetAnimationFloat("VerticalVelocity", rb.velocity.y);
        visualController.SetAnimationBool("IsAttacking", attackPhase != AttackPhase.None);
        visualController.SetAnimationInteger("ComboIndex", currentComboIndex);
        visualController.SetAnimationBool("IsDashing", isDashing);
        visualController.SetAnimationBool("IsHurt", isHurt);
        if (stats != null)
            visualController.SetAnimationBool("IsDead", stats.IsDead);
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
            float range = attackRange;
            Gizmos.DrawWireSphere(attackPoint.position, range);
        }
    }
}
