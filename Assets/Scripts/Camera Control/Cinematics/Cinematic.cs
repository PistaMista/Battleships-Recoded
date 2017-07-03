using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic : ScriptableObject
{
    /// <summary>
    /// The raw arguments received from the initializer.
    /// </summary>
    protected object[] rawArguments;

    /// <summary>
    /// The event to call when the cinematic ends.
    /// </summary>
    Cineman.OnCinematicEnd endEvent;

    /// <summary>
    /// Supplies the arguments for the cinematic.
    /// </summary>
    /// <returns>Whether the arguments are valid.</returns>
    public bool SupplyArguments(Cineman.OnCinematicEnd endEvent, object[] arguments)
    {
        if (AttachArguments(arguments))
        {
            this.endEvent = endEvent;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether the arguments are valid for the cinematic.
    /// </summary>
    /// <param name="arguments">The arguments to check.</param>
    /// <returns>Whether the arguments are valid.</returns>
    protected virtual bool AttachArguments(object[] arguments)
    {
        for (int i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] == null)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Begins the cinematic.
    /// </summary>
    public virtual void Begin()
    {

    }

    /// <summary>
    /// Cycles the cinematic.
    /// </summary>
    public virtual void Cycle()
    {

    }

    /// <summary>
    /// Ends the cinematic.
    /// </summary>
    public virtual void End()
    {
        if (endEvent != null)
        {
            endEvent();
        }

        Destroy(this);
    }
}
