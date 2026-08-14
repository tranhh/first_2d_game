using UnityEngine;

public class SkillObject_SwordSpin : SkillObject_Sword
{
    private int maxDistance;
    private float attackFrequence; // number of times damage trigger per second
    private float attackTimer;

    protected override void Update()
    {
        HandleAttack();
        HandleStopping();
        HandleComeback();
    }

    public override void SetUpSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetUpSword(swordManager, direction);

        anim?.SetTrigger("spin");
        maxDistance = swordManager.maxDistance;
        attackFrequence = swordManager.attackFrequence;
        Invoke(nameof(GetSwordBack), swordManager.maxSpinDuration); // call GetSwordBack() after maxSpinDuration
    }

    private void HandleStopping()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistance && rb.simulated == true)
            rb.simulated = false;
    }
    private void HandleAttack()
    {
        // only trigger if collided with sth
        if (rb.simulated)
            return;
        attackTimer -= Time.deltaTime;
        if (attackTimer < 0)
        {
            DamageEnemiesInRadius(transform, 1, 0.3f); // each spin deal 30% damage
            attackTimer = 1 / attackFrequence;
        }
    }

    // since base class already have a virtual function, even if the function in derived class doesn't need anything from the virtual function, it still have to be written as override
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        rb.simulated = false;
    }
}
