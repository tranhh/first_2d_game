using System;
using UnityEngine;

public class Player_DeadState : PlayerState
{
    public Player_DeadState(Player player, StateMachine stateMachine, String animboolname) : base(player, stateMachine, animboolname)
    {

    }
    public override void Enter()
    {
        base.Enter();

        input.Disable();
        rb.simulated = false;
    }
}
