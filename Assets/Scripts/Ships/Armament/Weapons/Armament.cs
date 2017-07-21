using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armament : MonoBehaviour
{
    /// <summary>
    /// On activation.
    /// </summary>
    public delegate void OnActivation ();

    public OnActivation onActivation;

    /// <summary>
	/// The mount this armament is mounted in.
	/// </summary>
    public RotatableWeaponMounting mount;

    /// <summary>
    /// The projectiles launched by this armament.
    /// </summary>
    public List<Projectile> launchedProjectiles;

    /// <summary>
    /// Whether this armament is ready to activate.
    /// </summary>
    public bool ready;

    /// <summary>
    /// Targets a position in the world.
    /// </summary>
    /// <param name="worldPosition">The position to target.</param>
    public virtual void TargetPosition ( Vector3 worldPosition )
    {

    }

    /// <summary>
    /// Fires the armament.
    /// </summary>
    public virtual void Fire ()
    {
        if (onActivation != null)
        {
            onActivation();
        }
    }

}
