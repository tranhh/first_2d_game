using UnityEngine;

public class Player_DomainExpansionState : PlayerState
{
    private Vector2 originalPosition;
    private float originalGravity;
    private float maxDistanceToGoUp;
    private bool isLevitating;
    private bool createDomain;

    public Player_DomainExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        originalPosition = player.transform.position;
        originalGravity = rb.gravityScale;
        maxDistanceToGoUp = GetAvailableRiseDistance();
        stateTimer = skillManager.domainExpansion.GetDomainDuration();
        player.SetVelocity(0, player.riseSpeed);
    }

    public override void Update()
    {
        base.Update();
        if (Vector2.Distance(originalPosition, player.transform.position) >= maxDistanceToGoUp && !isLevitating)
            Levitate();

        if (isLevitating && stateTimer <= 0)
            stateMachine.ChangeState(player.fallState);

    }

    public override void Exit()
    {
        base.Exit();

        HandleExitDomain();
    }

    private void HandleExitDomain()
    {
        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        isLevitating = false;
        createDomain = false;
    }

    private void Levitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        if (!createDomain)
        {
            createDomain = true;
            skillManager.domainExpansion.CreateDomain();
        }
    }

    private float GetAvailableRiseDistance()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.up, player.maxRiseDistance + 5f, player.WhatIsGround);

        if (hit.collider != null)
        {
            Debug.Log($"Ceiling hit: {hit.collider.gameObject.name}");
            Debug.Log($"Distance: {hit.distance}");
            return Mathf.Clamp(hit.distance - 1.25f, 0f, player.maxRiseDistance);
        }

        return player.maxRiseDistance;
    }
}
