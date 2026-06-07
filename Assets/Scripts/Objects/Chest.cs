using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX fx => GetComponentInChildren<Entity_VFX>();

    [Header("Open Details")]
    [SerializeField] private Vector2 knockback;
    public void TakeDamage(float damage, Transform damageDealer)
    {
        fx.PlayOnDamageVfx();

        anim.SetBool("chestOpen", true);
        rb.linearVelocity = knockback; // make the chest goes up a little bit
        rb.angularVelocity = Random.Range(-200f, 200f);

        //Drop items
    }
}
