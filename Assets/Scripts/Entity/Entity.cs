using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped;
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Entity_Stats stats { get; private set; }
    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;

    [SerializeField] protected float baseMoveSpeed;
    [SerializeField] protected float speedMultiplier = 1;
    private float currentSlowMultiplier;


    protected StateMachine stateMachine;

    [Header("Collision detection")]
    [SerializeField] protected LayerMask whatIsStandable;
    public LayerMask WhatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;

    public bool isGrounded { get; private set; }
    public bool wallDetected { get; private set; }

    //condition variables
    private bool isKnocked;
    private Coroutine knockbackCo;
    private Coroutine slowDownCo;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity_Stats>();

        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.updateActiveState();
    }

    public virtual void EntityDeath()
    {

    }

    public void ApplySlow(float slowMultiplier)
    {
        speedMultiplier = 1 - slowMultiplier;
        anim.speed = speedMultiplier;
    }

    public void RemoveSlow()
    {
        speedMultiplier = 1;
        anim.speed = 1;
        currentSlowMultiplier = 0;
        StopSlowDownEntity();
    }

    public virtual void SlowDownEntity(float duration, float slowMultiplier)
    {
        if (currentSlowMultiplier > slowMultiplier)
            return;

        if (slowDownCo != null)
            StopCoroutine(slowDownCo);

        currentSlowMultiplier = slowMultiplier;

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
    }

    public virtual void StopSlowDownEntity()
    {
        if (slowDownCo != null)
        {
            StopCoroutine(slowDownCo);
            slowDownCo = null;
        }
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        yield return null;
    }

    public void ReceiveKnockback(Vector2 knockback, float knockbackDuration)
    {
        if (!InterruptibleAction())
            return;

        if (knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, knockbackDuration));
    }

    public virtual bool InterruptibleAction()
    {
        return true;
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float knockbackDuration)
    {
        isKnocked = true;

        OnKnockbackStart();
        // set velocity to knockback velocity
        rb.linearVelocity = knockback;
        yield return new WaitForSeconds(knockbackDuration);

        CancelKnockback();
    }

    public void CancelKnockback()
    {
        if (knockbackCo != null)
        {
            StopCoroutine(knockbackCo);
            knockbackCo = null;
        }

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;

        OnKnockbackEnd();
    }

    public virtual void OnKnockbackStart()
    {
    }

    public virtual void OnKnockbackEnd()
    {
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false) Flip();
        else if (xVelocity < 0 && facingRight) Flip();
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = -facingDir;

        OnFlipped?.Invoke(); // calls all methods from every scripts that subscribed to OnFlipped
    }

    public int GetFacingDir()
    {
        return facingRight ? 1 : -1;
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    private void HandleCollisionDetection()
    {
        isGrounded = Physics2D.Raycast(GroundCheck.position, Vector2.down, groundCheckDistance, whatIsStandable);
        if (secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, WhatIsGround)
                        && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, WhatIsGround);
        }
        else
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, WhatIsGround);

    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(GroundCheck.position, GroundCheck.position + new Vector3(0, -groundCheckDistance));

        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

        if (secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));

    }
}
