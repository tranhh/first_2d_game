using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Details")]
    [SerializeField] private float counterRecovery = .25f;
    public bool CounterAttackPerformed()
    {
        bool hasCountered = false;

        foreach (var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;
            if (counterable.canBeCountered)
            {
                counterable.HandleCounter();
                hasCountered = true;
            }
        }
        return hasCountered;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;

}
