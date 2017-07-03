using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Armament
{

    float targetAngle = 0f;
    float currentAngle = 0f;
    float currentElevationRate = 0f;
    public Transform barrel;
    public ParticleSystem visualEffect;
    public Vector3 defaultBarrelPosition;

    public void Awake()
    {
        defaultBarrelPosition = barrel.transform.localPosition;
    }

    public override void TargetPosition(Vector3 worldPosition)
    {
        base.TargetPosition(worldPosition);
        float shellVelocity = Master.vars.shellVelocities[0];
        float gravity = Master.vars.gravity;
        float altitudeDifference = worldPosition.y - transform.position.y;
        float distanceToTarget = Vector2.Distance(new Vector2(worldPosition.x, worldPosition.z), new Vector2(transform.position.x, transform.position.z));
        targetAngle = -Mathf.Atan((Mathf.Pow(shellVelocity, 2f) - Mathf.Sqrt(Mathf.Pow(shellVelocity, 4f) - gravity * (gravity * Mathf.Pow(distanceToTarget, 2f) + 2 * altitudeDifference * Mathf.Pow(shellVelocity, 2f)))) / (gravity * distanceToTarget)) * Mathf.Rad2Deg;
    }

    public override void Fire()
    {
        base.Fire();
        Vector3 barrelRelativePosition = barrel.position - transform.position;
        CannonShell shell = Instantiate(Master.vars.cannonShellPrefabs[0]).GetComponent<CannonShell>();
        shell.velocity = barrelRelativePosition.normalized * Master.vars.shellVelocities[0];
        shell.transform.position = barrel.position;
        barrel.transform.Translate(Vector3.back * barrelRelativePosition.magnitude / 1.2f);
        visualEffect.Play();
    }

    void Update()
    {
        currentAngle = Mathf.SmoothDamp(currentAngle, targetAngle, ref currentElevationRate, 0.3f, 20f);
        transform.localRotation = Quaternion.Euler(Vector3.right * currentAngle);
        barrel.transform.localPosition = Vector3.MoveTowards(barrel.transform.localPosition, defaultBarrelPosition, 0.1f * Time.deltaTime);
    }
}
