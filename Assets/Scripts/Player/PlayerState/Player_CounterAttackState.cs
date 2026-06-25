using System;
using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    private Player_Combat combat;
    private bool hasCountered;
    public Player_CounterAttackState(Player player, StateMachine stateMachine, String animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = combat.GetCounterRecoveryDuration();
        hasCountered = combat.CounterAttackPerformed();

        anim.SetBool("counterAttackPerformed", hasCountered);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, rb.linearVelocity.y);

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (stateTimer < 0 && hasCountered == false)
            stateMachine.ChangeState(player.idleState);
    }

}
