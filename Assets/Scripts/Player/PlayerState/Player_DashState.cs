using UnityEngine;

public class Player_DashState : PlayerState
{
    private float OriginalGravityScale;
    private int dashDir;
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        skillManager.dash.OnStartEffect();
        player.vfx.PlayImageEchoEffect(player.dashDuration);
        dashDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir;
        stateTimer = player.dashDuration;
        OriginalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
    }
    public override void Update()
    {
        base.Update();
        DashCancelation();
        player.SetVelocity(player.dashSpeed * dashDir, 0);
        if (stateTimer < 0)
            if (player.isGrounded)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
    }
    public override void Exit()
    {
        base.Exit();

        skillManager.dash.OnEndEffect();
        player.SetVelocity(0, 0);
        rb.gravityScale = OriginalGravityScale;
    }
    private void DashCancelation()
    {
        if (player.wallDetected)
        {
            if (player.isGrounded) stateMachine.ChangeState(player.idleState);
            else stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
