using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Cameraman : MonoBehaviour
{
    struct CameraWaypoint
    {
        public Vector3 targetPosition;
        public Vector3 targetDirection;
        public float transitionTime;
        public float transitionSpeedLimit;
        public bool teleporting;
    }

    static List<CameraWaypoint> waypoints;

    public static int Waypoints
    {
        get { return waypoints.Count; }
    }

    static Vector3 initialPosition;
    static Vector3 initialDirection = Vector3.forward;
    static Vector3 currentDirection;
    static Vector3 currentPosition;
    static float transitionProgress = 0;
    static float transitionSpeed = 0f;
    static BlurOptimized blur;

    void Awake()
    {
        waypoints = new List<CameraWaypoint>();
        blur = Camera.main.GetComponent<BlurOptimized>();
    }


    void Update()
    {
        if (waypoints.Count > 0)
        {
            if (transitionProgress > 98f && waypoints.Count > 1)
            {
                NextWaypoint();
            }

            transitionProgress = Mathf.SmoothDamp(transitionProgress, 100f, ref transitionSpeed, waypoints[0].transitionTime, waypoints[0].transitionSpeedLimit);

            float interpolation = waypoints[0].teleporting ? (transitionProgress > 90 ? 100f : 0f) : transitionProgress;
            currentPosition = Vector3.Lerp(initialPosition, waypoints[0].targetPosition, interpolation / 100f);
            currentDirection = Vector3.Slerp(initialDirection, waypoints[0].targetDirection, interpolation / 100f);

            currentBlurIntensity = Mathf.SmoothDamp(currentBlurIntensity, targetBlurIntensity, ref blurChangeRate, blurChangeTime, Mathf.Infinity);
            blur.blurSize = currentBlurIntensity;
            blur.enabled = currentBlurIntensity > 0.1f;
        }
        else
        {
            transitionProgress = 0f;

            initialPosition = currentPosition;
            initialDirection = currentDirection;
        }
        Move();
    }

    /// <summary>
    /// Moves the camera to the current position and direction.
    /// </summary>
    void Move()
    {
        Camera.main.transform.position = currentPosition;
        Camera.main.transform.rotation = Quaternion.LookRotation(currentDirection);
    }


    public static void AddWaypoint(Vector3 position, Vector3 direction, float transitionTime, float transitionSpeedLimit, bool teleporting)
    {
        CameraWaypoint waypoint;
        waypoint.targetPosition = position;
        waypoint.targetDirection = direction;
        waypoint.transitionTime = transitionTime;
        waypoint.transitionSpeedLimit = transitionSpeedLimit;
        waypoint.teleporting = teleporting;

        waypoints.Add(waypoint);
    }

    static void NextWaypoint()
    {
        waypoints.RemoveAt(0);
        transitionProgress = 0f;

        initialPosition = currentPosition;
        initialDirection = currentDirection;
    }

    static float currentBlurIntensity = 0f;
    static float targetBlurIntensity = 0f;
    static float blurChangeRate;
    static float blurChangeTime;
    /// <summary>
    /// Sets the blur effect of the camera.
    /// </summary>
    /// <param name="intensity">The intensity of the blur.</param>
    public static void SetBlurIntensity(float intensity, float transitionTime)
    {
        targetBlurIntensity = intensity;
        blurChangeTime = transitionTime;
    }
}
