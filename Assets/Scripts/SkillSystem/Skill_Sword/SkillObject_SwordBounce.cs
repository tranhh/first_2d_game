using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] private float bounceSpeed = 10;
    private int bounceCount;
    private Collider2D[] enemyTargets;
    private Transform nextTarget;
    private List<Transform> selectedBefore = new List<Transform>();

    public override void SetUpSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        anim.SetTrigger("spin");
        base.SetUpSword(swordManager, direction);

        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;
    }

    protected override void Update()
    {
        HandleComeback();
        HandleBounce();
    }

    private void HandleBounce()
    {
        if (nextTarget == null)
            return;

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) <= .2f)
        {
            if (nextTarget.TryGetComponent(out Enemy enemy))
            {
                DamageEnemy(enemy);
            }
            BounceToNextTarget();

        }

        if (bounceCount == 0 || nextTarget == null)
        {
            nextTarget = null;
            GetSwordBack();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Enemy enemy))
        {
            GetSwordBack();
            return;
        }
        if (enemyTargets == null)
        {
            enemyTargets = GetEnemiesAround(transform, 8);
            selectedBefore.Clear();
            rb.simulated = false;
        }

        DamageEnemy(enemy);
        selectedBefore.Add(enemy.transform);
        if (enemyTargets.Length <= 1 || bounceCount == 0)
            GetSwordBack();
        else
        {
            nextTarget = GetNextTarget();
        }
    }

    private void BounceToNextTarget()
    {
        enemyTargets = GetEnemiesAround(transform, 8);
        nextTarget = GetNextTarget();
        bounceCount--;
    }

    private Transform GetNextTarget()
    {
        List<Transform> validTarget = GetValidTargets();

        int randomIndex = Random.Range(0, validTarget.Count);
        Transform nextTarget = validTarget[randomIndex];
        selectedBefore.Add(nextTarget);

        return nextTarget;
    }

    private List<Transform> GetValidTargets()
    {
        List<Transform> validTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();
        foreach (var enemy in aliveTargets)
        {
            if (selectedBefore.Contains(enemy.transform) == false)
                validTargets.Add(enemy.transform);
        }

        selectedBefore.Clear();

        if (validTargets.Count > 0)
            return validTargets;
        else
            return aliveTargets;
    }

    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new List<Transform>();

        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
                aliveTargets.Add(enemy.transform);
        }
        return aliveTargets;
    }
}
