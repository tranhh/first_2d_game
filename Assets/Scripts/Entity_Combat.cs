using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public Collider2D[] targetColliders;

    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private LayerMask whatIsTarget;


    private void GetDetectedColliders()
    {
        
    }
    private void OawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);       
 
    }
}
