using UnityEngine;

public class TimeEcho_BattleState : TimeEchoState
{
    private Transform target;

    public TimeEcho_BattleState(TimeEcho timeEcho, StateMachine stateMachine, string animBoolName)
        : base(timeEcho, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        target = timeEcho.GetClosestTarget();
    }


    public override void Update()
    {
        base.Update();

        // if target is gone, find another one
        if (target == null)
        {
            target = timeEcho.GetClosestTarget();

            if (target == null)
                stateMachine.ChangeState(timeEcho.idleState);
        }

        if (timeEcho.wallDetected && timeEcho.isGrounded)
            stateMachine.ChangeState(timeEcho.jumpState);

        if (!timeEcho.isGrounded)
            stateMachine.ChangeState(timeEcho.fallState);

        if (WithinAttackRange())
            stateMachine.ChangeState(timeEcho.attackState);

        if (!timeEcho.IsDead)
            timeEcho.MoveTowardsTarget(target);

    }

    private bool WithinAttackRange() => DistanceToTarget() < timeEcho.attackDistance;

    private float DistanceToTarget()
    {
        if (target == null)
            return float.MaxValue;

        return Mathf.Abs(
            target.position.x - timeEcho.transform.position.x
        );
    }
}

