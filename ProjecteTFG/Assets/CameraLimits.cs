using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLimits : MonoBehaviour
{
    public int area = 0;
    public GameObject limitContainer;
    [HideInInspector]
    public List<Vector2> limits = new List<Vector2>();
    public bool box = true;

    // Start is called before the first frame update
    void Start()
    {

    }
    private void Update()
    {
        limits = new List<Vector2>();
        foreach (Transform child in limitContainer.transform)
        {
            limits.Add(child.position);
        }
    }
    public bool IsOutsideLimit(Vector3 point)
    {
        return !Poly.ContainsPoint(limits.ToArray(), point);
    }

    private void OnDrawGizmosSelected()
    {
        List<Vector2> lim = new List<Vector2>();
        foreach (Transform child in limitContainer.transform)
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
