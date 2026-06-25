using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        bool gotHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);
        if (!gotHit)
            return false;

        // 3 ways to detects player once received damage

        // if (damageDealer.CompareTag("Player"))
        //     enemy.TryEnterBattleState(damageDealer);

        // if (damageDealer.GetComponent<Player>() != null)
        //     enemy.TryEnterBattleState(damageDealer);

        // most optimized way: 
        if (damageDealer.TryGetComponent<Player>(out _))
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }
}
