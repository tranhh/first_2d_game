using UnityEngine;

public interface IDamageable
{
    DamageResult TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer, bool isCrit);
}

public class DamageResult
{
    public bool hit;
    public float damageDealt;
    public bool isCrit;

    public DamageResult(bool hit, float damageDealt, bool isCrit)
    {
        this.hit = hit;
        this.damageDealt = damageDealt;
        this.isCrit = isCrit;
    }
}
