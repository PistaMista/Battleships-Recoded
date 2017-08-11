using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingUIElement : AttackViewUIElement
{
    /// <summary>
    /// The active targeting elements.
    /// </summary>
    public static List<TargetingUIElement> activeTargetingElements;

    public TargetingUIElement[] incompatibleWith;

    public List<object> targets;

    public override void Enable ()
    {
        base.Enable();
        activeTargetingElements = new List<TargetingUIElement>();
        targets = new List<object>();
    }

    public override void Disable ()
    {
        base.Disable();
    }

    protected sealed override bool IsFocusable ()
    {
        for (int i = 0; i < incompatibleWith.Length; i++)
        {
            if (activeTargetingElements.Contains( incompatibleWith[i] ))
            {
                return false;
            }
        }

        return base.IsFocusable();
    }

    protected virtual void AddTarget ( object target )
    {
        if (!targets.Contains( target ))
        {
            targets.Add( target );
            if (!activeTargetingElements.Contains( this ))
            {
                activeTargetingElements.Add( this );
            }
        }
    }

    protected void RemoveTarget ( object target )
    {
        targets.Remove( target );
        if (targets.Count == 0)
        {
            activeTargetingElements.Remove( this );
        }
    }

    protected override void OnFocusedTap ( Vector2 position )
    {
        base.OnFocusedTap( position );
    }

    protected override void OnFocusedBeginPress ( Vector2 position )
    {
        base.OnFocusedBeginPress( position );
    }

    protected override void OnFocusedEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedEndPress( initialPosition, currentPosition );
    }

    protected override void OnFocusedDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedDrag( initialPosition, currentPosition );
    }

    public void OnFireButtonPress ()
    {
        if (targets.Count > 0)
        {
            ConfirmTargeting();
        }
    }

    protected virtual void ConfirmTargeting ()
    {

    }
}
