using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        if (enemy.isGrounded == false || enemy.wallDetected)
            enemy.Flip();
    }
    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(enemy.MoveSpeed * enemy.facingDir, rb.linearVelocity.y);

        if (enemy.isGrounded == false || enemy.wallDetected)
            stateMachine.ChangeState(enemy.idleState);
    }
}
