using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScreen_UIElement : Slidable_UIElement
{
    public override void Enable ()
    {
        base.Enable();
        Cameraman.SetAuxiliaryParameter( 5f, 1f );
        carrier.anchoredPosition = originalPosition;
    }

    public override void Disable ()
    {
        base.Disable();
    }

    protected override void Update ()
    {
        base.Update();
    }

    protected override void OnBeginPress ( Vector2 position )
    {
        base.OnBeginPress( position );
        targetPosition.x = -1200f;
        UserInterface.elements[0].Enable();
        InputController.onBeginPress -= OnBeginPress;
    }
}
