using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// The armament that launched this projectile.
    /// </summary>
    Armament launchedBy;
    /// <summary>
    /// Whether this projectile has been terminated and is no longer needed.
    /// </summary>
    public bool terminated = false;

    float destructionDelay;
    protected virtual void Terminate ( float destructionDelay )
    {
        if (launchedBy != null)
        {
            launchedBy.launchedProjectiles.Remove( this );
        }
        terminated = true;
        this.destructionDelay = destructionDelay;
    }

    protected virtual void Terminate ()
    {
        if (launchedBy != null)
        {
            launchedBy.launchedProjectiles.Remove( this );
        }
        terminated = true;
        Destroy( gameObject );
    }

    protected virtual void Update ()
    {
        if (destructionDelay > 0)
        {
            destructionDelay -= Time.deltaTime;
            if (destructionDelay <= 0)
            {
                Destroy( gameObject );
            }
        }
    }
}
