using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] public Animator lightningAnimator;
    private void ReturnToIdle()
    {
        lightningAnimator.Play("Empty", 0, 0f);
    }

}
