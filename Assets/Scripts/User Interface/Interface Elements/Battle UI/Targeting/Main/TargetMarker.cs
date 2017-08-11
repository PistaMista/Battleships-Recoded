using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{

    public object target;

    public GameObject visualParent;

    public void OnRemove ()
    {
        //HACK This is temporary behaviour.
        Destroy( gameObject );
    }

    public virtual bool PositionIntersects ( Vector3 worldPosition )
    {
        return false;
    }

    public virtual void SetUp ()
    {

    }
}
