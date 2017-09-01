using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackViewUIElement : UIElement
{
    /// <summary>
    /// The focused element.
    /// </summary>
    public static AttackViewUIElement focusedElement;

    protected Player viewedPlayer;

    public override void Enable ()
    {
        base.Enable();
        viewedPlayer = UserInterface.managedBattle.selectedPlayer;
    }

    public override void Disable ()
    {
        base.Disable();
        Unfocus();
    }

    public sealed override void OnBattleChange ()
    {
        base.OnBattleChange();
        if (UserInterface.managedBattle.activePlayer == null)
        {
            if (gameObject.activeInHierarchy)
            {
                Disable();
            }
        }
        else if (!UserInterface.managedBattle.activePlayer.AI && UserInterface.managedBattle.selectedPlayer != null && !gameObject.activeInHierarchy)
        {
            Enable();
        }
        else if (gameObject.activeInHierarchy)
        {
            Disable();
        }
    }

    protected bool IsFocused ()
    {
        return focusedElement == this;
    }

    protected virtual bool IsFocusable ()
    {
        return focusedElement == null;
    }

    protected virtual void Focus ()
    {
        focusedElement = this;
        Debug.Log( "Attack view focusing: " + name );
    }

    protected virtual void Unfocus ()
    {
        if (focusedElement == this)
        {
            focusedElement = null;
        }
    }

    protected sealed override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
        if (IsFocusable())
        {
            OnFocusableTap( position );
        }
        else if (IsFocused())
        {
            OnFocusedTap( position );
        }
    }

    protected virtual void OnFocusableTap ( Vector2 position )
    {

    }


    protected virtual void OnFocusedTap ( Vector2 position )
    {

    }

    protected sealed override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
        if (IsFocusable())
        {
            OnFocusableDrag( initialPosition, currentPosition );
        }
        else if (IsFocused())
        {
            OnFocusedDrag( initialPosition, currentPosition );
        }
    }

    protected virtual void OnFocusableDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {

    }

    protected virtual void OnFocusedDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {

    }

    protected sealed override void OnBeginPress ( Vector2 position )
    {
        base.OnBeginPress( position );
        if (IsFocusable())
        {
            OnFocusableBeginPress( position );
        }
        else if (IsFocused())
        {
            OnFocusedBeginPress( position );
        }
    }

    protected virtual void OnFocusableBeginPress ( Vector2 position )
    {

    }

    protected virtual void OnFocusedBeginPress ( Vector2 position )
    {

    }

    protected sealed override void OnEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnEndPress( initialPosition, currentPosition );
        if (IsFocusable())
        {
            OnFocusableEndPress( initialPosition, currentPosition );
        }
        else if (IsFocused())
        {
            OnFocusedEndPress( initialPosition, currentPosition );
        }
    }

    protected virtual void OnFocusableEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {

    }

    protected virtual void OnFocusedEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {

    }
}
