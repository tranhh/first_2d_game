using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_Stats stats;
    private Entity_VFX vfx;

    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1f;
    [SerializeField] private LayerMask whatIsTarget;


    [Header("Status effect details")]
    [SerializeField] private float chillDuration = 3f;
    [SerializeField] private float chillSlowMultiplier = .2f;
    [SerializeField] private float burnDuration = 5f;
    [SerializeField] private float lightningResetTimer = 3f;

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack(float damageMultiplier = 1f)
    {
        foreach (var target in GetDetectedColliders())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
                continue;

            float damage = stats.GetPhysicalDamage(out bool isCrit) * damageMultiplier;
            float elementalDamage = stats.GetElementalDamage(out ElementType element) * damageMultiplier;
            bool targetGotHit = damageable.TakeDamage(damage, elementalDamage, element, transform);

            if (element != ElementType.None)
                ApplyStatusEffect(target.transform, element);

            if (targetGotHit)
            {
                vfx.UpdateOnHitColor(element);
                vfx.CreateOnHitVFX(target.transform, isCrit);
            }
        }
    }

    public void ApplyStatusEffect(Transform target, ElementType element)
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
        if (statusHandler == null)
            return;

        if (element == ElementType.Ice)
            statusHandler.ApplyChilledEffect(chillDuration, chillSlowMultiplier);

        if (element == ElementType.Fire)
        {
            float fireDamage = stats.offense.fireDamage.GetValue();
            float burnTick = fireDamage / 10; // burn takes 10% of damage taken for a duration and stack infinitely
            statusHandler.ApplyBurnEffect(burnDuration, burnTick);
        }
        if (element == ElementType.Lightning)
        {
            float lightningDamage = stats.offense.lightningDamage.GetValue();
            statusHandler.ApplyLightningEffect(lightningResetTimer, lightningDamage);
        }
    }

    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);

    }
}
