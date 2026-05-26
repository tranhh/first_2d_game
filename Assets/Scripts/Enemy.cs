using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;

    [Header("Movement details")]
    public float MoveSpeed = 1.4f;
    public float idleTime = 2;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1;


    [Header("Battle details")]
    public float battleMoveSpeed = 3;
    public float attackDistance = 2;
    public float battleTimeDuration = 5;
    public float minRetreatDistance = 5;
    public Vector2 retreatVelocity;


    [Header("Player Detection")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10;


    protected override void Update()
    {
        base.Update();
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
}
