using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    public event Action OnExplode;
    private SkillShard shard;
    [SerializeField] private GameObject vfxPrefab;
    private Transform target;
    private float speed;

    private void Update()
    {
        if (target == null)
            return;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    public void MoveTowardsClosestTarget(float speed)
    {
        target = ClosestTarget();
        this.speed = speed; // this.speed = field ( class variable), the other is the function's parameter
    }

    public void SetupShard(SkillShard shard)
    {
        this.shard = shard;
        playerStats = shard.player.stats;
        damageScaleData = shard.damageScaleData;
        float detonationTime = shard.GetDetonationTime();
        Invoke(nameof(Explode), detonationTime); // call Explode() after detonationTime.
    }

    public void SetupShard(SkillShard shard, float detonationTime, bool canMove, float shardSpeed)
    {
        this.shard = shard;
        playerStats = shard.player.stats;
        damageScaleData = shard.damageScaleData;
        Invoke(nameof(Explode), detonationTime); // call Explode() after detonationTime.
        if (canMove)
            MoveTowardsClosestTarget(shardSpeed);
    }

    public void Explode()
    {
        DamageEnemiesInRadius(transform, checkRadius);
        SpriteRenderer sprite = Instantiate(vfxPrefab, transform.position, Quaternion.identity).GetComponentInChildren<SpriteRenderer>();
        sprite.color = shard.player.vfx.GetElementColor(usedElement);
        OnExplode?.Invoke();
        Destroy(gameObject);
    }

    //check for collision with target
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;

        Explode();
    }



}
