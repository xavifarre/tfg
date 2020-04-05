using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Summoner : Boss
{
    private List<List<Vector3>> movementPoints;

    protected override void Init()
    {
        InitMovementPoints();
        StartCoroutine(MoveBetweenPoints());
    }


    private void InitMovementPoints()
    {
        GameObject mp = GameObject.Find("MovementPoints");

        movementPoints = new List<List<Vector3>>();

        for(int i = 0; i < 4; i++)
        {
            movementPoints.Insert(i,new List<Vector3>());
            foreach(Transform t in mp.transform.Find(i.ToString()))
            {
                movementPoints[i].Add(t.position);
            }
        }
    }

    IEnumerator MoveBetweenPoints()
    {
        while (true)
        {
            transform.position = movementPoints[fase][Random.Range(0, movementPoints[fase].Count)];
            yield return new WaitForSeconds(0.3f);
        }
    }
}
