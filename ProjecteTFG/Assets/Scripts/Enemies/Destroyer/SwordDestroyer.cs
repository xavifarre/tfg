using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SwordDestroyer : Attack
{
    public float inclination;
    public LineRenderer line;
    public TrailRenderer trail;
    public Collider2D swordCollider;
    public Collider2D releaseCollider;
    private bool prep;
    private bool charge;
    private List<SwordDestroyer> swordList;

    private Player player;

    public void StartRotating(float angularSpeed, float radius, float motionDir, Destroyer destroyer, float chargeDuration, float minRadius)
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(IPrep(angularSpeed, radius, motionDir, destroyer, chargeDuration, minRadius));   
    }

    public void Charge(List<SwordDestroyer> swords)
    {
        charge = true;
        swordList = swords;
    }

    public void ReleaseSword(float releaseDelay, float alertDuration, float speed, float duration, Destroyer destroyer)
    {
        prep = false;
        StartCoroutine(IRelease(releaseDelay, alertDuration, speed, duration, destroyer));
    }

    private IEnumerator IPrep(float angularSpeed, float radius, float motionDir, Destroyer destroyer, float chargeDuration, float minRadius)
    {
        float currentAngle = 0, t = 0;
        prep = true;
        charge = false;
        swordCollider.enabled = true;
        releaseCollider.enabled = false;
        float currentRadius = radius;
        while (prep)
        {
            currentAngle += angularSpeed * Time.deltaTime * motionDir;
            if (charge && t < chargeDuration)
            {
                t += Time.deltaTime;
                currentRadius = Mathf.Lerp(radius, minRadius, t / chargeDuration);
            }

            Vector3 circlePos = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle) / inclination) * currentRadius;
            Vector3 nextPosition = destroyer.transform.position + circlePos;
            transform.position = nextPosition;
            Vector3 dir = (destroyer.transform.position - transform.position).normalized;
            float rot_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot_z - 270);

            if (t > chargeDuration)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (transform.position - destroyer.transform.position).normalized,1000,LayerMask.GetMask("Player"));
                if (hit.collider != null)
                {
                    foreach (SwordDestroyer sword in swordList)
                    {
                        sword.ReleaseSword(destroyer.swordsCircleStats.castDelay, destroyer.swordsCircleStats.alertDuration, destroyer.swordsCircleStats.swordSpeed, destroyer.swordsCircleStats.swordDuration, destroyer);
                    }
                }
            }
            

            yield return null;
        }

    }

    private IEnumerator IRelease(float releaseDelay, float alertDuration, float speed, float duration, Destroyer destroyer)
    {
        LineRenderer lineInstance = Instantiate(line);
        Vector3 dir = (transform.position - destroyer.transform.position).normalized;
        List<Vector3> linePoints = new List<Vector3> { transform.position, transform.position + dir * 1000 };
        lineInstance.positionCount = linePoints.Count;
        lineInstance.SetPositions(linePoints.ToArray());
        lineInstance.enabled = true;
        float t = 0, startAlpha = lineInstance.startColor.a;
        while(t < alertDuration)
        {
            t += Time.deltaTime;
            lineInstance.startColor = new Color(lineInstance.startColor.r, lineInstance.startColor.g, lineInstance.startColor.b, Mathf.Lerp(startAlpha, 0, t / alertDuration));
            yield return null;
        }
        Destroy(lineInstance.gameObject);
        trail.emitting = true;
        trail.AddPosition(transform.position);
        yield return new WaitForSeconds(releaseDelay);

        swordCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (transform.position - destroyer.transform.position).normalized, 1000, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            player.Hit(this);
        }
        yield return null;

        t = 0;
        while(t < duration)
        {
            t += Time.deltaTime;
            transform.position += speed * Time.deltaTime * dir;
            yield return null;
        }
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Player>().Hit(this);
        }
    }
}
