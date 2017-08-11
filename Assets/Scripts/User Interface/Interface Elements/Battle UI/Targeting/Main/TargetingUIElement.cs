using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class TargetingUIElement : AttackViewUIElement
{
    /// <summary>
    /// The active targeting elements.
    /// </summary>
    public static List<TargetingUIElement> activeTargetingElements;

    public TargetingUIElement[] incompatibleWith;

    public List<TargetMarker> targetMarkers;

    public TargetMarker selectedTargetMarker;


    public override void Enable ()
    {
        base.Enable();
        activeTargetingElements = new List<TargetingUIElement>();
        targetMarkers = new List<TargetMarker>();
    }

    public override void Disable ()
    {
        base.Disable();
        foreach (TargetMarker targetMarker in targetMarkers)
        {
            targetMarker.OnRemove();
        }
        targetMarkers = null;
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

    protected override void Unfocus ()
    {
        base.Unfocus();
        selectedTargetMarker = null;
    }

    protected void AddTarget ( object target )
    {
        if (!targetMarkers.Find( ( TargetMarker obj ) => obj.target == target ))
        {
            TargetMarker targetMarker = AddTargetMarker( target );
            targetMarker.target = target;
            targetMarker.transform.SetParent( transform );
            targetMarker.SetUp();
            targetMarkers.Add( targetMarker );
            if (!activeTargetingElements.Contains( this ))
            {
                activeTargetingElements.Add( this );
            }
        }
    }

    protected virtual TargetMarker AddTargetMarker ( object target )
    {
        return null;
    }

    protected virtual void RemoveTargetMarker ( TargetMarker target )
    {
        target.OnRemove();
        targetMarkers.Remove( target );
        if (targetMarkers.Count == 0)
        {
            activeTargetingElements.Remove( this );
        }
    }

    protected override void OnFocusedTap ( Vector2 position )
    {
        base.OnFocusedTap( position );
        if (selectedTargetMarker != null)
        {
            RemoveTargetMarker( selectedTargetMarker );
        }
    }

    protected override void OnFocusedBeginPress ( Vector2 position )
    {
        base.OnFocusedBeginPress( position );
        foreach (TargetMarker targetMarker in targetMarkers)
        {
            if (targetMarker.PositionIntersects( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y - targetMarker.transform.position.y ) ))
            {
                selectedTargetMarker = targetMarker;
                break;
            }
        }
    }

    protected override void OnFocusedEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedEndPress( initialPosition, currentPosition );
        selectedTargetMarker = null;
    }

    protected override void OnFocusedDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedDrag( initialPosition, currentPosition );
    }

    public void OnFireButtonPress ()
    {
        if (targetMarkers != null)
        {
            if (targetMarkers.Count > 0)
            {
                ConfirmTargeting();
            }
        }

        if (gameObject.activeInHierarchy)
        {
            Disable();
        }
    }

    protected virtual void ConfirmTargeting ()
    {

    }
}
