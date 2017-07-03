using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slidable_UIElement : UIElement
{
    /// <summary>
    /// The canvas which carriers all of the UI objects.
    /// </summary>
    public RectTransform carrier;
    /// <summary>
    /// The target position on the screen.
    /// </summary>
    public Vector2 targetPosition;
    /// <summary>
    /// The time it should take to get to the target position.
    /// </summary>
    public float travelTime;
    Vector3 currentVelocity;

    public override void Enable()
    {
        base.Enable();
        targetPosition.x = 0f;
    }

    protected override void Update()
    {
        base.Update();
        carrier.anchoredPosition = Vector3.SmoothDamp(carrier.anchoredPosition, targetPosition, ref currentVelocity, travelTime, Mathf.Infinity);

        if (Mathf.Abs(carrier.anchoredPosition.x) > 1195f && Mathf.Abs(targetPosition.x) > 1195f)
        {
            Disable();
        }
    }
}
