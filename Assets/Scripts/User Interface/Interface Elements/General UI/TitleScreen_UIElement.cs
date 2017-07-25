using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen_UIElement : Slidable_UIElement
{
    /// <summary>
    /// Enables the UI element.
    /// </summary>
    public override void Enable()
    {
        base.Enable();
        Cameraman.SetAuxiliaryParameter(0f, 0.5f);
    }

    /// <summary>
    /// Disables the UI element.
    /// </summary>
    public override void Disable()
    {
        base.Disable();
    }

    /// <summary>
    /// Responds to battle updates.
    /// </summary>
    public override void OnBattleChange()
    {
        base.OnBattleChange();
    }

    /// <summary>
    /// The update function.
    /// </summary>
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnDrag(Vector2 initialPosition, Vector2 currentPosition)
    {
        base.OnDrag(initialPosition, currentPosition);
        targetPosition.x = (currentPosition.x - initialPosition.x);
    }

    protected override void OnEndPress(Vector2 initialPosition, Vector2 currentPosition)
    {
        base.OnEndPress(initialPosition, currentPosition);
        float rawDistance = currentPosition.x - initialPosition.x;
        if (Mathf.Abs(rawDistance) > Screen.width / 3.5f)
        {
            if (rawDistance > 0)
            {
                targetPosition.x = 1200f;
                UserInterface.elements[1].Enable();
            }
            else
            {
                targetPosition.x = -1200f;
                UserInterface.elements[2].Enable();
            }

            InputController.onEndPress -= OnEndPress;
            InputController.onDrag -= OnDrag;
        }
        else
        {
            targetPosition.x = 0f;
        }
    }
}
