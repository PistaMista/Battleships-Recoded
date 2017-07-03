using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    //The y layer at which to process input
    /// <summary>
    /// The y layer at which to process input.
    /// </summary>
    public static float referenceY;
    /// <summary>
    /// The current input screen position.
    /// </summary>
    static Vector3 currentScreenInputPosition;
    /// <summary>
    /// The initial input screen position.
    /// </summary>
    static Vector3 initialScreenInputPosition;
    /// <summary>
    /// The last frame screen input position.
    /// </summary>
    static Vector3 lastScreenInputPosition;
    /// <summary>
    /// Whether the screen began to get pressed this frame.
    /// </summary>
    static bool beginPress = false;
    /// <summary>
    /// Whether the screen ended getting pressed this frame.
    /// </summary>
    static bool endPress = false;
    /// <summary>
    /// Whether the player released the screen without dragging.
    /// </summary>
    static bool tap = false;
    /// <summary>
    /// Whether the player is dragging.
    /// </summary>
    static bool dragging = false;
    /// <summary>
    /// Whether the screen is pressed.
    /// </summary>
    static bool pressed = false;
    /// <summary>
    /// The distance the player needs to drag in order to register dragging.
    /// </summary>
    public float dragRegisterDistance = 0.3f;
    /// <summary>
    /// Determines what data will be returned when asking for variables.
    /// </summary>
    public static int securityLevel = 63;

    public delegate void OnBeginPress(Vector2 position);
    public delegate void OnTap(Vector2 position);
    public delegate void OnEndPress(Vector2 initialPosition, Vector2 currentPosition);
    public delegate void OnDrag(Vector2 initialPosition, Vector2 currentPosition);

    public static OnBeginPress onBeginPress;
    public static OnTap onTap;
    public static OnEndPress onEndPress;
    public static OnDrag onDrag;




    /// <summary>
    /// The update function.
    /// </summary>
    void Update()
    {
        if (Application.isMobilePlatform)
        {
            MobileInput();
        }
        else
        {
            PCInput();
        }

        Shared();

        lastScreenInputPosition = currentScreenInputPosition;
    }

    /// <summary>
    /// Methods used to calculate input for PC.
    /// </summary>
    void PCInput()
    {
        float clippingDistance = Camera.main.transform.position.y - referenceY;
        currentScreenInputPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        beginPress = Input.GetMouseButtonDown(0);
        if (endPress)
        {
            endPress = false;
        }
        else
        {
            endPress = Input.GetMouseButtonUp(0);
        }
        pressed = Input.GetMouseButton(0);
    }


    bool lastState;
    /// <summary>
    /// Methods used to calculate input for mobile.
    /// </summary>
    void MobileInput()
    {
        float clippingDistance = Camera.main.transform.position.y - referenceY;
        if (Input.touchCount > 0)
        {
            pressed = true;
            Touch touch = Input.GetTouch(0);
            currentScreenInputPosition = new Vector3(touch.position.x, touch.position.y);
        }
        else
        {
            pressed = false;
        }

        beginPress = !lastState && pressed;
        endPress = lastState && !pressed;

        lastState = pressed;
    }

    /// <summary>
    /// The methods used to calculate input for PC and mobile.
    /// </summary>
    void Shared()
    {
        if (beginPress)
        {
            initialScreenInputPosition = currentScreenInputPosition;
            if (onBeginPress != null)
            {
                onBeginPress(currentScreenInputPosition);
            }
        }

        tap = endPress && !dragging;

        if (tap && onTap != null)
        {
            onTap(initialScreenInputPosition);
        }

        if (endPress && onEndPress != null)
        {
            onEndPress(initialScreenInputPosition, currentScreenInputPosition);
        }

        if (pressed)
        {
            if (Vector3.Distance(currentScreenInputPosition, initialScreenInputPosition) / Screen.width * 100f > dragRegisterDistance)
            {
                dragging = true;
            }
        }
        else
        {
            dragging = false;
        }

        if (dragging && onDrag != null)
        {
            onDrag(initialScreenInputPosition, currentScreenInputPosition);
        }
    }

    /// <summary>
    /// Converts a position on the screen to a world position.
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <param name="clippingDistance"></param>
    public static Vector3 ConvertToWorldPoint(Vector2 screenPosition, float clippingDistance)
    {
        return Camera.main.ScreenToWorldPoint((Vector3)screenPosition + Vector3.forward * clippingDistance);
    }

    public void ChangeSecurityLevel(int level)
    {
        InputController.securityLevel = level;
    }
}
