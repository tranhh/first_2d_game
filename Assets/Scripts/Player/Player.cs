using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    private UI ui;
    public static event Action OnPlayerDeath;
    public PlayerInputSet input { get; private set; }

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_CounterAttackState counterAttackState { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;

    public Vector2 baseJumpAttackVelocity;
    [HideInInspector] public Vector2 jumpAttackVelocity => baseJumpAttackVelocity * speedMultiplier;

    public float baseAttackVelocityDuration = .1f;
    [HideInInspector] public float attackVelocityDuration => baseAttackVelocityDuration * speedMultiplier;

    public float baseComboResetTime = .75f;
    [HideInInspector] public float comboResetTime => baseComboResetTime / speedMultiplier;
    private Coroutine queuedAttackCo;

    [Header("Movement details")]
    [HideInInspector] public float moveSpeed => baseMoveSpeed * speedMultiplier;

    public float jumpForce = 5.0f;

    public Vector2 baseWallJumpForce;
    [HideInInspector] public Vector2 wallJumpForce => baseWallJumpForce * speedMultiplier;

    [Range(0, 1)]
    public float inAirMultiplier = .7f; // 0 ~ 1 only
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = .5f; // 0 ~ 1 only
    [Space]
    public float baseDashDuration = .25f;
    [HideInInspector] public float dashDuration => baseDashDuration / speedMultiplier;

    public float baseDashSpeed = 20;
    [HideInInspector] public float dashSpeed => baseDashSpeed * speedMultiplier;


    public Vector2 moveInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        ui = FindAnyObjectByType<UI>();
        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "isDead");
        counterAttackState = new Player_CounterAttackState(this, stateMachine, "counterAttack");
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        ApplySlow(slowMultiplier);

        yield return new WaitForSeconds(duration);

        RemoveSlow();
    }

    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }
    void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        input.Player.ToggleSkillTreeUI.performed += ctx => ui.ToggleSkillTreeUI();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);
        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }
}
