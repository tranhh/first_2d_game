using UnityEngine;

public class TimeEchoState : EntityState
{
    protected TimeEcho timeEcho;

    public TimeEchoState(TimeEcho timeEcho, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.timeEcho = timeEcho;

        rb = timeEcho.rb;
        anim = timeEcho.anim;
        stats = timeEcho.stats;
    }

    public override void UpdateAnimationParameters()
    {
        anim.SetFloat("Yvelocity", rb.linearVelocity.y);
    }
}
