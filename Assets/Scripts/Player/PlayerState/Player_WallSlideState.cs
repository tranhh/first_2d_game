using UnityEditor.Tilemaps;
using UnityEngine;

public class Player_WallSlideState : PlayerState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        HandleWallSlide();

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.wallJumpState);

        if (!player.wallDetected)
        {
            stateMachine.ChangeState(player.fallState);
            player.Flip();
        }

        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);

            // if (player.facingDir != player.moveInput.x)
            player.Flip();
        }
    }
    public override void Exit()
    {
        base.Exit();

    }

    private void HandleWallSlide()
    {
        if (player.moveInput.y < 0)
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
        else
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.wallSlideSlowMultiplier);
    }
}
