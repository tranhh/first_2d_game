using UnityEngine;

public class Player_TimeEchoState : PlayerState
{
    public Player_TimeEchoState(Player player, StateMachine stateMachine)
        : base(player, stateMachine, "")
    {
    }

    public override void Enter()
    {
        player.canMove = false;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        player.anim.SetTrigger("timeEchoTrigger");
    }

    public override void Update()
    {
        base.Update();
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        // Exit() will still be called if something cancelled the skill mid-way while casting ( like receiving a knock back or stun)
        triggerCalled = false;
        player.canMove = true;
        player.anim.ResetTrigger("timeEchoTrigger");
    }
}