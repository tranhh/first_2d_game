using UnityEngine;

public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer;
    private float lastTimeAttacked;

    private int comboIndex = 1;
    private int comboLimit = 3;
    private bool comboAttackQueued;
    private int attackDir;
    private const int FirstComboIndex = 1;
    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
        {
            Debug.LogWarning("comboLimit changed to match attackVelocity array!");
            comboLimit = player.attackVelocity.Length;
        }
    }
    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIfNeeded();
        SyncAttackSpeed();

        attackDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir;

        anim.SetInteger("BasicAttackIndex", comboIndex);
        GenerateAttackVelocity();
    }
    public override void Update()
    {
        base.Update();
        HandleBasicAttack();
        if (input.Player.Attack.WasPressedThisFrame())
            QueuedNextAttack();

        if (triggerCalled)
            HandleStateExit();

    }
    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;

    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();

        }
        else
            stateMachine.ChangeState(player.idleState);
    }
    private void QueuedNextAttack()
    {
        if (comboIndex < comboLimit) comboAttackQueued = true;
    }
    public void ResetComboIfNeeded()
    {
        if (comboIndex > comboLimit || (Time.time > lastTimeAttacked + player.comboResetTime)) comboIndex = FirstComboIndex;

    }
    private void HandleBasicAttack()
    {
        attackVelocityTimer -= Time.deltaTime;
        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }
    // push player slightly ahead when attack triggered
    private void GenerateAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];
        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }
}
