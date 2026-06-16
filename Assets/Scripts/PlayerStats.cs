using UnityEngine;

/// <summary>
/// 玩家属性：血量、死亡、重生
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("血量")]
    public int maxHP = 100;
    public int currentHP;

    [Header("无敌帧")]
    public float invincibleDuration = 0.5f;

    [Header("重生")]
    public Vector3 respawnPosition;

    public bool IsDead => currentHP <= 0;
    public bool IsInvincible { get; private set; }

    public PlayerBuild CurrentBuild => RunManager.Instance?.CurrentRun?.playerBuild;

    private PlayerBuild subscribedBuild;

    public System.Action<int, int> OnHPChanged;
    public System.Action OnPlayerDied;
    public System.Action OnPlayerRespawned;

    void Start()
    {
        SubscribeToBuild(CurrentBuild);

        if (CurrentBuild == null)
            currentHP = maxHP;

        respawnPosition = transform.position;
        IsInvincible = false;
    }

    void OnDestroy()
    {
        SubscribeToBuild(null);
    }

    public void SubscribeToBuild(PlayerBuild build)
    {
        if (subscribedBuild != null)
            subscribedBuild.OnStatsChanged -= OnBuildStatsChanged;
        subscribedBuild = build;
        if (subscribedBuild != null)
        {
            subscribedBuild.OnStatsChanged += OnBuildStatsChanged;
            OnBuildStatsChanged();
        }
    }

    void OnBuildStatsChanged()
    {
        if (subscribedBuild == null) return;

        int oldMaxHP = maxHP;
        maxHP = subscribedBuild.CharacterStats.MaxHP;

        if (oldMaxHP <= 0)
        {
            currentHP = maxHP;
        }
        else if (maxHP > oldMaxHP)
        {
            currentHP = Mathf.RoundToInt(currentHP * ((float)maxHP / oldMaxHP));
        }
        else
        {
            currentHP = Mathf.Min(currentHP, maxHP);
        }

        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int damage)
    {
        if (IsInvincible || IsDead) return;

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
        CurrentBuild?.TriggerOnDamaged(damage, Vector2.zero);
        CurrentBuild?.TriggerOnHPChanged(currentHP, maxHP);

        if (currentHP <= 0)
            Die();
        else
            StartInvincible();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);
        OnHPChanged?.Invoke(currentHP, maxHP);
        CurrentBuild?.TriggerOnHPChanged(currentHP, maxHP);
    }

    void Die()
    {
        OnPlayerDied?.Invoke();
        // 重生延迟，由调用方控制
    }

    public void Respawn()
    {
        if (subscribedBuild != null)
            maxHP = subscribedBuild.CharacterStats.MaxHP;

        currentHP = maxHP;
        IsInvincible = false;
        transform.position = respawnPosition;
        OnHPChanged?.Invoke(currentHP, maxHP);
        CurrentBuild?.TriggerOnHPChanged(currentHP, maxHP);
        OnPlayerRespawned?.Invoke();
    }

    public void SetRespawnPoint(Vector3 position)
    {
        respawnPosition = position;
    }

    void StartInvincible()
    {
        IsInvincible = true;
        CancelInvoke(nameof(EndInvincible));
        Invoke(nameof(EndInvincible), invincibleDuration);
    }

    void EndInvincible()
    {
        IsInvincible = false;
    }
}
