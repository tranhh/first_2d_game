using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX fx => GetComponentInChildren<Entity_VFX>();

    [Header("Open Details")]
    [SerializeField] private Vector2 knockback;
    private bool isOpened;

    public DamageResult TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer, bool isCrit)
    {
        if (isOpened)
            return new DamageResult(false, 0, isCrit);
        isOpened = true;
        anim.SetBool("chestOpen", true);

        fx.PlayOnDamageVfx();

        rb.linearVelocity = knockback; // make the chest goes up a little bit
        rb.angularVelocity = Random.Range(-200f, 200f);

        //Drop items
        return new DamageResult(true, 0, isCrit);
    }
}
