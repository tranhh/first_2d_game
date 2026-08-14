using UnityEngine;

public class TimeEcho_BattleState : TimeEchoState
{
    public TimeEcho_BattleState(TimeEcho timeEcho, StateMachine stateMachine, string animBoolName)
        : base(timeEcho, stateMachine, animBoolName)
    {
    }


    public override void Update()
    {
        base.Update();

        if (target == null || shouldStopChasing())
            stateMachine.ChangeState(timeEcho.idleState);

        if (timeEcho.wallDetected && timeEcho.isGrounded)
            stateMachine.ChangeState(timeEcho.jumpState);

        if (!timeEcho.isGrounded)
            stateMachine.ChangeState(timeEcho.fallState);

        if (WithinAttackRange() && yDistanceToTarget() < 2f)
            stateMachine.ChangeState(timeEcho.attackState);

        if (!timeEcho.IsDead && !WithinAttackRange())
            timeEcho.MoveTowardsTarget(target);

    }


}

