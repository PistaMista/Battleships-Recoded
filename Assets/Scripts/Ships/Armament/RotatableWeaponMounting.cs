using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatableWeaponMounting : MonoBehaviour
{

    /// <summary>
    /// The target rotation angle.
    /// </summary>
    public float targetRotation = 0f;
    /// <summary>
    /// The default rotation angle.
    /// </summary>
    public float defaultRotation = 0f;
    /// <summary>
    /// The current rotation angle.
    /// </summary>
    public float currentRotation = 0f;
    /// <summary>
    /// Whether the mount should automatically activate the weapons after rotating.
    /// </summary>
    public float autoFirePrecisionRating;

    public float leftRotationLimit;
    public float rightRotationLimit;


    /// <summary>
    /// The zones in which the guns are allowed to fire.
    /// </summary>
    public float[] firingAngles;
    /// <summary>
    /// The mounted weapons.
    /// </summary>
    public Armament[] weapons;
    /// <summary>
    /// The ship this mount is mounted on.
    /// </summary>
    public Ship ship;
    /// <summary>
    /// The maximum speed at which this mounting rotates.
    /// </summary>
    public float maximumTraverseSpeed;
    public float currentTraverseSpeed;
    void Update ()
    {
        if (autoFirePrecisionRating > 0 && currentTraverseSpeed < 0.5f / autoFirePrecisionRating && Mathf.Abs( currentRotation - targetRotation ) < 2)
        {
            if (FireWeapons())
            {
                autoFirePrecisionRating = 0;
            }
        }
        Rotate();
    }

    void Awake ()
    {
        defaultRotation = transform.rotation.eulerAngles.y;
    }

    /// <summary>
    /// Rotates the mounting towards the world position.
    /// </summary>
    /// <param name="worldPosition"></param>
    public void RotateTowards ( Vector3 worldPosition )
    {
        Vector3 localPosition = transform.InverseTransformPoint( worldPosition );

        targetRotation = currentRotation + Mathf.Atan2( localPosition.x, localPosition.z ) * Mathf.Rad2Deg;
        if (Mathf.Abs( targetRotation ) > 180f)
        {
            targetRotation -= 360f * Mathf.Sign( targetRotation );
        }
    }


    /// <summary>
    /// Aims the weapons at a position.
    /// </summary>
    /// <param name="worldPosition"></param>
    public void AimWeapons ( Vector3 worldPosition )
    {
        foreach (Armament weapon in weapons)
        {
            weapon.TargetPosition( worldPosition );
        }
    }

    /// <summary>
    /// Rotates the mounting towards the target rotation.
    /// </summary>
    void Rotate ()
    {
        currentRotation = Mathf.SmoothDamp( currentRotation, targetRotation, ref currentTraverseSpeed, 0.3f, maximumTraverseSpeed );
        if (currentRotation < -rightRotationLimit)
        {
            currentRotation = -rightRotationLimit;
        }
        else if (currentRotation > leftRotationLimit)
        {
            currentRotation = leftRotationLimit;
        }

        transform.localRotation = Quaternion.Euler( Vector3.up * ( currentRotation + defaultRotation ) );
    }

    /// <summary>
    /// Fires the weapons.
    /// </summary>
    public bool FireWeapons ()
    {

        if (IsWithinFiringAngles() || autoFirePrecisionRating < 0.1f)
        {
            foreach (Armament weapon in weapons)
            {
                if (!weapon.ready)
                {
                    return false;
                }
            }

            foreach (Armament weapon in weapons)
            {
                weapon.Fire();
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Whether this ship can fire the guns without hitting itself or being incapable of aiming at the target.
    /// </summary>
    /// <returns></returns>
    public bool IsWithinFiringAngles ()
    {
        for (int i = 1; i < firingAngles.Length; i += 2)
        {
            if (targetRotation < firingAngles[i] && targetRotation > firingAngles[i - 1] && currentRotation < firingAngles[i] && currentRotation > firingAngles[i - 1])
            {
                return true;
            }
        }
        return false;
    }
}
