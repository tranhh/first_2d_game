using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();

    public override void TakeDamage(float damage, Transform damageDealer)
    {
        base.TakeDamage(damage, damageDealer);

        if (isDead)
            return;

        // 3 ways:

        // if (damageDealer.CompareTag("Player"))
        //     enemy.TryEnterBattleState(damageDealer);

        // if (damageDealer.GetComponent<Player>() != null)
        //     enemy.TryEnterBattleState(damageDealer);

        // most optimized way: 
        if (damageDealer.TryGetComponent<Player>(out _))
            enemy.TryEnterBattleState(damageDealer);
    }
}
