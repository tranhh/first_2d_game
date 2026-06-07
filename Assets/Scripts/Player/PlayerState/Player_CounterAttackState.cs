using System;
using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    public Player_CounterAttackState(Player player, StateMachine stateMachine, String animBoolName) : base(player, stateMachine, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();

        stateTimer = 1;
    }
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
