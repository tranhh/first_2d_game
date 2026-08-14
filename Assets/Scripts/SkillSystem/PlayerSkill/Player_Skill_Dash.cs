using UnityEngine;

public class Player_Skill_Dash : Player_SkillBase
{
    public void OnStartEffect()
    {
        if (UnlockedSkill(SkillUpgradeType.Dash_CloneOnStart) || UnlockedSkill(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();

        if (UnlockedSkill(SkillUpgradeType.Dash_ShardOnStart) || UnlockedSkill(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    public void OnEndEffect()
    {

    }

    private void CreateShard()
    {
        skillManager.shard.CreateRawShard();
    }

    private void CreateClone()
    {
        Debug.Log("Clone created or sth...");
    }
}
