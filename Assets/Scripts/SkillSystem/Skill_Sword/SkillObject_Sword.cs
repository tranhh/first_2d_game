using UnityEngine;

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_SwordThrow swordManager;
    protected Transform playerTransform;
    protected bool shouldComeback;
    protected float maxAllowedDistance = 40;


    protected virtual void Update()
    {
        HandleSwordPoint();
        HandleComeback();
    }

    public virtual void SetUpSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        rb.linearVelocity = direction;

        // just passing SkillObject_Base variables with the values of Player_SkillBase's variables values
        this.swordManager = swordManager;

        playerTransform = swordManager.transform.root;
        playerStats = swordManager.player.stats;
        damageScaleData = swordManager.damageScaleData;
    }

    public void GetSwordBack()
    {
        shouldComeback = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }


    protected void HandleComeback()
    {
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        float comebackSpeed = GetComebackSpeed();
        if (distance > maxAllowedDistance)
            Destroy(gameObject);
        if (!shouldComeback)
            return;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, comebackSpeed * Time.deltaTime);
        if (distance < .5f)
            Destroy(gameObject);
    }

    private float GetComebackSpeed() => swordManager.throwPower * 10;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        StopSword(collision);
        DamageEnemiesInRadius(transform, 1);
    }

    private void HandleSwordPoint()
    {
        if (shouldComeback)
            transform.right = playerTransform.position - transform.position;

        else if (rb.linearVelocity.magnitude > 0.001f) // velocity of the sword in vector 2
            transform.right = rb.linearVelocity;

    }

    protected void StopSword(Collider2D collision)
    {
        rb.simulated = false;
        transform.parent = collision.transform; // make the sword stick to the object it attached to (like move with the enemy if it hits an enemy)
    }
}
