using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClickableGameObject : MonoBehaviour
{
    protected bool isActive = false;

    protected virtual void Update()
    {
        if (isActive) OnActive();
        else OnInactive();
    }

    protected virtual void FixedUpdate()
    {
        if (isActive) OnActiveFixed();
        else OnInactiveFixed();
    }

    public virtual void ToggleActive()
    {
        isActive = !isActive;
    }
    protected abstract void OnActive();
    protected abstract void OnInactive();

    protected virtual void OnActiveFixed() { }

    protected virtual void OnInactiveFixed() { }
}
