using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Player_Skill_Dash dash { get; private set; }
    public SkillShard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get; private set; }
    public Skill_TimeEcho timeEcho { get; private set; }

    public Skill_DomainExpansion domainExpansion { get; private set; }

    private Player_SkillBase[] allSkills;

    private void Awake()
    {
        dash = GetComponentInChildren<Player_Skill_Dash>();
        shard = GetComponentInChildren<SkillShard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
        timeEcho = GetComponentInChildren<Skill_TimeEcho>();
        domainExpansion = GetComponentInChildren<Skill_DomainExpansion>();

        allSkills = GetComponentsInChildren<Player_SkillBase>();
    }

    public void ReduceAllSkillsCooldownBy(float amount)
    {
        foreach (var skill in allSkills)
            skill.ReduceCoolDownBy(amount);
    }

    public Player_SkillBase GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash: return dash;

            case SkillType.TimeShard: return shard;

            case SkillType.SwordThrow: return swordThrow;

            case SkillType.TimeEcho: return timeEcho;

            case SkillType.DomainExpansion: return domainExpansion;

            default:
                Debug.Log($"Skill type {type} is not implemented yet");
                return null;
        }
    }
}
