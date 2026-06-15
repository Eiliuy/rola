using UnityEngine;

/// <summary>
/// 玩家行为观察器
/// 检测重复/无聊行为并触发旁白吐槽
/// </summary>
public class NarrationBehaviorWatcher : MonoBehaviour
{
    [System.Serializable]
    public class BehaviorRule
    {
        [Tooltip("行为类型")]
        public BehaviorType type;

        [Tooltip("阈值次数")]
        public int threshold = 5;

        [Tooltip("时间窗口（秒），超过则计数重置")]
        public float timeWindow = 3f;

        [Tooltip("触发旁白")]
        public NarrationData narration;

        [Tooltip("只触发一次")]
        public bool triggerOnce = true;
    }

    public enum BehaviorType { RepeatedJump, RepeatedAttack, IdleTooLong }

    [Header("行为规则")]
    public BehaviorRule[] rules;

    private int jumpCount = 0;
    private int attackCount = 0;
    private float idleTimer = 0f;

    private float[] ruleTimers;
    private bool[] ruleTriggered;

    void Start()
    {
        if (rules == null) rules = new BehaviorRule[0];

        ruleTimers = new float[rules.Length];
        ruleTriggered = new bool[rules.Length];
    }

    void Update()
    {
        // 监听输入
        if (Input.GetButtonDown("Jump"))
            jumpCount++;

        if (Input.GetButtonDown("Fire1"))
            attackCount++;

        // 检测静止
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || Input.GetButtonDown("Jump"))
            idleTimer = 0f;
        else
            idleTimer += Time.deltaTime;

        // 检查规则
        for (int i = 0; i < rules.Length; i++)
        {
            BehaviorRule rule = rules[i];
            if (rule == null || rule.narration == null) continue;
            if (rule.triggerOnce && ruleTriggered[i]) continue;

            ruleTimers[i] += Time.deltaTime;
            if (ruleTimers[i] > rule.timeWindow)
            {
                ResetCounters();
                ruleTimers[i] = 0f;
                continue;
            }

            bool triggered = false;
            switch (rule.type)
            {
                case BehaviorType.RepeatedJump:
                    triggered = jumpCount >= rule.threshold;
                    break;
                case BehaviorType.RepeatedAttack:
                    triggered = attackCount >= rule.threshold;
                    break;
                case BehaviorType.IdleTooLong:
                    triggered = idleTimer >= rule.threshold;
                    break;
            }

            if (triggered)
            {
                NarrationManager.Instance?.PlayNarration(rule.narration);
                ruleTriggered[i] = true;
                ResetCounters();
                ruleTimers[i] = 0f;
            }
        }
    }

    void ResetCounters()
    {
        jumpCount = 0;
        attackCount = 0;
    }
}
