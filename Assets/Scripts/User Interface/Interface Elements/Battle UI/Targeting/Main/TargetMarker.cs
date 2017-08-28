using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    public TargetingUIElement.Target target;
    public TargetingUIElement.Target potentialTarget;

    public GraphicsElement main;
    public GraphicsElement ghost;

    public void OnRemove ()
    {
        main.targetTransparencyMod = 0.0f;
        main.destroyAfterTransparencyTransition = true;

        if (ghost != null)
        {
            Destroy( ghost.gameObject );
            ghost = null;
        }

        potentialTarget.target = null;
        target.target = null;
    }

    public virtual bool PositionIntersects ( Vector3 worldPosition )
    {
        return false;
    }

    public virtual void SetVisualsForTarget ( TargetingUIElement.Target target )
    {
        main.MainMaterial = target.valid ? Master.vars.targetValidMaterial : Master.vars.targetInvalidMaterial;
    }

    public virtual void StartMove ()
    {
        ghost = Instantiate( main.gameObject ).GetComponent<GraphicsElement>();
        ghost.transform.SetParent( transform );
        ghost.name = ghost.name + " - Ghost";
        ghost.transform.position = main.transform.position;
        ghost.targetTransparencyMod = 0.5f;
        lastTarget = null;
    }

    protected object lastTarget;
    public virtual void Moving ()
    {
        if (potentialTarget.target != lastTarget)
        {
            if (potentialTarget.target == null)
            {
                if (main != null)
                {
                    Destroy( main.gameObject );
                    main = null;
                }
            }
            else
            {
                SetVisualsForTarget( potentialTarget );
            }
        }

        lastTarget = potentialTarget;
    }


    public virtual void EndMove ()
    {
        if (potentialTarget.valid)
        {
            target = potentialTarget;
            if (ghost != null)
            {
                ghost.targetTransparencyMod = 0.0f;
                ghost.destroyAfterTransparencyTransition = true;
                ghost = null;
            }
        }
        else if (ghost != null)
        {
            if (main != null)
            {
                main.targetTransparencyMod = 0.0f;
                main.destroyAfterTransparencyTransition = true;
            }


            main = ghost;
            main.targetTransparencyMod = 1.0f;
            ghost = null;

        }
    }

    void Update ()
    {
        if (main == null && ghost == null)
        {
            Destroy( gameObject );
        }
    }

    void OnDisable ()
    {
        Destroy( gameObject );
    }
}
