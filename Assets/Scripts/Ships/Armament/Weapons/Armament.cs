using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armament : MonoBehaviour
{

    /// <summary>
	/// The mount this armament is mounted in.
	/// </summary>
    public RotatableWeaponMounting mount;

    /// <summary>
    /// Targets a position in the world.
    /// </summary>
    /// <param name="worldPosition">The position to target.</param>
    public virtual void TargetPosition(Vector3 worldPosition)
    {

    }

    /// <summary>
    /// Fires the armament.
    /// </summary>
    public virtual void Fire()
    {

    }

}
