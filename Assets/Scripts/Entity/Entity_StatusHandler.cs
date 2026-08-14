using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_Stats entity_Stats;
    private Entity_Health entity_Health;
    private Entity_VFX entity_VFX;
    private Coroutine chilledCo;
    private Coroutine BurnCo;

    // fire element status effect
    private float damagePerTick;
    private float burnDuration;
    private const float tickRate = 1f;  // number of times burn damage trigger every second

    //lightning element status effect
    private int lightningCombo;
    private Coroutine LightningcomboResetCo;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        entity_VFX = GetComponent<Entity_VFX>();
        entity_Stats = GetComponent<Entity_Stats>();
        entity_Health = GetComponent<Entity_Health>();
    }

    public void RemoveAllNegativeEffects()
    {
        StopBurn();
        StopChill();
    }

    public void ApplyStatusEffect(ElementType element, ElementalEffectData effectData)
    {
        if (element == ElementType.Ice)
            ApplyChilledEffect(effectData.chillDuration, effectData.chillSlowMultiplier);
        if (element == ElementType.Fire)
            ApplyBurnEffect(effectData.burnDuration, effectData.totalBurnDamage);
        if (element == ElementType.Lightning)
            ApplyLightningEffect(effectData.lightningDuration, effectData.lightningDamage);
    }

    private void ApplyLightningEffect(float resetTimer, float damage)
    {
        lightningCombo++;
        if (LightningcomboResetCo != null)
            StopCoroutine(LightningcomboResetCo);

        float lightningRes = entity_Stats.GetElementalResistance(ElementType.Lightning);
        float finalDamage = damage * (1 - lightningRes);

        LightningcomboResetCo = StartCoroutine(LightningComboCo(resetTimer));

        switch (lightningCombo)
        {
            case 1:
                entity_VFX.PlayLightningStrikeCombo(1);
                entity_Health.ReduceHealth(finalDamage * 0.1f); // first strike deal 10% extra damage
                break;

            case 2:
                entity_VFX.PlayLightningStrikeCombo(2);
                entity_Health.ReduceHealth(finalDamage * 0.2f); // 2nd strike deal 20% extra damage
                break;

            case 3:
                entity_VFX.PlayLightningStrikeCombo(3);
                entity_Health.ReduceHealth(finalDamage * 0.3f); // 3rd strike deal 30% extra damage
                break;

            case 4:
                entity_VFX.PlayLightningStrikeCombo(4);
                entity_Health.ReduceHealth(finalDamage * 1.5f); // last strike deal 150% extra damage

                lightningCombo = 0;

                StopCoroutine(LightningcomboResetCo);

                LightningcomboResetCo = null;
                break;
        }
    }

    private IEnumerator LightningComboCo(float resetTimer)
    {
        yield return new WaitForSeconds(resetTimer);
        lightningCombo = 0;
        LightningcomboResetCo = null;
    }

    private void ApplyBurnEffect(float duration, float totalDamage)
    {
        StopChill();

        float fireRes = entity_Stats.GetElementalResistance(ElementType.Fire);
        float finalDamage = totalDamage * (1 - fireRes);
        damagePerTick += finalDamage / 10; // burn deals 10% of total damage for a duration and stack infinitely

        burnDuration = duration;
        entity_VFX.StartBurnEffectVfx(burnDuration, ElementType.Fire);

        if (BurnCo != null)
            StopCoroutine(BurnCo);

        BurnCo = StartCoroutine(BurnEffectCo());
    }

    private void StopChill()
    {
        if (chilledCo != null)
        {
            StopCoroutine(chilledCo);
            entity.RemoveSlow();
            entity_VFX.StopChillEffectVfx();
            chilledCo = null;
        }
    }

    private IEnumerator BurnEffectCo()
    {
        while (burnDuration > 0)
        {
            entity_Health.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickRate);
            burnDuration -= tickRate;
        }

        entity_VFX.StopBurnStatusEffectVfx();

        damagePerTick = 0;
        BurnCo = null;
    }

    private void ApplyChilledEffect(float duration, float slowMultiplier)
    {
        StopBurn();

        if (chilledCo != null)
            StopCoroutine(chilledCo);
        chilledCo = StartCoroutine(ChilledEffectCo(duration, slowMultiplier));
    }

    private void StopBurn()
    {
        if (BurnCo != null)
        {
            StopCoroutine(BurnCo);
            entity_VFX.StopBurnStatusEffectVfx();
            damagePerTick = 0;
            burnDuration = 0;
            BurnCo = null;
        }
    }

    private IEnumerator ChilledEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);

        entity_VFX.StartChillEffectVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);

        entity_VFX.StopChillEffectVfx();
    }

}
