using UnityEngine;

public class SkillObject_SwordPierce : SkillObject_Sword
{
    private int enemiesHit = 0;

    public override void SetUpSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetUpSword(swordManager, direction);
    }

    // since base class already have a virtual function, even if the function in derived class doesn't need anything from the virtual function, it still have to be written as override
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground"); // = true if collided with ground


        if (groundHit)
        {
            StopSword(collision);
            return;
        }

        if (collision.TryGetComponent(out Enemy enemy))
        {
            float damageMultiplier = Mathf.Max(0.7f, 1f - enemiesHit * 0.1f); // each enemy hit after the first takes 10% less damage, up to 30% reduced damage.

            DamageEnemy(enemy, damageMultiplier);
            enemiesHit++;
        }
    }
}
