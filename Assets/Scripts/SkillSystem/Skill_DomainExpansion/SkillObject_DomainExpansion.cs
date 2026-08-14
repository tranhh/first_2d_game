using UnityEngine;

public class SkillObject_DomainExpansion : SkillObject_Base
{
    private Skill_DomainExpansion domainManager;
    private Vector3 targetScale;
    private float expandSpeed;
    private float duration;
    private float maxSize;
    private float slowDownPercent;
    private bool isShrinking;

    private void Update()
    {
        HandleScaling();
    }

    public void SetupDomain(Skill_DomainExpansion domainManager)
    {
        this.domainManager = domainManager;
        duration = domainManager.GetDomainDuration();
        slowDownPercent = domainManager.GetDomainSlowDownPercent();
        maxSize = domainManager.GetDomainMaxSize();

        expandSpeed = domainManager.GetExpandSpeed();

        targetScale = Vector3.one * maxSize;

        Invoke(nameof(ShrinkDomain), duration);
    }

    private void HandleScaling()
    {
        float sizeDiff = Mathf.Abs(transform.localScale.x - targetScale.x);
        bool shouldChangeScale = sizeDiff > 0.1f;

        // if the skill's size has not reached its maximum size yet, keep it increasing every frame
        if (shouldChangeScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, expandSpeed * Time.deltaTime);

        //when its size reached max and runs out of skill duration, simply destroy the gameObject
        if (isShrinking && sizeDiff < .1f)
        {
            domainManager.DomainEnd();
            domainManager.ClearTargetList();
            Destroy(gameObject);
        }
    }

    private void ShrinkDomain()
    {
        isShrinking = true;
        targetScale = Vector3.zero;
        domainManager.SetSkillOnCoolDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy == null)
            return;
        domainManager.AddTarget(enemy);
        enemy.ApplySlow(slowDownPercent);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null)
            return;
        domainManager.RemoveTarget(enemy);
    }
}
