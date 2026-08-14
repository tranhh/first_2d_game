using UnityEngine;

public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skillManager;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;
        anim = player.anim;
        rb = player.rb;
        input = player.input;
        stats = player.stats;
        skillManager = player.skillManager;
    }
    public override void Update()
    {
        base.Update();
        if (input.Player.Dash.WasPressedThisFrame() && CanDash() && player.CanChangeState())
        {
            skillManager.dash.SetSkillOnCoolDown();
            stateMachine.ChangeState(player.dashState);
        }

        if (input.Player.RangeAttack.WasPressedThisFrame() && skillManager.swordThrow.CanUseSkill() && player.CanChangeState())
            stateMachine.ChangeState(player.swordThrowState);

        if (input.Player.Skill_E.WasPressedThisFrame())
            skillManager.timeEcho.TryUseSkill();

        if (input.Player.UltimateSkill.WasPressedThisFrame())
            skillManager.domainExpansion.TryUseSkill();
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    private bool CanDash() => !player.wallDetected && skillManager.dash.CanUseSkill();

}
