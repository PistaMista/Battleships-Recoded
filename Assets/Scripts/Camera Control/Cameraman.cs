using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Serialization;
using System.Security.Cryptography;

public enum CameramanAuxParameter
{
    BLUR,
    NOISE,
    FADE,
    SHAKE
}

public class Cameraman : MonoBehaviour
{
    struct CameraWaypoint
    {
        public TargetCameraVector3Value targetPosition;
        public TargetCameraVector3Value targetDirection;
        public float switchThreshold;
        public bool teleporting;
    }

    static List<CameraWaypoint> waypoints;

    public static int Waypoints
    {
        get { return waypoints.Count; }
    }

    struct CameraVector3Value
    {
        public Vector3 initial;
        public Vector3 current;
        public float transitionProgress;
        public float transitionSpeed;
    }

    public struct TargetCameraVector3Value
    {
        public Vector3 target;
        public float transitionTime;
        public float transitionSpeedLimit;

        public TargetCameraVector3Value ( Vector3 target, float transitionTime, float transitionSpeedLimit )
        {
            this.target = target;
            this.transitionTime = transitionTime;
            this.transitionSpeedLimit = transitionSpeedLimit;
        }
    }

    struct CameraAuxiliaryValue
    {
        public float current;
        public float target;
        public float transitionTime;
        public float transitionSpeed;
        public float transitionSpeedLimit;
    }

    static CameraVector3Value position;
    static CameraVector3Value direction;

    static CameraAuxiliaryValue[] auxiliaryParameters;



    static BlurOptimized blur;
    void Awake ()
    {
        auxiliaryParameters = new CameraAuxiliaryValue[5];
        waypoints = new List<CameraWaypoint>();

        blur = Camera.main.GetComponent<BlurOptimized>();
    }


    void Update ()
    {
        if (waypoints.Count > 0)
        {
            if (position.transitionProgress > waypoints[0].switchThreshold && waypoints.Count > 1)
            {
                NextWaypoint();
            }

            for (int i = 0; i < 2; i++)
            {
                CameraVector3Value val = i == 0 ? position : direction;

                float transitionTime = i == 0 ? waypoints[0].targetPosition.transitionTime : waypoints[0].targetDirection.transitionTime;
                float transitionSpeedLimit = i == 0 ? waypoints[0].targetPosition.transitionSpeedLimit : waypoints[0].targetDirection.transitionSpeedLimit;
                Vector3 target = i == 0 ? waypoints[0].targetPosition.target : waypoints[0].targetDirection.target;

                val.transitionProgress = waypoints[0].teleporting ? 100f : Mathf.SmoothDamp( val.transitionProgress, 100f, ref val.transitionSpeed, transitionTime, transitionSpeedLimit );
                float currentProgress = val.transitionProgress / 100f;

                val.current = i == 0 ? Vector3.Lerp( val.current, target, currentProgress ) : Vector3.Slerp( val.current, target, currentProgress );

                if (i == 0)
                {
                    position = val;
                }
                else
                {
                    direction = val;
                }
            }
            //transitionProgress = Mathf.SmoothDamp( transitionProgress, 100f, ref transitionSpeed, waypoints[0].transitionTime, waypoints[0].transitionSpeedLimit );

            //float interpolation = waypoints[0].teleporting ? ( transitionProgress > 90 ? 100f : 0f ) : transitionProgress;
            //currentPosition = Vector3.Lerp( initialPosition, waypoints[0].targetPosition, interpolation / 100f );
            //currentDirection = Vector3.Slerp( initialDirection, waypoints[0].targetDirection, interpolation / 100f );

            //currentBlurIntensity = Mathf.SmoothDamp( currentBlurIntensity, targetBlurIntensity, ref blurChangeRate, blurChangeTime, Mathf.Infinity );
            //blur.blurSize = currentBlurIntensity;
            //blur.enabled = currentBlurIntensity > 0.1f;


        }

        for (int i = 0; i < auxiliaryParameters.Length; i++)
        {
            CameraAuxiliaryValue val = auxiliaryParameters[i];
            val.current = Mathf.SmoothDamp( val.current, val.target, ref val.transitionSpeed, val.transitionTime, val.transitionSpeedLimit );
            switch (i)
            {
                case 0:
                    blur.blurSize = val.current;
                    blur.enabled = val.current > 0.1f;
                    break;
            }
            auxiliaryParameters[i] = val;
        }
        Move();
    }

    /// <summary>
    /// Moves the camera to the current position and direction.
    /// </summary>
    void Move ()
    {
        Camera.main.transform.position = position.current;
        Camera.main.transform.rotation = Quaternion.LookRotation( direction.current );
    }


    public static void AddWaypoint ( TargetCameraVector3Value targetPosition, TargetCameraVector3Value targetDirection, float switchThreshold, bool teleporting )
    {
        CameraWaypoint waypoint;
        waypoint.targetPosition = targetPosition;
        waypoint.targetDirection = targetDirection;
        waypoint.switchThreshold = switchThreshold;
        waypoint.teleporting = teleporting;

        waypoints.Add( waypoint );
    }

    public static void ResetWaypoints ()
    {
        waypoints = new List<CameraWaypoint>();
    }

    static void NextWaypoint ()
    {
        waypoints.RemoveAt( 0 );
        position.transitionProgress = 0;
        direction.transitionProgress = 0;

        position.initial = position.current;
        direction.initial = position.current;
    }


    public static void SetAuxiliaryParameter ( CameramanAuxParameter parameter, float targetValue, float transitionTime, float transitionSpeedLimit )
    {
        int i = (int)parameter;
        auxiliaryParameters[i].target = targetValue;
        auxiliaryParameters[i].transitionTime = transitionTime;
        auxiliaryParameters[i].transitionSpeedLimit = transitionSpeedLimit;
    }
}
