using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void PlayEffect(int combo)
    {
        anim.Play($"LightningStrike_{combo:00}", 0, 0f);
    }

    // Call in Animation Event
    public void DestroySelf()
    {
        Destroy(transform.root.gameObject);
    }
}
