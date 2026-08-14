using System.Collections;
using UnityEngine;

// ----------- Entity ------------
public class TimeEcho : Entity
{
    private Transform playerTransform;
    private Skill_TimeEcho echoManager;
    private TrailRenderer wispTrail;

    private Entity_Combat entity_Combat;
    private Entity_StatusHandler statusHandler;
    public TimeEcho_IdleState idleState { get; private set; }
    public TimeEcho_BattleState battleState { get; private set; }
    public TimeEcho_JumpState jumpState { get; private set; }
    public TimeEcho_FallState fallState { get; private set; }
    public TimeEcho_BasicAttackState attackState { get; private set; }

    private float jumpForce = 5f;
    private bool canAttack;
    private bool canApplyOnHit;

    public bool CanAttack => canAttack;
    public bool CanApplyOnHit => canApplyOnHit;
    public Vector2[] attackVelocity { get; private set; }
    public float attackVelocityDuration { get; private set; }
    public float attackDistance = 2f;

    private float soulLinkHealAmount;
    protected bool isDead;
    public bool IsDead => isDead;

    [SerializeField] private GameObject onDeathVfx;

    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask whatIsTarget;

    protected override void Awake()
    {
        base.Awake();
        entity_Combat = GetComponent<Entity_Combat>();
        idleState = new TimeEcho_IdleState(this, stateMachine, "idle");
        battleState = new TimeEcho_BattleState(this, stateMachine, "move");
        jumpState = new TimeEcho_JumpState(this, stateMachine, "jumpFall");
        fallState = new TimeEcho_FallState(this, stateMachine, "jumpFall");
        attackState = new TimeEcho_BasicAttackState(this, stateMachine, "attack");

        stateMachine.Initialize(idleState);
    }
    protected override void Start()
    {
        entity_Combat.OnDamageDealt += HandleSoulLinkHeal;
    }

    protected override void Update()
    {
        base.Update();
        if (echoManager.CanCreateWisp() && isDead)
            HandleWispMovement();

    }

    public void SetupEcho(Skill_TimeEcho echoManager, float cloneStatsMultiplier, SkillUpgradeType upgradeType, float duration)
    {
        this.echoManager = echoManager;
        SetupUpgradeType(upgradeType);
        SetupStats(cloneStatsMultiplier);
        SetupWisp();

        Invoke(nameof(HandleDeath), duration);
    }

    private void SetupWisp()
    {
        if (!echoManager.CanCreateWisp())
            return;

        playerTransform = echoManager.transform.root;
        statusHandler = echoManager.player.statusHandler;

        wispTrail = GetComponentInChildren<TrailRenderer>(true);
        wispTrail.gameObject.SetActive(false);
    }

    private void SetupUpgradeType(SkillUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case SkillUpgradeType.timeEcho_SoulManifestation:
            case SkillUpgradeType.timeEcho_SoulBound:
            case SkillUpgradeType.timeEcho_SoulLink:
            case SkillUpgradeType.timeEcho_SoulPurge:
                canAttack = true;
                canApplyOnHit = false;
                break;

            case SkillUpgradeType.timeEcho_Resonance:
                canAttack = true;
                canApplyOnHit = true;
                break;

            default:
                canAttack = false;
                canApplyOnHit = false;
                break;
        }
    }

    private void SetupStats(float cloneStatsMultiplier)
    {
        stats.CopyStats(echoManager.player.stats, cloneStatsMultiplier);
        baseMoveSpeed = echoManager.player.moveSpeed;
        jumpForce = echoManager.player.jumpForce;
        attackVelocity = (Vector2[])echoManager.player.attackVelocity.Clone();
        attackVelocityDuration = echoManager.player.attackVelocityDuration;
    }

    private void HandleSoulLinkHeal(DamageResult result)
    {
        if (!echoManager.CanCreateWisp())
            return;

        soulLinkHealAmount += result.damageDealt * echoManager.soulLinkHealPercent;
    }

    private void HandleWispMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, echoManager.wispMoveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, playerTransform.position) < .5f)
        {
            echoManager.player.GetComponent<Entity_Health>().HealOverTime(soulLinkHealAmount, echoManager.soulLinkHealDuration, echoManager.soulLinkHealInterval);

            if (echoManager.CanRemoveNegativeEffects())
                statusHandler.RemoveAllNegativeEffects();

            Destroy(gameObject);
        }
    }

    private void HandleResonance()
    {
        if (!canApplyOnHit)
            return;

        // add logic later ...
    }

    public void Jump()
    {
        SetVelocity(rb.linearVelocity.x * 0.75f, jumpForce);
    }

    public void HandleDeath()
    {
        if (isDead)
            return;

        isDead = true;

        Instantiate(onDeathVfx, transform.position, Quaternion.identity);

        if (echoManager.CanCreateWisp())
            TurnIntoWisp();
        else
            Destroy(gameObject);
    }

    private void TurnIntoWisp()
    {
        Transform healthBar = transform.Find("HealthBar_UI");

        if (playerTransform == null || healthBar == null)
        {
            Destroy(gameObject);
            return;
        }

        anim.gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        healthBar.gameObject.SetActive(false);

        wispTrail.gameObject.SetActive(true);
        rb.simulated = false;
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("SkillObject_Sword"));
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    public void MoveTowardsTarget(Transform target)
    {
        if (target == null)
            return;

        float direction = target.position.x > transform.position.x ? 1 : -1;

        SetVelocity(direction * baseMoveSpeed, rb.linearVelocity.y);

        if (direction != 0)
            HandleFlip(direction);
    }

    public Transform GetClosestTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(
            transform.position,
            detectionRadius,
            whatIsTarget
        );

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D target in targets)
        {
            float distance = Vector2.Distance(
                transform.position,
                target.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target.transform;
            }
        }

        return closestTarget;
    }

    public override void EntityDeath()
    {
        HandleDeath();
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        ApplySlow(slowMultiplier);

        yield return new WaitForSeconds(duration);

        RemoveSlow();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Attack distance
        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.right * attackDistance
        );
    }
}
