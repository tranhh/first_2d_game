using UnityEngine;

public class TimeEcho_FallState : TimeEchoState
{
    public TimeEcho_FallState(TimeEcho timeEcho, StateMachine stateMachine, string animBoolName)
        : base(timeEcho, stateMachine, animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();
    }


    public override void Update()
    {
        base.Update();

        if (timeEcho.isGrounded)
        {
            stateMachine.ChangeState(timeEcho.battleState);
            return;
        }
    }
}