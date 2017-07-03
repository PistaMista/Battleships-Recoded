using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    /// <summary>
    /// Enables the UI element.
    /// </summary>
    public virtual void Enable()
    {
        gameObject.SetActive(true);
        justEnabled = true;
    }

    bool justEnabled;

    /// <summary>
    /// Disables the UI element.
    /// </summary>
    public virtual void Disable()
    {
        gameObject.SetActive(false);
        InputController.onBeginPress -= OnBeginPress;
        InputController.onTap -= OnTap;
        InputController.onDrag -= OnDrag;
        InputController.onEndPress -= OnEndPress;
    }

    /// <summary>
    /// Responds to battle updates.
    /// </summary>
    public virtual void OnBattleChange()
    {

    }

    /// <summary>
    /// The update function.
    /// </summary>
    protected virtual void Update()
    {
        if (justEnabled)
        {
            InputController.onBeginPress -= OnBeginPress;
            InputController.onTap -= OnTap;
            InputController.onDrag -= OnDrag;
            InputController.onEndPress -= OnEndPress;

            InputController.onBeginPress += OnBeginPress;
            InputController.onTap += OnTap;
            InputController.onDrag += OnDrag;
            InputController.onEndPress += OnEndPress;
            justEnabled = false;
        }
    }

    /// <summary>
    /// Executed when the player presses on the screen.
    /// </summary>
    protected virtual void OnBeginPress(Vector2 position)
    {

    }

    /// <summary>
    /// Executed when the player taps on the screen.
    /// </summary>
    /// <param name="position"></param>
    protected virtual void OnTap(Vector2 position)
    {

    }

    /// <summary>
    /// Executed when the player drags on the screen.
    /// </summary>
    protected virtual void OnDrag(Vector2 initialPosition, Vector2 currentPosition)
    {

    }

    /// <summary>
    /// Executed when the player stops pressing the screen.
    /// </summary>
    protected virtual void OnEndPress(Vector2 initialPosition, Vector2 currentPosition)
    {

    }
}
