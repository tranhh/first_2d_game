using UnityEngine;

public class ElementalEffectData
{
    public float chillDuration;
    public float chillSlowMultiplier;
    public float burnDuration;
    public float totalBurnDamage;
    public float lightningDamage;
    public float lightningDuration;

    public ElementalEffectData(Entity_Stats stats, DamageScaleData damageScale)
    {
        chillDuration = damageScale.chillDuration;
        chillSlowMultiplier = damageScale.chillSlowMultiplier;

        burnDuration = damageScale.burnDuration;
        totalBurnDamage = stats.offense.fireDamage.GetValue() * damageScale.burnDamageScale;

        lightningDamage = stats.offense.lightningDamage.GetValue() * damageScale.lightningDamageScale;
        lightningDuration = damageScale.lightningDuration;

    }

}

