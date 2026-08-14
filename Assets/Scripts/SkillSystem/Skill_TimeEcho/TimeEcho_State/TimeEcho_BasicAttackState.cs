using UnityEngine;

public class TimeEcho_BasicAttackState : TimeEchoState
{
    private int comboIndex = 1;
    private int comboLimit = 3;

    private float attackVelocityTimer;
    private int attackDir;

    public TimeEcho_BasicAttackState(
        TimeEcho timeEcho,
        StateMachine stateMachine,
        string animBoolName)
        : base(timeEcho, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timeEcho.SetVelocity(0, rb.linearVelocity.y);
        SyncAttackSpeed();
        attackDir = timeEcho.facingDir;

        anim.SetInteger("attackIndex", comboIndex);

        GenerateAttackVelocity();
    }

    public override void Update()
    {
        base.Update();

        HandleAttackMovement();

        if (triggerCalled)
            stateMachine.ChangeState(timeEcho.battleState);
    }


    private void GenerateAttackVelocity()
    {
        if (timeEcho.attackVelocity == null)
            Debug.Log("TimeEcho attackVelocity is NULL");

        Vector2 attackVelocity = timeEcho.attackVelocity[comboIndex - 1];

        attackVelocityTimer = timeEcho.attackVelocityDuration;

        timeEcho.SetVelocity(
            attackVelocity.x * attackDir,
            attackVelocity.y
        );
    }

    private void HandleAttackMovement()
    {
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
            timeEcho.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        comboIndex++;

        if (comboIndex > comboLimit)
            comboIndex = 1;
    }
}