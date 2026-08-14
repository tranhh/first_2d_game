using UnityEngine;

// ------ Decide when and where the skill can be used
public class Skill_TimeEcho : Player_SkillBase
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private GameObject smokeVFX;
    [SerializeField] private float timeEchoDuration;
    [SerializeField] private float cloneStatsMultiplier;

    [Header("Soul Link Upgrades")]
    public float soulLinkHealPercent = .25f;
    public float soulLinkHealDuration = 3f;
    public float soulLinkHealInterval = .25f;
    public float wispMoveSpeed = 5f;


    private Vector3 spawnPosition;

    public float GetTimeEchoDuration()
    {

        return timeEchoDuration;
    }

    public override void TryUseSkill()
    {
        if (!CanUseSkill())
            return;

        player.CastTimeEchoSkill();
        // and then set skill onCooldown from animation event
    }

    public bool CanRemoveNegativeEffects() => UnlockedSkill(SkillUpgradeType.timeEcho_SoulPurge);

    public bool CanCreateWisp() => UnlockedSkill(SkillUpgradeType.timeEcho_SoulLink) || UnlockedSkill(SkillUpgradeType.timeEcho_SoulPurge);

    public void CreateTimeEcho()
    {
        spawnPosition = player.transform.position + Vector3.right * player.facingDir * 2f;

        CreateSmokeVfx();

        Invoke(nameof(CreateClone), .1f);
    }

    public void CreateTimeEcho(Vector3 position)
    {
        spawnPosition = position;

        CreateSmokeVfx();

        Invoke(nameof(CreateClone), .1f);
    }

    private void CreateClone()
    {
        GameObject timeEcho = Instantiate(timeEchoPrefab, spawnPosition, Quaternion.identity);
        timeEcho.GetComponent<TimeEcho>().SetupEcho(this, cloneStatsMultiplier, upgradeType, GetTimeEchoDuration());
    }

    private void CreateSmokeVfx() => Instantiate(smokeVFX, spawnPosition, Quaternion.identity);

}
