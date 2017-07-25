using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic_test : Cinematic
{
    int steps = 0;
    /// <summary>
    /// Attaches the play time argument.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <returns>Validity.</returns>
    protected override bool AttachArguments ( object[] arguments )
    {
        if (!base.AttachArguments( arguments ))
        {
            return false;
        }

        if (ReferenceEquals( arguments[0].GetType(), steps.GetType() ))
        {
            steps = (int)arguments[0];

            if (steps > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Begins the test cinematic.
    /// </summary>
    public override void Begin ()
    {
        base.Begin();
        float angularIncrease = 360f / steps;
        for (int i = 0; i < steps; i++)
        {
            Vector3 position = new Vector3( Mathf.Cos( angularIncrease * i * Mathf.Deg2Rad ) * 30f, 10f, Mathf.Sin( angularIncrease * i * Mathf.Deg2Rad ) * 30f );
            Vector3 direction = -position;

            Cameraman.AddWaypoint( new Cameraman.TargetCameraVector3Value( position, 0, 40000f ), new Cameraman.TargetCameraVector3Value( direction, 0, 40000f ), 95f, false );
        }
    }

    /// <summary>
    /// Cycles the test cinematic.
    /// </summary>	
    public override void Cycle ()
    {
        base.Cycle();
        if (Cameraman.Waypoints == 1)
        {
            End();
        }
    }

    /// <summary>
    /// Ends the test cinematic.
    /// </summary>	
    public override void End ()
    {
        base.End();
    }
}
