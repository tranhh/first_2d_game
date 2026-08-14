using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] private GameObject onHitVfx;
    [Space]
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 10f;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected Entity_Stats playerStats;
    protected DamageScaleData damageScaleData;
    protected ElementType usedElement;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected void DamageEnemy(Enemy enemy, float damageMultiplier = 1f)
    {
        IDamageable damageable = enemy.GetComponent<IDamageable>();
        if (damageable == null)
            return;

        AttackData attackData = playerStats.GetAttackData(damageScaleData); //currently setting for normal attack deals 100% of physical damage + extra 20% of elemental damage
        Entity_StatusHandler statusHandler = enemy.GetComponent<Entity_StatusHandler>();

        DamageResult result = damageable.TakeDamage(attackData.physicalDamage * damageMultiplier, attackData.elementalDamage * damageMultiplier, attackData.element, transform, attackData.isCrit);

        if (attackData.element != ElementType.None)
            statusHandler.ApplyStatusEffect(attackData.element, attackData.effectData);

        if (result.hit)
            Instantiate(onHitVfx, transform.position, Quaternion.identity);

        usedElement = attackData.element;
    }

    protected void DamageEnemiesInRadius(Transform t, float radius, float damageMultiplier = 1f)
    {
        foreach (var target in GetEnemiesAround(t, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue;

            AttackData attackData = playerStats.GetAttackData(damageScaleData);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            DamageResult result = damageable.TakeDamage(attackData.physicalDamage * damageMultiplier, attackData.elementalDamage * damageMultiplier, attackData.element, transform, attackData.isCrit);

            if (attackData.element != ElementType.None)
                statusHandler.ApplyStatusEffect(attackData.element, attackData.effectData);

            if (result.hit)
                Instantiate(onHitVfx, transform.position, Quaternion.identity);

            usedElement = attackData.element;
        }
    }

    protected Transform ClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;
        foreach (var enemy in GetEnemiesAround(transform, checkRadius))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                target = enemy.transform;
                closestDistance = distance;
            }
        }
        return target;
    }

    protected Collider2D[] GetEnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);
    }

    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
        {
            targetCheck = transform;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, checkRadius); // control checkRadius in prefab of the skill
    }
}
