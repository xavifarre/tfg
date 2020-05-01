﻿using UnityEngine;
using System.Collections;

public class BarrelProximity : Barrel
{
    [Header("Proximity")]
    public float explosionDelay;
    public float durability;
    public float triggerRadius;
    public GameObject triggerObject;

    [Header("Colors (Debug)")]
    public Color colorInactive;
    public Color colorActive;
    public Color colorAboutToExplode;

    private enum BarrelState { Inactive, Active, AboutToExplode, Destroyed };
    private BarrelState state;

    //Dest position
    public Vector3 destPos;
    private Vector2[] arcPoints;
    private float flightTime;

    private Perserver perserver;
    private ShadowController shadowController;
    
    // Use this for initialization
    void Start()
    {
        GetComponent<SpriteRenderer>().color = colorInactive;
        shadowController = GetComponent<ShadowController>();
        perserver = FindObjectOfType<Perserver>();

        //transform.position = destPos;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LaunchBarrel(Vector3 launchPosition, Vector3 destPosition, float speed, int iterations, float gravityScale)
    {
        destPos = destPosition;
        state = BarrelState.Inactive;

        float horizontalDistance = Mathf.Abs(destPos.x - launchPosition.x);
        float yOffset = destPos.y - launchPosition.y;
        float gravity = Physics.gravity.magnitude * gravityScale;
        int direction = destPos.x > launchPosition.x ? 1 : -1;

        bool reached = MathFunctions.ProjectileLaunchAngle(speed, horizontalDistance, yOffset, gravity, out float angle0, out float angle1);
        arcPoints = MathFunctions.ProjectileArcPoints(iterations, speed, horizontalDistance, gravity, angle0, direction, launchPosition);
        flightTime = MathFunctions.ProjectileTimeOfFlight(speed, angle0, yOffset, gravity);
        Debug.Log(reached);
        StartCoroutine(ILaunch(iterations, launchPosition));
    }

    public void ActivateBarrel()
    {
        state = BarrelState.Active;
        triggerObject.SetActive(true);
        GetComponent<SpriteRenderer>().color = colorActive;
    }

    public void DisableBarrel()
    {
        state = BarrelState.Destroyed;
        Destroy(gameObject);
    }

    public void Hit(Attack attack)
    {
        durability -= attack.damage;
        if (durability <= 0)
        {
            DisableBarrel();
        }
    }

    public void StartCountdown()
    {
        state = BarrelState.AboutToExplode;
        StartCoroutine(ICountDown());
        GetComponent<SpriteRenderer>().color = colorAboutToExplode;

    }

    public bool IsHitable()
    {
        return state == BarrelState.Active || state == BarrelState.AboutToExplode;
    }

    public IEnumerator ICountDown()
    {
        yield return new WaitForSeconds(explosionDelay);
        if (state == BarrelState.AboutToExplode)
        {
            Explode();
            state = BarrelState.Destroyed;
        }
    }

    public IEnumerator ILaunch(int iterations, Vector3 launchPos)
    {
        yield return null;
        float iterationTime = flightTime / iterations;
        float t = 0;
        for (int i = 0; i < arcPoints.Length - 1; i++)
        {
            while(t < iterationTime)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(arcPoints[i], arcPoints[i + 1], t / iterationTime);
                MathFunctions.LineLineIntersection(out Vector3 intersecPoint, transform.position, - Vector3.up, launchPos, destPos - launchPos);
                shadowController.height = Vector3.Distance(intersecPoint, transform.position);
                yield return null;
            }
            t %= iterationTime;
        }
        transform.position = destPos;
        ActivateBarrel();
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < arcPoints.Length - 1 ; i++)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawLine(arcPoints[i], arcPoints[i + 1]);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(arcPoints[i + 1], 0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy" || collision.tag == "EnemyAttack")
        {
            if(state == BarrelState.Active || state == BarrelState.AboutToExplode)
            {
                Explode();
            }
        }
    }
}