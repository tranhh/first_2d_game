using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;
    public Stat_DefensiveGroup defense;
    public Stat_OffensiveGroup offense;
    public Stat_ResourceGroup resources;
    public Stat_MajorGroup major;

    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;
            case StatType.Strength: return major.strength;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Agility: return major.agility;
            case StatType.Vitality: return major.vitality;
            case StatType.Damage: return offense.damage;
            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritDamage: return offense.critDamage;
            case StatType.ArmorPenetration: return offense.armorPenetration;
            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;
            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;
            case StatType.FireRes: return defense.fireRes;
            case StatType.IceRes: return defense.iceRes;
            case StatType.LightningRes: return defense.lightningRes;

            default:
                Debug.Log($"Stat Type {type} not implemented yet!");
                return null;
        }
    }

    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5; // each vitality point grants 5 hp
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;

        return finalMaxHealth;
    }

    public float GetElementalDamage(out ElementType element)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue(); // 1 intelligence = 1 elemental dmg point

        float highestDamage = fireDamage;
        element = ElementType.Fire;

        if (iceDamage > highestDamage)
        {
            highestDamage = iceDamage;
            element = ElementType.Ice;
        }

        if (lightningDamage > highestDamage)
        {
            highestDamage = lightningDamage;
            element = ElementType.Lightning;
        }
        if (highestDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float finalDamage = highestDamage + bonusElementalDamage;
        return finalDamage;
    }

    public float GetElementalResistance(ElementType element)
    {
        float baseRes = 0;
        float bonusRes = major.intelligence.GetValue() * .5f; // each intelligence point gives 0.5% elemental resistance

        switch (element)
        {
            case ElementType.Fire:
                baseRes = defense.fireRes.GetValue();
                break;
            case ElementType.Ice:
                baseRes = defense.iceRes.GetValue();
                break;
            case ElementType.Lightning:
                baseRes = defense.lightningRes.GetValue();
                break;
        }
        float resistance = baseRes + bonusRes;

        float finalRes = Mathf.Clamp(resistance, 0, 75f) / 100; // cap at 75%

        return finalRes;
    }

    public float GetArmorMitigation(float armorPenetration)
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue(); // 1 vitality = 1 armor point
        float totalArmor = baseArmor + bonusArmor;

        float reductionMultiplier = Mathf.Clamp01(1 - armorPenetration); // short for Clamp(1 - armorPenetration, 0, 1)
        float effectiveArmor = totalArmor * reductionMultiplier;

        float finalArmorMitigation = (effectiveArmor) / (effectiveArmor + 100); // the formula

        return Mathf.Clamp(finalArmorMitigation, 0, .85f); // return 0 or 0.85 if finalArmorMitigation is lower or higher than that
    }

    public float GetAmorPenetration()
    {
        float finalReduction = offense.armorPenetration.GetValue() / 100; // ignore a percentage of armor

        return finalReduction;
    }

    public float GetPhysicalDamage(out bool isCrit)
    {
        float baseDmg = offense.damage.GetValue();
        float bonusDmg = major.strength.GetValue(); // 1 strength point = 1 dmg point
        float totalDmg = baseDmg + bonusDmg;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * .3f; // 1 agility point = 0.3% crit
        float totalCritChance = baseCritChance + bonusCritChance;

        float baseCritDmg = offense.critDamage.GetValue();
        float bonusCritDmg = major.strength.GetValue() * .5f; // 1 strength point = 0.5% crit dmg
        float totalCritDmg = (baseCritDmg + bonusCritDmg) / 100;

        isCrit = Random.Range(0, 100) < totalCritChance;
        float finalDamage = isCrit ? totalDmg * totalCritDmg : totalDmg;

        return finalDamage;
    }
    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * .5f; // each agility point = 0.5% evasion and 0.3% crit chance
        float totalEvasionStat = baseEvasion + bonusEvasion;
        float evasionCap = 85f; // evasion will be capped at 85%

        return Mathf.Clamp(totalEvasionStat, 0, evasionCap); // return totalEvasionStat while preventing it to drop lower than 0 or higher than the cap
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if (defaultStatSetup == null)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }

        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);

        major.vitality.SetBaseValue(defaultStatSetup.vitality);
        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);

        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critDamage.SetBaseValue(defaultStatSetup.critDamage);
        offense.armorPenetration.SetBaseValue(defaultStatSetup.armorPenetration);
        offense.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);
        defense.fireRes.SetBaseValue(defaultStatSetup.fireRes);
        defense.iceRes.SetBaseValue(defaultStatSetup.iceRes);
        defense.lightningRes.SetBaseValue(defaultStatSetup.lightningRes);
    }
}
