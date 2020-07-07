﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashParticles : MonoBehaviour
{   
    public void PlayParticles(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(direction, Vector2.right);
        transform.rotation = Quaternion.Euler(0, 0, -angle);
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startRotationZ = new ParticleSystem.MinMaxCurve(angle * Mathf.Deg2Rad);
        Destroy(gameObject, main.duration);
    }
}
