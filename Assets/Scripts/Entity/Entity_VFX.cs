using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity entity;
    private Material originalMaterial;

    [Header("On Damage Hit")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .25f;
    [SerializeField] private Color hitVfxColor = Color.white;
    private Coroutine onDamageVfxCoroutine;

    [Header("On Damaging VFX")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("On Elemental Damage")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] public GameObject lightningStrikeVfx;
    [SerializeField] public Animator lightningAnimator;

    private Color originalColor;
    private Color originalHitVfxColor;
    private Coroutine burnVfxCo;
    private Coroutine chillVfxCo;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        entity = GetComponent<Entity>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
        originalColor = sr.color;
    }

    public void StartChillEffectVfx(float duration, ElementType element)
    {
        if (element != ElementType.Ice)
            return;

        if (chillVfxCo != null)
            StopCoroutine(chillVfxCo);

        chillVfxCo = StartCoroutine(PlayStatusVfxCo(duration, chillVfx));

    }

    public void StopChillEffectVfx()
    {
        if (chillVfxCo != null)
        {
            StopCoroutine(chillVfxCo);
            chillVfxCo = null;
        }

        sr.color = originalColor;
    }

    public void StartBurnEffectVfx(float duration, ElementType element)
    {
        if (element != ElementType.Fire)
            return;

        if (burnVfxCo != null)
            StopCoroutine(burnVfxCo);

        burnVfxCo = StartCoroutine(PlayStatusVfxCo(duration, burnVfx));

    }

    public void StopBurnStatusEffectVfx()
    {
        if (burnVfxCo != null)
        {
            StopCoroutine(burnVfxCo);
            burnVfxCo = null;
        }

        sr.color = originalColor;
    }

    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = .25f;
        float timer = 0;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * .8f;

        bool toggle = false;

        while (timer < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timer += tickInterval;
        }

        sr.color = originalColor;
    }

    public void UpdateOnHitColor(ElementType element)
    {
        if (element == ElementType.Ice)
            hitVfxColor = chillVfx;
        if (element == ElementType.None)
            hitVfxColor = originalHitVfxColor;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);

        vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;

        if (entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);
    }

    // call this method everytime entities take damage
    public void PlayOnDamageVfx()
    {
        //make sure it only trigger once
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;

        yield return new WaitForSeconds(onDamageVfxDuration);

        sr.material = originalMaterial;
    }
}
