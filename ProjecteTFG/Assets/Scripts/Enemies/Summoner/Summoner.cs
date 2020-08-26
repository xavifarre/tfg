using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Summoner : Boss
{

    [Header("Lunge")]
    //Lunge
    public int lungeDamage = 20;
    public float lungeKnockback = 3;
    public float lungeSpeed = 20;
    private Vector3 lungeDest;
    public float lungeDistance = 20;
    //Temps que ha de passar des de que el player o un enemic es atacat per a realitzar el lunge
    public float lastHitTimeConstraint = 1;
    //Variable que conta quantes vegades es repeteix consecutivament una acció. Utilitzada per a triggejar el lunge quan fa molt que no el fa
    public int sameActionLimit = 2;
    private int sameActionCounter = 0;
    public Attack lungeObject;

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
    public float lastFaseDashAttackProbability = 0.5f;

    [Header("Summon")]
    //Summon
    public List<float> summonTime = new List<float> { 4, 4, 4, 3 } ;
    public List<float> summonPreparationTime = new List<float> { 1, 1, 1, 1 };
    private enum SummType { Normal, Hit, StartFase };
    private SummType summonType;

    //Spawners
    public GameObject spawnersContainer;
    private List<SpawnerGroup> faseSpawners;

    //EnemiesContainer
    public int maxEnemies = 12;
    private GameObject enemiesContainer;

    public ElectricitySummon electricitySummon;
    private List<int> summonColors;

    //State
    public enum SummState { Idle, Move, Lunge, Dash, DashAttack, Summon, Melee, Damaged, Die, Start};
    public SummState state;

    [Header("Float")]
    public float floatAmplitude;
    public float floatAngularSpeed;

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

    //Knockback
    public float knockBackHit = 2;
    public float knockbackTime = 0.5f;

    //Shadow
    public ShadowController shadowController;

    //Materials
    [Header("Materials")]
    public Material disolveMaterial;

    [Header("Particles")]
    public ParticlePlayer dieParticles;
    public ParticleSystem moveParticles;
    public ParticleSystem dashParticles;
    public ParticleSystem lungeParticles;
    public TrailRenderer lungeTrail;

    protected override void Init()
    {
        enemiesContainer = GameObject.Find("Enemies");
        meleeObject = GetComponentInChildren<SummonerMeleeAttack>();

        defaultMaterial = spriteRenderer.material;
        InitMovementPoints();
        InitSpawners();
        firstHit = false;

        StartCoroutine(IPresentation());
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
            UpdateMove(2);
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
        tAction += Time.deltaTime;
        Vector3 direction = (movementPoints[fase][nextPoint] - realPos).normalized;
        realPos = realPos + (speed + speed * speedFaseMultiplier * fase) * spMulti * direction * Time.deltaTime;

        Vector3 floatingPosition = realPos + FloatY() * Vector3.up;
        shadowController.height = FloatY();

        if (Vector3.Distance(movementPoints[fase][nextPoint], realPos) < 0.5f)
        {
            EndMove();
        }

        PixelPerfectMovement.Move(floatingPosition, rb);
    }

    //Fa flotar el enemic en forma de funció sinusoidal
    private float FloatY()
    {
        return floatAmplitude * Mathf.Sin(floatAngularSpeed * tAction * Mathf.PI);
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

    //Knockback
    private IEnumerator IKnockBack()
    {
        float t = 0;
        while(t < timeKnockback)
        {
            t += Time.deltaTime;
            Vector3 nextPos = MathFunctions.EaseOutExp(t, startKnockback, endKnockback, timeKnockback, 5);
            realPos = nextPos;
            PixelPerfectMovement.Move(nextPos, rb);
            yield return null;
        }
    }

    private void StartMove()    
    {
        tAction = 0;
        animator.SetTrigger("Move");
        moveParticles.Play();
        ResetLayer();
        float nextXMovement = (movementPoints[fase][nextPoint] - realPos).x;
        //UpdateAnimFlip(nextXMovement);
        StartCoroutine(ICompleteAnim(Move));
    }

    private void Move()
    {
        firstHit = true;
        state = SummState.Move;
        sameActionCounter++;
    }

    private void EndMove()
    {
        ChangeLayerIgnore();
        state = SummState.Idle;
        moveParticles.Stop();
        if (CheckDamageFase() != fase)
        {
            StartFase(CheckDamageFase());
        }
        else
        {
            int prevPoint = previousPoint;
            int actualPoint = nextPoint;
            nextPoint = RandomAdjacentPoint();

            animator.SetTrigger("EndAction");
            animator.SetBool("EndMoveReversed", actualPoint % 2 != 0 || actualPoint % 2 == 0 && prevPoint == nextPoint);

            StartCoroutine(ICompleteAnim(EndMoveFinished));
        }
    }

    private void EndMoveFinished()
    {
       
        if (enemiesContainer.transform.childCount < maxEnemies)
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
            return point == MathFunctions.Mod((nextPoint + 1), movementPoints[fase].Count) || point == MathFunctions.Mod((nextPoint - 1), movementPoints[fase].Count);
        }
        if (fase == 1)
        {
            return point == MathFunctions.Mod((nextPoint + 1), movementPoints[fase].Count) || point == MathFunctions.Mod((nextPoint - 1), movementPoints[fase].Count);
        }
        if (fase == 2)
        {
            return MathFunctions.Mod(point , (movementPoints[fase].Count / 2)) == MathFunctions.Mod((nextPoint + 1), (movementPoints[fase].Count / 2)) || MathFunctions.Mod(point, (movementPoints[fase].Count / 2)) == MathFunctions.Mod((nextPoint - 1), (movementPoints[fase].Count/2));
        }
        return true;
    }

    private bool WeightRepeat(int index)
    {
        return index == previousPoint && Random.Range(0, 100) / 100f < antiRepeatWeight;
    }

    private void PrepareLunge(bool startFaseLunge = false)
    {
        sameActionCounter = 0;
        lungeDest = realPos + (player.transform.position - realPos).normalized * lungeDistance;

        StartCoroutine(IPrepareLunge(startFaseLunge));
    }

    private void StartLunge()
    {
        lungeDest = realPos + (player.transform.position - realPos).normalized * lungeDistance;
        lungeParticles.Play();
        lungeTrail.emitting = true;
        lungeObject.gameObject.SetActive(true);
        lungeObject.damage = lungeDamage;
        lungeObject.knockback = lungeKnockback;
        ResetLayer();
        state = SummState.Lunge;
    }

    private void EndLunge()
    {
        state = SummState.Idle;
        lungeParticles.Stop();
        lungeTrail.emitting = false;
        lungeObject.gameObject.SetActive(false);
        animator.SetTrigger("EndAction");
        nextPoint = NearestPoint();

        StartCoroutine(ICompleteAnim(RecoverLunge));
    }

    private void RecoverLunge()
    {
        animator.SetBool("EndMoveReversed", HasToFlip((movementPoints[fase][nextPoint] - realPos).x));
        animator.SetTrigger("EndAction");
        StartCoroutine(ICompleteAnim(StartMove));
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

        ChangeLayerIgnore();

        StartDashAnim();
    }

    private void DashAttack()
    {
        dashOrigin = realPos;
        Vector2 randomOffset = new Vector2(Random.Range(-100000,100000)/100000f, Random.Range(-100000, 100000) / 100000f).normalized * meleeRange;

        dashDest = player.transform.position + (Vector3)randomOffset;

        tAction = 0;
        state = SummState.DashAttack;

        ChangeLayerIgnore();

        StartDashAnim();

    }

    private void StartDashAnim()
    {
        animator.SetTrigger("Dash");
        
        float nextXMovement = (dashDest - realPos).x;
        UpdateAnimFlip(nextXMovement);

        PlayDashParticles(realPos, dashDest);

        float xDest = (player.transform.position - dashDest).x, xMov = (dashDest - dashOrigin).x;
        bool hasToReverse = xDest > 0 && xMov < 0 || xDest < 0 && xMov > 0;


        StartCoroutine(IDashReverse(hasToReverse));
        StartCoroutine(IDashDisolve());
    }

    private void EndDash()
    {
        dashParticles.Stop();
        ResetLayer();

        if (state == SummState.Dash)
        {
            state = SummState.Idle;
            CallStopForSeconds();

        }
        else
        {
            state = SummState.Melee;
            StartCoroutine(ITriggerMelee());
        }
    }

    private void PlayDashParticles(Vector2 from, Vector2 to)
    {
        ParticleSystem.MainModule main = dashParticles.main;
        main.startRotation = Vector2.SignedAngle(to - from, Vector2.right) * Mathf.Deg2Rad;

        dashParticles.Play();
    }

    private void CallStopForSeconds()
    {
        StopForSeconds(idleTime);
    }

    private void StartMelee()
    {
        state = SummState.Melee;
        StartCoroutine(IMeleeAttack());      
    }


    private void Melee()
    {
        int dir = MathFunctions.GetDirection(player.transform.position - realPos);

        animator.SetTrigger("Slash");
        animator.SetInteger("SlashDirection", dir);
        float x = (player.transform.position - realPos).x;
        UpdateAnimFlip(x);
    }

    public void PerformMelee()
    {
        meleeObject.PerformAttack(animator.GetInteger("SlashDirection"));
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
        summonColors = new List<int>();
    }

    public void Summon()
    {
        if (summonType == SummType.Normal)
        {
            faseSpawners[fase].StartSpawning();
        }
        else if (summonType == SummType.Hit)
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
        ResetLayer();
        NextAction();
        firstHit = true;
        summonType = SummType.Normal;
    }

    public void NotifySummon(int type)
    {
        summonColors.Add(type);
        if(summonColors.Count == 1)
        {
            StartCoroutine(IWaitSummonColors());
        }
    }

    private IEnumerator IWaitSummonColors()
    {
        float t = 0;
        while(t < 0.5f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        if(summonColors.Count == 1)
        {
            electricitySummon.PlayAnim(summonColors[0], summonColors[0]);
        }
        else
        {
            electricitySummon.PlayAnim(summonColors[0], summonColors[1]);
        }
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
                PrepareLunge();
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
                    StartMelee();
                }
                else
                {
                    DashAttack();
                }
            }
            else if(enemiesContainer.transform.childCount < maxEnemies && state != SummState.Summon)
            {
                if (Random.value > lastFaseDashAttackProbability)
                {
                    StartSummon();
                }
                else
                {
                    DashAttack();
                }
                    
            }
            else
            {
                if (Random.value > lastFaseDashAttackProbability)
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
        lungeObject.gameObject.SetActive(false);

        moveParticles.Stop();
        lungeParticles.Stop();
        lungeTrail.emitting = false;

        if (fase < 3)
        {
            if (firstHit == true)
            {
                firstHit = false;
                vulnerable = true;
                state = SummState.Idle;

                UpdateAnimFlip(player.transform.position.x - realPos.x);

                KnockBack(knockBackHit, knockbackTime);
                StartCoroutine(IKnockBack());

                animator.SetTrigger("Damage");

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
        else
        {
            GetDamage(attack.damage);
        }
    }

    private IEnumerator IPresentation()
    {
        gm.BlockInputs(true);

        //Initialize boss
        ChangeLayerIgnore();
        transform.position = movementPoints[0][0];
        realPos = transform.position;
        nextPoint = RandomAdjacentPoint();
        float nextXMovement = (movementPoints[fase][nextPoint] - realPos).x;
        UpdateAnimFlip(nextXMovement);

        ScreenManager.instance.StartFadeShowScreen(5, 2);
        yield return new WaitForSeconds(1f);
        CameraManager.instance.mainCamera.SetDestination(transform.position, 3);

        yield return new WaitForSeconds(5f);

        StartFase(0);
        yield return new WaitForSeconds(4f);
        gm.BlockInputs(false);
        CameraManager.instance.mainCamera.FollowPlayer(1f);
    }


    protected override void StartFase(int f)
    {
        fase = f;
        if(fase == 0)
        {
            StartSummon();
        }
        else if (fase == 1)
        {
            PrepareLunge(true);
        }
        else if (fase == 2)
        {
            PrepareLunge(true);
        }
        else if (fase == 3)
        {
            summonType = SummType.StartFase;
            animator.SetTrigger("EndAction");
            StartSummon();
            DashToRandom();
        }
    }

    public override void Die()
    {
        StopAllCoroutines();
        base.Die();
        state = SummState.Die;
        ChangeLayerIgnore();
        animator.SetTrigger("Die");

        KnockBack(3, 0.5f);
        StartCoroutine(IKnockBack());

        gm.SlowDownGame(killSlowScale, killSlowTime);

        KillAllMinions();
        
        Globals.gameState = GameState.SummonerDefeated;
    }

    public void DieAnim()
    {
        Time.timeScale = 1f;
        FadeShadow();
        StartCoroutine(IDie());

        Instantiate(dieParticles, transform.position, Quaternion.Euler(-90, 0, 0));
    }

    private void KillAllMinions()
    {
        foreach(Minion minion in enemiesContainer.transform.GetComponentsInChildren<Minion>())
        {
            minion.Die();
        }
    }

    public void UpdateAnimFlip(float nextXMovement)
    {
        if (nextXMovement > 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    public bool HasToFlip(float nextXMovement)
    {
        return nextXMovement < 0 && spriteRenderer.flipX || nextXMovement > 0 && !spriteRenderer.flipX;
    }


    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
    

    private IEnumerator IVulnerable()
    {
        yield return new WaitForSeconds(vulnerableTime);
        state = SummState.Damaged;
        UpdateAnimFlip(movementPoints[fase][nextPoint].x - realPos.x);
        moveParticles.Play();
        vulnerable = false;
        ChangeLayerIgnore();
    }

    private IEnumerator ISummon()
    {
        yield return new WaitForSeconds(summonPreparationTime[fase]);
        animator.SetTrigger("Summon");
        yield return new WaitForSeconds(summonTime[fase]);
        EndSummon();
    }

    private IEnumerator IStopForSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        if(state == SummState.Idle)
        {
            NextAction();
        }
    }

    private IEnumerator ITriggerMelee()
    {
        AnimatorClipInfo[] clipInfo;
        clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        while (clipInfo[0].clip.name != "idle")
        {
            clipInfo = animator.GetCurrentAnimatorClipInfo(0);

            yield return null;
        }

        StartMelee();
    }

    private IEnumerator IMeleeAttack()
    {
        yield return new WaitForSeconds(meleeDelay);
        if (state == SummState.Melee)
        {
            Melee();
        }
        yield return new WaitForSeconds(meleeInvulnerableTime);
        firstHit = true;
        yield return new WaitForSeconds(meleeRecoveryTime);
        if (state == SummState.Melee)
        {
            
            DashToRandom();
        }
    }

    private IEnumerator ICompleteAnim(System.Action nextAction)
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);
        nextAction();
    }



   private  IEnumerator IPrepareLunge(bool startFaseLunge)
    {
        AnimatorClipInfo[] clipInfo;
        if (startFaseLunge)
        {
            animator.SetTrigger("EndAction");
            animator.SetBool("EndMoveReversed", HasToFlip((lungeDest - realPos).x));
            clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            yield return new WaitForSeconds(clipInfo[0].clip.length);
            yield return null; 
        }
        animator.SetTrigger("Move");
        clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);
        yield return null;
        animator.SetTrigger("Lunge");
        clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(clipInfo[0].clip.length);
        yield return null;
        StartLunge();
    }

    private IEnumerator IDashReverse(bool hasToReverse)
    {
        float dashAnimDuration = (0.834f/2) / 1.5f;
        yield return new WaitForSeconds(dashAnimDuration);
        if (hasToReverse)
        {
            Flip();
        }
    }

    private IEnumerator IDashDisolve()
    {
        float t = 0;
        spriteRenderer.material = disolveMaterial;
        float dashAnimDuration = 0.834f / 1.5f;
        while (t < dashAnimDuration)
        {
            t += Time.deltaTime;
            if(t < dashAnimDuration / 2)
            {
                spriteRenderer.material.SetFloat("_Fade", Mathf.Lerp(1, 0, t / (dashAnimDuration / 2)));
            }
            else
            {
                spriteRenderer.material.SetFloat("_Fade", Mathf.Lerp(0, 1, (t - dashAnimDuration / 2) / (dashAnimDuration / 2)));
            }
            yield return null;
        }
        spriteRenderer.material = defaultMaterial;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lungeDest, 1);
    }

}
