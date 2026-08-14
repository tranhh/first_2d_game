using UnityEngine;

public enum SkillUpgradeType
{
    None,
    // ------- Dash Tree --------
    Dash,
    Dash_CloneOnStart, // create a clone when dash starts
    Dash_CloneOnStartAndArrival,// create a clone when dash starts and ends
    Dash_ShardOnStart, // create a shard when dash starts
    Dash_ShardOnStartAndArrival, // create a shard when dash starts and ends

    // ------ Shard Tree --------
    Shard,
    Shard_MoveToEnemy, // auto find a nearest enemy and move forward to it
    Shard_Multicast, // stacking charges, cast all in a raw
    Shard_Teleport,   // swap places with the last shard created
    Shard_TeleportAndHpRewind, // when swapped place with the last shard, also return Hp as it was when you created the shard

    // ------ Sword Tree -------
    SwordThrow,
    SwordThrow_Spin,
    SwordThrow_Pierce,
    SwordThrow_Bounce,

    // ------ Time Echo -------
    timeEcho_EchoCast,   // create a clone of the player. It can take damage from enemies
    timeEcho_SoulManifestation,    // chases and attacks enemies
    timeEcho_SoulLink, // onDestroyed, create a wisp that follows and heals the player for a percentage of damage dealt to enemies
    timeEcho_SoulPurge, // the wisp also cleanses the player
    timeEcho_SoulBound, // while the clone exists, 50% of the damage the player take will be transfered to the clone instead
    timeEcho_Resonance, // the clone can apply onHit effect to enemies

    // ------ Domain Expansion -------
    Domain_SlowingDown, // create an area in which greatly slows down enemies inside, while you can freely attack and move
    Domain_EchoSpam, // You can no longer move while casting the skill, but instead call out 3 TimeEcho at a time, they can also attack and move freely inside the domain
    Domain_ShardSpam, // You still can't move while casting, but relentlessly casting shard skill every 0.5s during the duration

}
