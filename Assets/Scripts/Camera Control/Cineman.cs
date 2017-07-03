using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cineman : MonoBehaviour
{
    public delegate void OnCinematicEnd();

    /// <summary>
    /// What cinematic is currently playing.
    /// </summary>
    public static Cinematic currentlyPlaying;
    /// <summary>
    /// Starts a cinematic.
    /// </summary>
    /// <param name="cinematic">The type of cinematic to start.</param>
    /// <param name="endEvent">What method to call after the cinematic ends.</param>
    /// <param name="parameters">The parameters of the cinematic.</param>
    /// <returns>Whether the cinematic was started successfully.</returns>
    public static bool StartCinematic(string cinematic, OnCinematicEnd endEvent, object[] parameters)
    {
        Cinematic candidate = (Cinematic)ScriptableObject.CreateInstance("Cinematic_" + cinematic);
        if (candidate == null)
        {
            return false;
        }
        else if (candidate.SupplyArguments(endEvent, parameters))
        {
            candidate.Begin();
            currentlyPlaying = candidate;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (currentlyPlaying != null)
        {
            currentlyPlaying.Cycle();
        }
    }
}
