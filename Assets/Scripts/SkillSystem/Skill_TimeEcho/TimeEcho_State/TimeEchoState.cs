using UnityEngine;

public class TimeEchoState : EntityState
{
    protected TimeEcho timeEcho;
    protected Transform target;

    public TimeEchoState(TimeEcho timeEcho, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.timeEcho = timeEcho;

        rb = timeEcho.rb;
        anim = timeEcho.anim;
        stats = timeEcho.stats;
    }

    public override void Update()
    {
        base.Update();
        target = timeEcho.GetClosestTarget();

    }

    public override void UpdateAnimationParameters()
    {
        anim.SetFloat("Yvelocity", rb.linearVelocity.y);
    }

    protected bool shouldStopChasing() => WithinAttackRange() && (yDistanceToTarget() > 2f);
    protected bool WithinAttackRange() => xDistanceToTarget() < timeEcho.attackDistance;

    protected float xDistanceToTarget()
    {
        if (target == null)
            return float.MaxValue;

        return Mathf.Abs(target.position.x - timeEcho.transform.position.x);
    }

    protected float yDistanceToTarget()
    {
        if (target == null)
            return float.MaxValue;

        return Mathf.Abs(target.position.y - timeEcho.transform.position.y);
    }
}
