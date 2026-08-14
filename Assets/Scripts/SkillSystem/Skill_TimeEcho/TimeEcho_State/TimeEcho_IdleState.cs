using UnityEngine;

public class TimeEcho_IdleState : TimeEchoState
{
    private float spawnDelay = .25f;
    private float timer;
    private bool canSearchForTarget;
    public TimeEcho_IdleState(TimeEcho timeEcho, StateMachine stateMachine, string animBoolName) : base(timeEcho, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        timer = spawnDelay;
        canSearchForTarget = false;
    }

    public override void Update()
    {
        base.Update();

        if (!timeEcho.CanAttack)
            return;

        Transform target = timeEcho.GetClosestTarget();
        if (!canSearchForTarget)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
                canSearchForTarget = true;

            return;
        }
        if (target != null)
            stateMachine.ChangeState(timeEcho.battleState);
    }
}
