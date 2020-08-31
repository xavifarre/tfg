using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLimits : MonoBehaviour
{
    public int area = 0;
    public bool hasLimits = false;
    [HideInInspector]
    public List<Vector2> limits = new List<Vector2>();
    public bool box = true;
    private CameraManager cm;

    private PolygonCollider2D boundsPolygon;

    // Start is called before the first frame update
    void Start()
    {
        cm = FindObjectOfType<CameraManager>();

        limits = new List<Vector2>();
        foreach (Transform child in transform)
        {
            limits.Add(child.position);
        }

        if (hasLimits && !box)
        {
            GameObject child = transform.GetChild(0).gameObject;
            boundsPolygon = child.AddComponent<PolygonCollider2D>();
            child.layer = LayerMask.NameToLayer("IgnoreAll");
        }
    }
    private void Update()
    {
        limits = new List<Vector2>();
        foreach (Transform child in transform)
        {
            limits.Add(child.position);
        }
        if (boundsPolygon)
        {
            for (int i = 0; i < limits.Count; i++)
            {
                limits[i] = limits[i] - (Vector2)boundsPolygon.transform.position;
            }
            boundsPolygon.points = limits.ToArray();
        } 
    }

    public bool IsOutsideLimit(Vector3 point)
    {
        return !Poly.ContainsPoint(limits.ToArray(), point);
    }


    public Vector3 GetClosestPoint(Vector3 pos)
    {
        Vector2 closestPoint = boundsPolygon.ClosestPoint(pos);
        return new Vector3(closestPoint.x, closestPoint.y, pos.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            cm.AreaChange(area);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hasLimits)
        {
            List<Vector2> lim = new List<Vector2>();
            foreach (Transform child in transform)
            {
                lim.Add(child.position);
            }
            if (box)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(lim[0] + (lim[1] - lim[0]) / 2, lim[1] - lim[0]);
            }
            else
            {
                for (int i = 0; i < lim.Count; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(lim[i], lim[(i + 1) % lim.Count]);
                }
            }
        }
    }
}
