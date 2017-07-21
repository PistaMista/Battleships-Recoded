using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Hosting;

public class Cinematic_WeaponTurretPan : Cinematic
{

    RotatableWeaponMounting turret;

    float panTime;

    protected override bool AttachArguments ( object[] arguments )
    {
        if (!base.AttachArguments( arguments ) || !( arguments[0] is RotatableWeaponMounting && arguments[1] is float ))
        {
            return false;
        }

        turret = (RotatableWeaponMounting)arguments[0];
        panTime = (float)arguments[1];

        int density = 60;
        int arc = 120;
        float lowerLimit = Mathf.Clamp( turret.currentRotation - arc / 2, -turret.leftRotationLimit, turret.rightRotationLimit );
        float upperLimit = Mathf.Clamp( turret.currentRotation + arc / 2, -turret.leftRotationLimit, turret.rightRotationLimit );


        for (int i = 0; i < density; i++)
        {
            float time = panTime / density;

        }

        return true;
    }

    public override void Begin ()
    {
        base.Begin();
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
