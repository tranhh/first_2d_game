using System.Collections;
using UnityEngine;

public class SkillShard : Player_SkillBase
{
    private Entity_Health playerHealth;
    public SkillObject_Shard currentShard;

    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private float detonationTime = 2f;

    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 7;
    public float GetShardSpeed() => shardSpeed;

    [Header("Multicast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges;
    [SerializeField] private bool isRecharging;

    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float existDuration = 6;

    [Header("Health Rewind Shard Upgrade")]
    [SerializeField] private float savedHealthPercent;

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
        playerHealth = GetComponentInParent<Entity_Health>();
    }

    public override void TryUseSkill()
    {
        base.TryUseSkill();

        // if set this at base function, base function return exit, but override function continue execute
        if (!CanUseSkill())
            return;

        if (UnlockedSkill(SkillUpgradeType.Shard))
            HandleShardRegular();

        if (UnlockedSkill(SkillUpgradeType.Shard_MoveToEnemy))
            HandleShardMoving();

        if (UnlockedSkill(SkillUpgradeType.Shard_Multicast))
            HandleShardMulticast();

        if (UnlockedSkill(SkillUpgradeType.Shard_Teleport))
            HandleShardTeleport();

        if (UnlockedSkill(SkillUpgradeType.Shard_TeleportAndHpRewind))
            HandleShardRewind();
    }

    private void HandleShardRewind()
    {
        if (currentShard == null)
        {
            CreateShard();
            savedHealthPercent = playerHealth.GetHealthByPercent();
        }
        else
        {
            SwapPlayerAndShard();
            playerHealth.SetHealthToPercent(savedHealthPercent);
            SetSkillOnCoolDown();
        }
    }

    private void HandleShardTeleport()
    {
        if (currentShard == null)
            CreateShard();
        else
        {
            SwapPlayerAndShard();
            SetSkillOnCoolDown();
        }
    }

    private void SwapPlayerAndShard()
    {
        Vector3 shardPosition = currentShard.transform.position;
        Vector3 playerPosition = player.transform.position;
        currentShard.transform.position = playerPosition;
        currentShard.Explode();
        player.TeleportPlayer(shardPosition);
    }

    private void HandleShardMulticast()
    {
        if (currentCharges <= 0)
            return;
        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);
        currentCharges--;

        if (!isRecharging)
        {
            isRecharging = true;
            StartCoroutine(ShardRechargeCo());
        }
    }

    private IEnumerator ShardRechargeCo()
    {
        while (currentCharges < maxCharges)
        {
            yield return new WaitForSeconds(coolDown);
            currentCharges++;
        }
        isRecharging = false;
    }

    public void HandleShardMoving()
    {
        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);
        SetSkillOnCoolDown();
    }

    private void HandleShardRegular()
    {
        CreateShard();
        SetSkillOnCoolDown();
    }

    public void CreateShard()
    {
        if (upgradeType == SkillUpgradeType.None)
            return;

        float detonateTime = GetDetonationTime();
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetupShard(this); // explode the shard after detonateTime

        // after detonateTime, set the skill on cooldown (basically set onCooldown when the shard explode, not from the moment it's created)
        if (UnlockedSkill(SkillUpgradeType.Shard_Teleport) || UnlockedSkill(SkillUpgradeType.Shard_TeleportAndHpRewind))
            currentShard.OnExplode += ForceCooldown;
    }

    public void CreateRawShard()
    {
        bool canMove = UnlockedSkill(SkillUpgradeType.Shard_MoveToEnemy) || UnlockedSkill(SkillUpgradeType.Shard_Multicast);
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        shard.GetComponent<SkillObject_Shard>().SetupShard(this, detonationTime, canMove, shardSpeed);
    }

    private void ForceCooldown()
    {
        if (!OnCoolDown())
        {
            currentShard.OnExplode -= ForceCooldown;
        }
        SetSkillOnCoolDown();
    }

    public float GetDetonationTime()
    {
        if (UnlockedSkill(SkillUpgradeType.Shard_Teleport) || UnlockedSkill(SkillUpgradeType.Shard_TeleportAndHpRewind))
            return existDuration;
        return detonationTime;
    }


}
