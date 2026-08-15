using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Entity entity;
    private Slider healthBar;
    private Entity_VFX entityVFX;
    private Entity_Stats entityStats;

    [SerializeField] protected float currentHealth;
    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;
    public float lastDamageTaken { get; private set; }
    [Header("Resources Regen")]
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenerateHealth = true;


    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(3f, 5f);
    [SerializeField] private float knockbackDuration = .2f;
    [SerializeField] private float heavyKnockbackDuration = .5f;

    [Header("On Heavy Damage")]
    [SerializeField] private float heavyDamageThreshold = .3f; //percentage of health losing in 1 strike to consider dmg as heavy

    protected virtual void Awake()
    {

        entityVFX = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        healthBar = GetComponentInChildren<Slider>();
        entityStats = GetComponent<Entity_Stats>();
    }

    protected virtual void Start()
    {
        SetupHealth();
    }

    private void SetupHealth()
    {
        if (entityStats == null)
            return;

        currentHealth = entityStats.GetMaxHealth();
        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);

    }

    public void HealOverTime(float amount, float duration, float healTickInterval)
    {
        StartCoroutine(HealOverTimeCo(amount, duration, healTickInterval));
    }

    private IEnumerator HealOverTimeCo(float amount, float duration, float healTickInterval)
    {
        float healPerTick = amount / duration * healTickInterval;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Heal(healPerTick);

            elapsed += healTickInterval;
            yield return new WaitForSeconds(healTickInterval);
        }
    }

    public virtual void Heal(float amount)
    {
        if (isDead)
            return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, entityStats.GetMaxHealth());
    }

    public virtual DamageResult TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer, bool isCrit)
    {
        if (isDead || AttackEvaded() || !canTakeDamage)
            return new DamageResult(false, 0, false);

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();

        float armorPenetration = attackerStats != null ? attackerStats.GetAmorPenetration() : 0;
        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorPenetration) : 0;
        float physicalDamageTaken = damage * (1 - mitigation);

        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;
        float elementalDamageTaken = elementalDamage * (1 - resistance);

        float finalDamage = physicalDamageTaken + elementalDamageTaken;

        TakeKnockBack(damageDealer, finalDamage);

        ReduceHealth(finalDamage);
        lastDamageTaken = finalDamage;
        return new DamageResult(true, finalDamage, isCrit);
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    private bool AttackEvaded()
    {
        if (entityStats == null)
            return false;

        return Random.Range(0, 100) < entityStats.GetEvasion();
    }

    private void RegenerateHealth()
    {
        if (!canRegenerateHealth)
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();

        IncreaseHealth(regenAmount);
    }
    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        currentHealth = Mathf.Min(newHealth, maxHealth);
        UpdateHealthBar();
    }

    public void ReduceHealth(float damage)
    {
        entityVFX?.PlayOnDamageVfx();
        currentHealth = currentHealth - damage;
        UpdateHealthBar();
        if (currentHealth <= 0)
            Die();
    }

    public float GetHealthByPercent() => currentHealth / entityStats.GetMaxHealth();

    public void SetHealthToPercent(float percent)
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percent);
        UpdateHealthBar();
    }

    protected virtual void Die()
    {
        isDead = true;
        entity.EntityDeath();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null || entityStats.GetMaxHealth() <= 0)
            return;

        healthBar.value = currentHealth / entityStats.GetMaxHealth();
    }

    private void TakeKnockBack(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);

        entity?.ReceiveKnockback(knockback, duration);
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x = knockback.x * direction;

        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    private bool IsHeavyDamage(float damage) => entityStats != null ? (damage / entityStats.GetMaxHealth() > heavyDamageThreshold) : false;
}
