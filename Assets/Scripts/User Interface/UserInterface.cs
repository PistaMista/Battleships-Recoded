using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    /// <summary>
    /// The battle that is currently being managed by the UI.
    /// </summary>
    public static MainBattle managedBattle;
    /// <summary>
    /// All of the UI elements.
    /// </summary>
    public static UIElement[] elements;

    public void Start()
    {
        elements = Master.vars.UIElements;
        elements[0].Enable();
    }

    /// <summary>
    /// Makes the user interface respond to battle updates.
    /// </summary>
    public static void RespondToBattleChanges()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].OnBattleChange();
        }
    }
}
