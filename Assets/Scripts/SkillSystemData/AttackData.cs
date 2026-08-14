using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float physicalDamage;
    public float elementalDamage;
    public bool isCrit;
    public ElementType element;
    public ElementalEffectData effectData;

    public AttackData(Entity_Stats stats, DamageScaleData scaleData)
    {
        physicalDamage = stats.GetPhysicalDamage(out isCrit) * scaleData.physical;
        elementalDamage = stats.GetElementalDamage(out element) * scaleData.elemental;

        effectData = new ElementalEffectData(stats, scaleData);
    }
}
