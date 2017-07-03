﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShell : Projectile
{

    public Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);
        velocity.y -= Master.vars.gravity * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(velocity);
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }
}