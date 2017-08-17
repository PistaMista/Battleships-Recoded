using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryMarkCross_DeployableGraphicsElement : DeployableGraphicsElement
{

    private void Start ()
    {
        Set( 1, 4, 10, 1 );
    }

    float strokeWidth;
    float strokeLength;

    float secondStrokeMiddleExclusion;

    Renderer[] renderers;
    public void Set ( float strokeWidth, float strokeLength, float deployTime, float transparency )
    {
        //TEST
        BaseReset();
        defaultTransparency = transparency;
        this.deployTime = deployTime;
        this.strokeWidth = strokeWidth;
        this.strokeLength = strokeLength;

        renderers = new Renderer[3];

        deploying = true;
        deploySpeedLimit = Mathf.Infinity;

        secondStrokeMiddleExclusion = strokeWidth / strokeLength;
    }

    protected override void SetTransparencyMod ( float transparencyMod )
    {
        base.SetTransparencyMod( transparencyMod );
        foreach (Renderer render in renderers)
        {
            if (render != null)
            {
                SetTransparencyForRenderer( render, defaultTransparency * transparencyMod );
            }
        }
    }

    protected override void Update ()
    {
        base.Update();
        //if (deployProgress <= 0.5f)
        //{
        //    Vector3 origin = new Vector3( -1, 0, -1 ) * strokeLength / 2.0f;
        //    Vector3 destination = origin + new Vector3( 1, 0, 1 ) * deployProgress * strokeLength * 2.0f;
        //    ManageLine( 0, origin, destination );
        //}
        //else if (deployProgress / 0.5f - 1.0f < 0.5f - secondStrokeMiddleExclusion / 2.0f)
        //{
        //    Vector3 origin = new Vector3( -1, 0, 1 ) * strokeLength / 2.0f;
        //    Vector3 destination = origin + new Vector3( 1, 0, -1 ) * ( strokeLength * ( deployProgress - 0.5f ) );
        //    ManageLine( 1, origin, destination );
        //}
        //else if (deployProgress / 0.5f - 1.0f > 0.5f + secondStrokeMiddleExclusion / 2.0f)
        //{
        //    Vector3 origin = new Vector3( 1, 0, -1 ) * ( 0.5f + secondStrokeMiddleExclusion / 2.0f );
        //    Vector3 destination = origin + new Vector3( 1, 0, -1 ) * ( deployProgress - ( 0.5f + secondStrokeMiddleExclusion / 2.0f ) );
        //    ManageLine( 2, origin, destination );
        //}
    }

    void ManageLine ( int quadID, float length )
    {
        //Renderer quad = renderers[quadID];

        //if (quad == null)
        //{
        //    quad = GameObject.CreatePrimitive( PrimitiveType.Quad ).GetComponent<Renderer>();
        //    quad.transform.SetParent( visualParent.transform );
        //    quad.material = mainMaterial;
        //    SetTransparencyForRenderer( quad, defaultTransparency * transparencyMod );
        //    renderers[quadID] = quad;
        //}

        //quad.transform.localPosition = origin / 2.0f + destination / 2.0f;
        //quad.transform.localScale = new Vector3( strokeWidth, Vector3.Distance( origin, destination ), 1f );
        //quad.transform.localRotation.SetLookRotation( destination - origin );
    }
}
