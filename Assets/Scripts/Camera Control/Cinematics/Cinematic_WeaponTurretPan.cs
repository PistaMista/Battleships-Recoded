using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Hosting;

public class Cinematic_WeaponTurretPan : Cinematic
{

    RotatableWeaponMounting turret;
    float panTime;
    float distance;

    protected override bool AttachArguments ( object[] arguments )
    {
        if (!base.AttachArguments( arguments ) || !( arguments[0] is RotatableWeaponMounting && arguments[1] is float && arguments[2] is float ))
        {
            return false;
        }

        turret = (RotatableWeaponMounting)arguments[0];
        panTime = (float)arguments[1];
        distance = (float)arguments[2];

        return true;
    }

    public override void Begin ()
    {
        base.Begin();
        float density = 1;
        float arc = 120;
        float lowerLimit = Mathf.Clamp( turret.targetRotation - arc / 2, -turret.leftRotationLimit, turret.rightRotationLimit );
        float upperLimit = Mathf.Clamp( turret.targetRotation + arc / 2, -turret.leftRotationLimit, turret.rightRotationLimit );
        arc = Mathf.Abs( lowerLimit - upperLimit );


        int count = Mathf.FloorToInt( arc / density );
        density = arc / count;
        int cycles = 0;
        float elevation = Random.Range( 0.20f, 0.35f );

        for (float i = lowerLimit; i < upperLimit; i += density)
        {
            float time = panTime / count;
            float angle = i * Mathf.Deg2Rad;

            Vector3 localPos = new Vector3( Mathf.Sin( angle ), elevation, Mathf.Cos( angle ) ) * distance;
            Vector3 worldPosition = turret.transform.TransformPoint( localPos );
            Vector3 lookDirection = turret.transform.position - worldPosition;

            Cameraman.AddWaypoint( worldPosition, lookDirection, 0.01f, cycles == 0 ? Mathf.Infinity : 100f / time, 85f, cycles == 0 );
            cycles++;
        }
    }

    public override void End ()
    {
        base.End();
    }

    public override void Cycle ()
    {
        base.Cycle();
        if (Cameraman.Waypoints == 1)
        {
            End();
        }
    }
}
