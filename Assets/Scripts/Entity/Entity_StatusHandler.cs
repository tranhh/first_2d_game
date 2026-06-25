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

    public void ApplyLightningEffect(float resetTimer, float damage)
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
                entity_VFX.lightningStrikeVfx.SetActive(true);
                entity_VFX.lightningAnimator.enabled = true;
                entity_VFX.lightningAnimator.Play("LightningStrike_01", 0, 0f);
                entity_Health.ReduceHp(finalDamage * 0.1f); // first strike deal 10% extra damage
                break;

            case 2:
                entity_VFX.lightningStrikeVfx.SetActive(true);
                entity_VFX.lightningAnimator.enabled = true;
                entity_VFX.lightningAnimator.Play("LightningStrike_02", 0, 0f);
                entity_Health.ReduceHp(finalDamage * 0.2f); // 2nd strike deal 20% extra damage
                break;

            case 3:
                entity_VFX.lightningStrikeVfx.SetActive(true);
                entity_VFX.lightningAnimator.enabled = true;
                entity_VFX.lightningAnimator.Play("LightningStrike_03", 0, 0f);
                entity_Health.ReduceHp(finalDamage * 0.3f); // 3rd strike deal 30% extra damage
                break;

            case 4:
                entity_VFX.lightningStrikeVfx.SetActive(true);
                entity_VFX.lightningAnimator.enabled = true;
                entity_VFX.lightningAnimator.Play("LightningStrike_04", 0, 0f);
                entity_Health.ReduceHp(finalDamage * 1.5f); // last strike deal 150% extra damage
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

    public void ApplyBurnEffect(float duration, float totalDamage)
    {
        if (chilledCo != null)
        {
            StopCoroutine(chilledCo);
            entity.RemoveSlow();
            entity_VFX.StopChillEffectVfx();
            entity.StopSlowDownEntity();
            chilledCo = null;
        }

        float fireRes = entity_Stats.GetElementalResistance(ElementType.Fire);
        float finalDamage = totalDamage * (1 - fireRes);
        damagePerTick += finalDamage;

        burnDuration = duration;
        entity_VFX.StartBurnEffectVfx(burnDuration, ElementType.Fire);

        if (BurnCo != null)
            StopCoroutine(BurnCo);

        BurnCo = StartCoroutine(BurnEffectCo());
    }

    private IEnumerator BurnEffectCo()
    {
        while (burnDuration > 0)
        {
            entity_Health.ReduceHp(damagePerTick);
            Debug.Log(damagePerTick);
            yield return new WaitForSeconds(tickRate);
            burnDuration -= tickRate;
        }

        entity_VFX.StopBurnStatusEffectVfx();

        damagePerTick = 0;
        BurnCo = null;
    }

    public void ApplyChilledEffect(float duration, float slowMultiplier)
    {
        if (BurnCo != null)
        {
            StopCoroutine(BurnCo);
            entity_VFX.StopBurnStatusEffectVfx();
            damagePerTick = 0;
            burnDuration = 0;
            BurnCo = null;
        }

        if (chilledCo != null)
            StopCoroutine(chilledCo);
        chilledCo = StartCoroutine(ChilledEffectCo(duration, slowMultiplier));
    }

    private IEnumerator ChilledEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);

        entity_VFX.StartChillEffectVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);

        entity_VFX.StopChillEffectVfx();
    }

}
