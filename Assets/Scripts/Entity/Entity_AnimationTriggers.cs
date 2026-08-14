using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;
    private Entity_Stats stats;
    protected Player_SkillManager skillManager;


    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
        stats = GetComponentInParent<Entity_Stats>();
        skillManager = GetComponentInParent<Player_SkillManager>();
    }

    //called by unity animation events
    private void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger();
    }

    private void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }

    private void AttackTriggerPlus()
    {
        entityCombat.PerformAttack(1.5f);
    }

}
