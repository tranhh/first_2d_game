using UnityEngine;

public class TimeEcho_JumpState : TimeEchoState
{
    public TimeEcho_JumpState(TimeEcho timeEcho, StateMachine stateMachine, string animBoolName)
        : base(timeEcho, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timeEcho.Jump();
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(timeEcho.fallState);
    }
}
