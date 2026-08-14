using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();


    // if got hit by a player -> enter battle state

    public override DamageResult TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer, bool isCrit)
    {
        DamageResult result = base.TakeDamage(damage, elementalDamage, element, damageDealer, isCrit);

        if (!result.hit)
            return result;

        if (damageDealer.TryGetComponent<Player>(out _) || damageDealer.TryGetComponent<TimeEcho>(out _))
            enemy.TryEnterBattleState(damageDealer);

        return result;
    }
}
