using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
    private void OnEnable()
    {
        //subscribe the method to the event ( like giving phone number so they can call when needed, not call rightaway)
        entity.OnFlipped += HandleFlip; // no parentheses bcs it's only be called when the event happens, plus it's a void, no return type.
    }

    private void OnDisable()
    {
        entity.OnFlipped -= HandleFlip;
    }

    private void HandleFlip() => transform.rotation = Quaternion.identity;

}
