using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DomainExpansion : Player_SkillBase
{
    private GameObject activeDomain;

    [Header("Domain Expansion Details")]
    [SerializeField] private GameObject domainPrefab;
    [SerializeField] private float maxDomainSize = 20f;
    [SerializeField] private float expandSpeed = 4f;

    [Header("Slowing Down Upgrades")]
    [SerializeField] private float slowDownPercent = 0.6f;
    [SerializeField] private float domainDuration = 5f;

    [Header("Domain Upgrades")]
    private List<Enemy> trappedEnemies = new List<Enemy>();


    public float GetDomainDuration() => domainDuration;
    public float GetDomainSlowDownPercent() => slowDownPercent;
    public float GetDomainMaxSize() => maxDomainSize;
    public float GetExpandSpeed() => expandSpeed;

    public override void TryUseSkill()
    {
        if (!CanUseSkill() || !player.CanChangeState() || activeDomain != null)
            return;
        if (InstantDomain())
            CreateDomain();
        else
            player.CastDomainExpansionSkill();
    }

    public void SpamCasting(Enemy enemy)
    {
        if (InstantDomain())
            return;

        SpamTimeEcho(enemy);
        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
            SpamShard();

    }

    private void SpamShard()
    {
        StartCoroutine(SpamShardCo());
    }

    private IEnumerator SpamShardCo()
    {
        float timer = 0f;
        float tickInterval = .5f;
        while (timer < domainDuration)
        {
            skillManager.shard.CreateRawShard();

            yield return new WaitForSeconds(tickInterval);

            timer += tickInterval;
        }
    }

    private void SpamTimeEcho(Enemy enemy) => StartCoroutine(SpawnTimeEchoCo(enemy));


    private IEnumerator SpawnTimeEchoCo(Enemy enemy)
    {
        Vector3 spawnPosition1 = enemy.transform.position + Vector3.right * enemy.facingDir * 2f;
        Vector3 spawnPosition2 = enemy.transform.position + Vector3.left * enemy.facingDir * 2f;
        Vector3 spawnPosition3 = enemy.transform.position + Vector3.right * enemy.facingDir * 3f;

        skillManager.timeEcho.CreateTimeEcho(spawnPosition1);

        yield return new WaitForSeconds(.25f);

        skillManager.timeEcho.CreateTimeEcho(spawnPosition2);

        yield return new WaitForSeconds(.25f);

        skillManager.timeEcho.CreateTimeEcho(spawnPosition3);
    }

    public bool InstantDomain() => !UnlockedSkill(SkillUpgradeType.Domain_EchoSpam) && !UnlockedSkill(SkillUpgradeType.Domain_ShardSpam);

    public void DomainEnd() => activeDomain = null;

    public void CreateDomain()
    {
        activeDomain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        activeDomain.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);
    }

    public void AddTarget(Enemy enemy)
    {
        if (!trappedEnemies.Contains(enemy))
        {
            trappedEnemies.Add(enemy);
            SpamCasting(enemy);
        }
    }

    public void RemoveTarget(Enemy enemy)
    {
        if (trappedEnemies.Contains(enemy))
        {
            enemy.RemoveSlow();
            trappedEnemies.Remove(enemy);
        }
    }

    public void ClearTargetList()
    {
        foreach (Enemy enemy in trappedEnemies)
        {
            if (enemy != null)
                enemy.RemoveSlow();
        }
        trappedEnemies.Clear();
    }
}
