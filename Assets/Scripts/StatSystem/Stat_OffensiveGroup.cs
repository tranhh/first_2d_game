using System;
using UnityEngine;

[Serializable]
public class Stat_OffensiveGroup
{
    // base damage
    public Stat damage;
    public Stat critChance;
    public Stat critDamage;
    public Stat armorPenetration;

    // elemental damage
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;
}
