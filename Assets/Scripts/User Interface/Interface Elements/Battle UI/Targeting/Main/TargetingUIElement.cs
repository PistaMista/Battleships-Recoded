using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TargetingUIElement : AttackViewUIElement
{
    /// <summary>
    /// The active targeting elements.
    /// </summary>
    public static List<TargetingUIElement> activeTargetingElements;

    public TargetingUIElement[] incompatibleWith;

    public List<TargetMarker> targetMarkers;

    public TargetMarker selectedTargetMarker;

    public struct Target
    {
        public object target;
        public bool valid;

        public Target ( object target, bool valid )
        {
            this.target = target;
            this.valid = valid;
        }

        public static bool operator == ( Target a, Target b )
        {
            return a.target == b.target && a.valid == b.valid;
        }

        public static bool operator != ( Target a, Target b )
        {
            return !( a.target == b.target && a.valid == b.valid );
        }

        public override bool Equals ( object obj )
        {
            if (obj != typeof( Target ))
            {
                return false;
            }

            Target tar = (Target)obj;

            return tar.target == target && tar.valid == valid;
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode();
        }
    }



    public override void Enable ()
    {
        base.Enable();
        activeTargetingElements = new List<TargetingUIElement>();
        targetMarkers = new List<TargetMarker>();
    }

    public override void Disable ()
    {
        base.Disable();
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

    protected void AddTarget ( Target target )
    {
        if (!targetMarkers.Find( ( TargetMarker obj ) => obj.Equals( target ) ))
        {
            TargetMarker targetMarker = AddTargetMarker( target );
            targetMarker.target = target;
            targetMarker.transform.SetParent( transform );
            targetMarker.SetVisualsForTarget( target );
            targetMarkers.Add( targetMarker );
            if (!activeTargetingElements.Contains( this ))
            {
                activeTargetingElements.Add( this );
            }
        }
    }

    protected virtual TargetMarker AddTargetMarker ( Target target )
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
                targetMarker.StartMove();
                break;
            }
        }
    }

    protected override void OnFocusedEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedEndPress( initialPosition, currentPosition );
        if (selectedTargetMarker != null)
        {
            selectedTargetMarker.EndMove();
        }
        selectedTargetMarker = null;

        for (int i = 0; i < targetMarkers.Count; i++)
        {
            if (!targetMarkers[i].target.valid)
            {
                RemoveTargetMarker( targetMarkers[i] );
                i--;
            }
        }
    }

    protected override void OnFocusedDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedDrag( initialPosition, currentPosition );
        if (selectedTargetMarker != null)
        {
            Target potentialTarget = CalculateTargetFromScreenPosition( currentPosition );
            selectedTargetMarker.potentialTarget = potentialTarget;
            selectedTargetMarker.Moving();
        }
    }

    protected virtual Target CalculateTargetFromScreenPosition ( Vector2 position )
    {
        return new Target( null, false );
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
