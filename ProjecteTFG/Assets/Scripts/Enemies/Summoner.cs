using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Summoner : Boss
{
    public GameObject spawnersContainer;
    private List<SpawnerGroup> faseSpawners;

    //Punts de moviment
    private List<List<Vector3>> movementPoints;

    //Dash
    public float dashSpeed = 20;
    private Vector3 dashDest;
    public float dashExtraDistance = 20;

    //Summon
    public List<float> summonTime = new List<float> { 5, 4, 3, 2 } ;

    //State
    public enum SummState { Idle, Move, Dash, Summon, Melee, Die, Start};

    public SummState state;

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
        if (state == SummState.Move)
        {
            UpdateMove();
        }
        if (state == SummState.Dash)
        {
            UpdateDash();
        }
        
    }

    void FixedUpdate()
    {

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
        foreach(Transform child in spawnersContainer.transform)
        {
            faseSpawners.Add(child.GetComponent<SpawnerGroup>());
        }
    }

    //Move
    private void UpdateMove()
    {
        Vector3 direction = (movementPoints[fase][nextPoint] - realPos).normalized;
        realPos = realPos + speed * direction * Time.deltaTime;

        if (Vector3.Distance(movementPoints[fase][nextPoint], realPos) < 0.5f)
        {
            EndMove();
        }

        PixelPerfectMovement.Move(realPos, rb);
    }

    //Dash
    private void UpdateDash()
    {
        Vector3 direction = (dashDest - realPos).normalized;
        realPos = realPos + dashSpeed * direction * Time.deltaTime;

        if (Vector3.Distance(dashDest, realPos) < 0.5f)
        {   
            EndDash();
        }

        PixelPerfectMovement.Move(realPos, rb);
    }

    private void EndMove()
    {
        if (Random.Range(0, 100) < 0)
        {
            StartDash();
        }
        else
        {
            Summon();
            nextPoint = RandomAdjacentPoint();
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
        return true;
    }

    private bool WeightRepeat(int index)
    {
        return index == previousPoint && Random.Range(0, 100) / 100f < antiRepeatWheight;
    }

    private void StartDash()
    {
        state = SummState.Dash;
        dashDest = player.transform.position + (player.transform.position - realPos).normalized * dashExtraDistance;
    }

    private void EndDash()
    {
        nextPoint = NearestPoint();
        state = SummState.Move;
    }

    //Retorna el punt més proper al boss
    private int NearestPoint()
    {
        Vector3 nearest = movementPoints[fase][0];
        foreach(Vector3 p in movementPoints[fase])
        {
            if(Vector3.Distance(p,realPos) < Vector3.Distance(nearest, realPos))
            {
                nearest = p;
            }
        }
        return movementPoints[fase].IndexOf(nearest);
    }

    public void Summon()
    {
        faseSpawners[fase].StartSpawning();
        state = SummState.Summon;
        StartCoroutine(ISummon());
    }

    IEnumerator ISummon()
    {
        yield return new WaitForSeconds(summonTime[fase]);
        state = SummState.Move;
    }

    public override void Hit(Attack attack)
    {
        GetDamage(attack.damage);
        if(CheckDamageFase() != fase && health > 0)
        {
            StartFase(CheckDamageFase());
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(dashDest, 1);
    }

}
