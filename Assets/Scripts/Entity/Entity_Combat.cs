using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_Stats stats;
    private Entity_VFX vfx;
    public DamageScaleData basicAttackScale;
    public event Action<DamageResult> OnDamageDealt;

    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1f;
    [SerializeField] private LayerMask whatIsTarget;

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();

    }

    public void PerformAttack(float damageMultiplier = 1f)
    {
        //get AttackData outside of foreach loop, so that if a hit would crit, it would apply crit to all enemies got hit by that attack
        AttackData attackData = stats.GetAttackData(basicAttackScale); //currently setting for normal attack deals 100% of physical damage + extra 20% of elemental damage

        foreach (var target in GetDetectedColliders())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
                continue;

            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            DamageResult result = damageable.TakeDamage(attackData.physicalDamage * damageMultiplier, attackData.elementalDamage * damageMultiplier, attackData.element, transform, attackData.isCrit);
            if (attackData.element != ElementType.None)
                statusHandler.ApplyStatusEffect(attackData.element, attackData.effectData);

            if (result.hit)
            {
                OnDamageDealt?.Invoke(result);
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit, attackData.element);
            }
        }
    }

    private void HandleTimeEchoLifeSteal(DamageResult result)
    {

    }

    public Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);

    }
}
