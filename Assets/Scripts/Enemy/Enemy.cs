using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    private Enemy_Health health;
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_StunnedState stunnedState;

    [Header("Movement details")]
    public float idleTime = 2;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1;



    [Header("Battle details")]
    public float baseBattleMoveSpeed = 3;
    public float attackDistance = 2;
    public float battleTimeDuration = 5;
    public float minRetreatDistance = 5;
    public Vector2 retreatVelocity;

    [Header("Status Effect Details")]
    public float moveSpeed => baseMoveSpeed * speedMultiplier;
    public float battleMoveSpeed => baseBattleMoveSpeed * speedMultiplier;


    [Header("Stunned State Details")]
    public float stunnedDuration = 1;
    public Vector2 stunnedVelocity = new Vector2(7, 7);
    [SerializeField] protected bool canBeStunned;


    [Header("Player Detection")]
    public Transform player { get; private set; }
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10;


    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Enemy_Health>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        ApplySlow(slowMultiplier);

        yield return new WaitForSeconds(duration);

        RemoveSlow();
    }

    public void EnableCounterWindow(bool enable) => canBeStunned = enable;

    public override void EntityDeath()
    {
        base.EntityDeath();

        stateMachine.ChangeState(deadState);
    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }
    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;
        this.player = player;
        HandleFlip(DirectionToTarget());
        stateMachine.ChangeState(battleState);
    }

    private int DirectionToTarget() => player.position.x > transform.position.x ? 1 : -1;

    public Transform GetPlayerReference()
    {
        if (player == null)
            player = PlayerDetected().transform;

        return player;
    }

    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | WhatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;
        return hit;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, playerCheck.position + Vector3.right * facingDir * playerCheckDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, playerCheck.position + Vector3.right * facingDir * attackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, playerCheck.position + Vector3.right * facingDir * minRetreatDistance);
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }
}
