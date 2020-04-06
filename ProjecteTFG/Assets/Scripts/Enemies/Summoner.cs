using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Summoner : Boss
{
    public GameObject SpawnersContainer;
    private List<SpawnerGroup> faseSpawners;

    //Punts de moviment
    private List<List<Vector3>> movementPoints;

    //Next point
    private int nextPoint;
    private int previousPoint;
    public float antiRepeatWheight = 0.5f;

    protected override void Init()
    {
        InitMovementPoints();
        InitSpawners();

        nextPoint = Random.Range(0, movementPoints[fase].Count);

        //StartCoroutine(MoveBetweenPoints());
    }

    protected override void UpdateEnemy()
    {
        
    }

    void FixedUpdate()
    {
        //Move
        Vector3 direction = (movementPoints[fase][nextPoint] - realPos).normalized;
        realPos = realPos + speed * direction * Time.fixedDeltaTime;
        if (Vector3.Distance(movementPoints[fase][nextPoint], realPos) < 0.5f)
        {
            realPos = movementPoints[fase][nextPoint];
            nextPoint = RandomAdjacentPoint();
        }
        PixelPerfectMovement.Move(realPos, rb);
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

    private void InitSpawners()
    {
        faseSpawners = new List<SpawnerGroup>();
        foreach(Transform child in transform)
        {
            faseSpawners.Add(child.GetComponent<SpawnerGroup>());
        }
    }

    private int RandomAdjacentPoint()
    {
        int randomPoint = Random.Range(0, movementPoints[fase].Count);
        while(!IsAdjacent(randomPoint) || WeightRepeat(randomPoint))
        {
            randomPoint = Random.Range(0, movementPoints[fase].Count);
        }

        previousPoint = nextPoint;

        return randomPoint;
    }

    private bool IsAdjacent(int point)
    {
        if(fase == 0)
        {
            return point == (nextPoint + 1)% movementPoints[fase].Count || point == (nextPoint - 1) % movementPoints[fase].Count;
        }
        if (fase == 1)
        {
            return point == (nextPoint + 1) % movementPoints[fase].Count || point == (nextPoint - 1) % movementPoints[fase].Count;
        }
        if (fase == 2)
        {
            return point % (movementPoints[fase].Count / 2) == (nextPoint + 1)% (movementPoints[fase].Count/2) || point % (movementPoints[fase].Count / 2) == (nextPoint - 1) % (movementPoints[fase].Count/2);
        }
        return false;
    }

    private bool WeightRepeat(int index)
    {
        return index == previousPoint && Random.Range(0, 100) / 100f < antiRepeatWheight;
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
