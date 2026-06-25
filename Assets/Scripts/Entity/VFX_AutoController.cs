using UnityEngine;

public class VFX_AutoController : MonoBehaviour
{
    [SerializeField] protected bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1;

    [Header("Random Rotation")]
    [SerializeField] private float minRotation = 0;
    [SerializeField] private float maxRotaion = 360;

    [Header("Random position")]
    [SerializeField] private bool randomOffset = true;
    [SerializeField] private bool randomRotation = true;
    [SerializeField] private float xMinOffset = -.3f;
    [SerializeField] private float xMaxOffset = .3f;
    [Space]
    [SerializeField] private float yMinOffset = -.3f;
    [SerializeField] private float yMaxOffset = .3f;


    private void Start()
    {
        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy)
            Destroy(gameObject, destroyDelay);
    }

    private void ApplyRandomOffset()
    {
        if (!randomOffset)
            return;

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);

        transform.position = transform.position + new Vector3(xOffset, yOffset); // or just: transform.position += new Vector3(xOffset, yOffset);
    }

    private void ApplyRandomRotation()
    {
        if (!randomRotation)
            return;
        float zRotation = Random.Range(minRotation, maxRotaion);
        transform.Rotate(0, 0, zRotation);

    }
}
