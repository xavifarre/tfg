﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Summoner : Boss
{

    [Header("Lunge")]
    //Lunge
    public float lungeSpeed = 20;
    private Vector3 lungeDest;
    public float lungeDistance = 20;
    //Temps que ha de passar des de que el player o un enemic es atacat per a realitzar el lunge
    public float lastHitTimeConstraint = 1;
    //Variable que conta quantes vegades es repeteix consecutivament una acció. Utilitzada per a triggejar el lunge quan fa molt que no el fa
    public int sameActionLimit = 2;
    private int sameActionCounter = 0;

    [Header("Dash")]
    //Dash
    public float dashDuration;
    private Vector3 dashOrigin;
    private Vector3 dashDest;
    public float dashMinimumDistance = 20;

    [Header("Melee attack")]
    //Melee
    public float meleeRange = 3f;
    public int meleeDamage = 1;
    public float meleeDelay = 0.5f;
    public float meleeInvulnerableTime = 0.5f;
    public float meleeRecoveryTime = 1f;
    private SummonerMeleeAttack meleeObject;

    [Header("Summon")]
    //Summon
    public List<float> summonTime = new List<float> { 5, 4, 3, 2 } ;
    private enum SummType { Normal, Hit, StartFase };
    private SummType summonType;

    //Spawners
    public GameObject spawnersContainer;
    private List<SpawnerGroup> faseSpawners;

    //EnemiesContainer
    public int maxEnemies = 12;
    private GameObject enemiesContainer;

    //State
    public enum SummState { Idle, Move, Lunge, Dash, DashAttack, Summon, Melee, Damaged, Die, Start};
    [HideInInspector]
    public SummState state;


    [Header("Misc")]
    //Speed modifier
    public float speedFaseMultiplier = 0.2f;

    //Idle
    public float idleTime = 2f;

    //Vulnerable
    public float vulnerableTime = 1f;
    private bool firstHit = true;

    //Punts de moviment
    private List<List<Vector3>> movementPoints;
    private RectArea areaMove;

    //Next point
    private int nextPoint;
    private int previousPoint;
    public float antiRepeatWeight = 0.5f;

    //Animator
    private Animator animator;

    
    protected override void Init()
    {
        enemiesContainer = GameObject.Find("Enemies");
        meleeObject = GetComponentInChildren<SummonerMeleeAttack>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitMovementPoints();
        InitSpawners();

        StartFase(fase);
    }

    protected override void UpdateEnemy()
    {
        if (state == SummState.Move)
        {
            UpdateMove();
        }
        if (state == SummState.Lunge)
        {
            UpdateLunge();
        }
        if (state == SummState.Dash || state == SummState.DashAttack)
        {
            UpdateDash();
        }
        if (state == SummState.Damaged)
        {
            if(fase < 3)
            {
                UpdateMove(2);
            }
            else
            {
                DashToRandom();
                firstHit = true;
                gameObject.layer = LayerMask.NameToLayer("Boss");
            }
          
        }
    }

    void FixedUpdate()
    {

    }

    private void InitMovementPoints()
    {
        GameObject mp = GameObject.Find("MovementPoints");

        movementPoints = new List<List<Vector3>>();

        for (int i = 0; i < 4; i++)
        {
            movementPoints.Insert(i, new List<Vector3>());
            foreach (Transform t in mp.transform.Find(i.ToString()))
            {
                movementPoints[i].Add(t.position);
            }
        }

        areaMove = mp.transform.Find("3").GetComponent<RectArea>();

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
    private void UpdateMove(float spMulti = 1)
    {
        Vector3 direction = (movementPoints[fase][nextPoint] - realPos).normalized;
        realPos = realPos + (speed + speed * speedFaseMultiplier * fase) * spMulti * direction * Time.deltaTime;

        if (Vector3.Distance(movementPoints[fase][nextPoint], realPos) < 0.5f)
        {
            EndMove();
        }

        PixelPerfectMovement.Move(realPos, rb);
    }

    //Lunge
    private void UpdateLunge()
    {
        Vector3 direction = (lungeDest - realPos).normalized;
        realPos = realPos + lungeSpeed * direction * Time.deltaTime;

        if (Vector3.Distance(lungeDest, realPos) < 0.5f)
        {   
            EndLunge();
        }

        PixelPerfectMovement.Move(realPos, rb);
    }

    //Dash
    private void UpdateDash()
    {
        tAction += Time.deltaTime;
        realPos = MathFunctions.EaseOutExp(tAction, dashOrigin, dashDest, dashDuration, 5);

        PixelPerfectMovement.Move(realPos, rb);

        if (tAction >= dashDuration)
        {
            EndDash();
        }

    }

    private void StartMove()
    {
        animator.SetTrigger("Move");
        float nextXMovement = (movementPoints[fase][nextPoint] - realPos).x;
        UpdateAnimFlip(nextXMovement);
        StartCoroutine(ICompleteAnim(Move));
    }

    private void Move()
    {
        state = SummState.Move;
        sameActionCounter++;
    }

    private void EndMove()
    {
        int prevPoint = previousPoint;
        int actualPoint = nextPoint;
        nextPoint = RandomAdjacentPoint();

        animator.SetTrigger("EndAction");
        animator.SetBool("EndMoveReversed", actualPoint % 2 != 0 || actualPoint % 2 == 0 && prevPoint == nextPoint);

        StartCoroutine(ICompleteAnim(EndMoveFinished));
    }

    private void EndMoveFinished()
    {
        if (CheckDamageFase() != fase)
        {
            StartFase(CheckDamageFase());
        }
        else if (enemiesContainer.transform.childCount < maxEnemies)
        {
            StartSummon();
        }
        else
        {
            StopForSeconds(idleTime);
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
        return index == previousPoint && Random.Range(0, 100) / 100f < antiRepeatWeight;
    }

    private void StartLunge()
    {
        
        sameActionCounter = 0;
        lungeDest = realPos + (player.transform.position - realPos).normalized * lungeDistance;

        //StartCoroutine(IPrepLounge());
    }

    private void Lunge()
    {
        state = SummState.Lunge;
    }

    private void EndLunge()
    {
        nextPoint = NearestPoint();
        state = SummState.Move;

        animator.SetTrigger("EndAction");
    }

    private void DashToRandom()
    {
        dashOrigin = realPos;
        dashDest = areaMove.RandomPoint();
        while (Vector3.Distance(dashDest, realPos) < dashMinimumDistance)
        {
            dashDest = areaMove.RandomPoint();
        }
        tAction = 0;
        state = SummState.Dash;

        animator.SetTrigger("Dash");
        float nextXMovement = (dashDest - realPos).x;
        UpdateAnimFlip(nextXMovement);
    }

    private void DashAttack()
    {
        dashOrigin = realPos;
        dashDest = areaMove.RandomPoint();
        int n = 0;
        while (Vector3.Distance(dashDest, realPos) < dashMinimumDistance / 2 || Vector3.Distance(player.transform.position, dashDest) > meleeRange)
        {
            if (n > 30)
            {
                break;
            }
            n++;
            dashDest = areaMove.RandomPoint();
        }

        if (n > 30)
        {
            DashToRandom();
        }
        else
        {
            tAction = 0;
            state = SummState.DashAttack;

            animator.SetTrigger("Dash");
            float nextXMovement = (dashDest - realPos).x;
            UpdateAnimFlip(nextXMovement);
        }
    }

    private void EndDash()
    {
        if(state == SummState.Dash)
        {
            animator.SetTrigger("EndAction");
            StopForSeconds(idleTime);
        }
        else
        {
            Melee();
        }
    }

    private void Melee()
    {
        state = SummState.Melee;
        Vector2 dir = player.transform.position - realPos;
        StartCoroutine(IMeleeAttack(dir));

        animator.SetTrigger("Slash");
        animator.SetInteger("SlashDirection", MathFunctions.GetDirection(dir));
        int x = MathFunctions.GetDirection(dir) == 1 ? -1 : 1;
        UpdateAnimFlip(x);
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

    public void StartSummon()
    {
        state = SummState.Summon;

        StartCoroutine(ISummon());
    }

    public void Summon()
    {
        if (summonType == SummType.Normal)
        {
            faseSpawners[fase].StartSpawning();
        }
        else if(summonType == SummType.Hit)
        {
            faseSpawners[fase].StartSpawningHit();
        }
        else if (summonType == SummType.StartFase)
        {
            faseSpawners[fase].StartSpawningStart();
        }

        animator.SetTrigger("EndAction");
        animator.SetInteger("Fase", fase);
    }

    private void EndSummon()
    {
        NextAction();
        firstHit = true;
        summonType = SummType.Normal;
        gameObject.layer = LayerMask.NameToLayer("Boss");
    }

    private void NextAction()
    {
        if(fase == 0)
        {
            StartMove();
        }
        else if(fase == 1 || fase == 2)
        {
            if (CanLunge())
            {
                StartLunge();
            }
            else
            {
                StartMove();
            }
        }
        else if (fase == 3)
        {
            if(Vector3.Distance(player.transform.position, realPos) < meleeRange)
            {
                if(Random.Range(0,2) == 0)
                {
                    Melee();
                }
                else
                {
                    DashAttack();
                }
            }
            else if(enemiesContainer.transform.childCount < maxEnemies && state != SummState.Summon)
            {
                StartSummon();
            }
            else
            {
                if (Random.Range(0, 3) == 0)
                {
                    DashToRandom();
                }
                else
                {
                    DashAttack();
                }
                
            }
        }
    }

    private bool CanLunge()
    {
        if(gm.tLastHit > lastHitTimeConstraint || sameActionCounter >= sameActionLimit)
        {
            return true;
        }
        return false;
    }

    private void StopForSeconds(float s)
    {
        state = SummState.Idle;
        StartCoroutine(IStopForSeconds(s));
    }

    public override void Hit(Attack attack)
    {
        if (firstHit == true)
        {
            firstHit = false;
            vulnerable = true;
            state = SummState.Idle;

            if (fase < 3)
            {
                animator.SetTrigger("Damage");
            }

            StartCoroutine(IVulnerable());
        }

        if (vulnerable)
        {
            GetDamage(attack.damage);
            summonType = SummType.Normal;
        }
        else
        {
            GetDamage(0);
        }
    }

    private void Presentation()
    {
        transform.position = movementPoints[0][0];
        realPos = transform.position;
        StopForSeconds(2);
    }

    protected override void StartFase(int f)
    {
        fase = f;
        if(fase == 0)
        {
            Presentation();
        }
        else if (fase == 1)
        {
            StartLunge();
        }
        else if (fase == 2)
        {
            StartLunge();
        }
        else if (fase == 3)
        {
            summonType = SummType.StartFase;
            StartSummon();
        }
    }

    public void UpdateAnimFlip(float nextXMovement)
    {
        Debug.Log(nextXMovement);
        if (nextXMovement > 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
    

    IEnumerator IVulnerable()
    {
        yield return new WaitForSeconds(vulnerableTime);
        vulnerable = false;
        state = SummState.Damaged;
        gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
    }

    IEnumerator ISummon()
    {
        yield return new WaitForSeconds(summonTime[fase] / 2);
        animator.SetTrigger("Summon");
        yield return new WaitForSeconds(summonTime[fase] / 2);
        EndSummon();
    }

    IEnumerator IStopForSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        if(state == SummState.Idle)
        {
            NextAction();
        }
    }

    IEnumerator IMeleeAttack(Vector2 dir)
    {
        firstHit = false;
        yield return new WaitForSeconds(meleeDelay);
        if (state == SummState.Melee)
        {
            meleeObject.PerformAttack(dir);
        }
        yield return new WaitForSeconds(meleeInvulnerableTime);
        firstHit = true;

        yield return new WaitForSeconds(meleeRecoveryTime);
        if (state == SummState.Melee)
        {
            DashToRandom();
        }

    }

    IEnumerator ICompleteAnim(System.Action nextAction)
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);
        nextAction();
    }



    IEnumerator IPrepareLunge()
    {
        if((lungeDest - realPos).x > 0)
        {
            animator.SetTrigger("EndMoveReversed");
        }

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);
        Move();
    }

    //animator.SetTrigger("Lunge");
    //    float nextXMovement = (lungeDest - realPos).x;
    //UpdateAnimFlip(nextXMovement);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lungeDest, 1);
    }

}