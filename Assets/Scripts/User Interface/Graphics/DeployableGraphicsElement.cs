using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployableGraphicsElement : GraphicsElement
{
    public float deployProgress;
    public bool deploying;
    public float deployTime;
    public float deploySpeedLimit;
    protected float currentDeploySpeed;

    protected override void Update ()
    {
        base.Update();
        if (deploying)
        {
            deployProgress = Mathf.SmoothDamp( deployProgress, 1.0f, ref currentDeploySpeed, deployTime, deploySpeedLimit );
        }
    }
}
