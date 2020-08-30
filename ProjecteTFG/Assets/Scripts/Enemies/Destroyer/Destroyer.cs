using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Destroyer : Boss, IState
{
    #region Variables

    public List<float> lagTimeMultiplierPerFase;

    private State state;

    [Header("Distance evaluation")]
    public float shortDistance = 2;
    public float mediumDistance = 6;

    //Abilities
    public enum Ability { Slash, Dash, CrystalsUp, OrbitalStrike, SwordsCircle, DashSlam, HorizontalPierce, None };
    private Ability currentAbility;
    private Ability previousAbility;
    private IEnumerator currentAbilityRoutine;
    private List<bool> abilityAvailable;

    //Movement
    private Vector3 actionDest;

    //Abilities stats
    [Header("Abilities")]

    public _Slash slashStats;
    public _Dash dashStats;
    public _CrystalsUp crystalsUpStats;
    public _OrbitalStrike orbitalStrikeStats;
    public _SwordsCircle swordsCircleStats;
    public _DashSlam dashSlamStats;
    public _HorizontalPierce horizontalPierceStats;

    //Probabilities
    [Header("Probabilities Fase 0")]
    [Range(0.0f, 1.0f)]
    public List<float> fase0Medium;
    [Range(0.0f, 1.0f)]
    public List<float> fase0Long;

    [Header("Probabilities Fase 1")]
    [Range(0.0f, 1.0f)]
    public List<float> fase1Short;
    [Range(0.0f, 1.0f)]
    public List<float> fase1Medium;
    [Range(0.0f, 1.0f)]
    public List<float> fase1Long;

    [Header("Misc")]
    public RectArea walkableArea;

    [Header("Particles")]
    public ParticlePlayer dieParticles;
    public TrailRenderer dashTrail;
    public DashParticles dashParticles;

    private PixelPerfectCamera ppCamera;
    #endregion

    #region Abilities Structs

    [System.Serializable]
    public struct _Slash
    {
        public int damage;
        public float knockback;
        public float attackMoveDistance;
        public float prepDuration;
        public float timeActive;
        public List<int> slashesPerFase;
        [Range(0.0f, 1.0f)]
        public float extraSlashProb;
        public List<float> slashDuration;
        public float moveDuration;
        public List<float> timeBetweenSlashes;
        public float lagTime;
        public Attack slashObject;
    }

    [System.Serializable]
    public struct _Dash
    {
        public float duration;
        public float minDistance;
        public float probabilityMultiplier;
        public float cooldown;
        public float lagTime;
    }


    [System.Serializable]
    public struct _CrystalsUp
    {
        public int damage;
        public float nCrystals;
        public float knockback;
        public float crystalSpeed;
        public float crystalDuration;
        public float prepDuration;
        public float castDelay;
        public float crystalSpawnDuration;
        public float crystalCastDuration;
        public float lagTime;
        public float spawnMinRadius;
        public float spawnMaxRadius;
        public float crystalMinDistance;
        public CrystalDestroyer crystalObject;
        public GameObject circleObject;
        public bool drawGizmos;
    }

    [System.Serializable]
    public struct _OrbitalStrike
    {
        public int damage;
        public float knockback;
        public float prepDuration;
        public float stopDuration;
        public float orbitalDelay;
        public float orbitalDuration;
        public float damageInterval;
        public float orbitalRadius;
        public float circleRadius;
        public float lagTime;
        public OrbitalStrike orbitalObject;
        public GameObject circleObject;
    }

    [System.Serializable]
    public struct _SwordsCircle
    {
        public int damage;
        public float knockback;
        public float moveSpeed;
        public float nSwords;
        public float prepDuration;
        public float swordsSpawnDuration;
        public float swordSpeed;
        public float swordFrequency;
        public Vector2 swordSize;
        public float spawnRadius;
        public float minRadius;
        public float castDelay;
        public float alertDuration;
        public float chargeDuration;
        public float swordDuration;
        public float cooldown;
        public float lagTime;
        public SwordDestroyer swordObject;
    }

    [System.Serializable]
    public struct _DashSlam
    {
        public int damage;
        public float knockback;
        public float prepDuration;
        public float dashDuration;
        public float slamDelay;
        public float slamRadius;
        public float slamDuration;
        public float slamColliderDuration;
        public float lagTime;
        public DashSlam slamObject;
        public ParticleSystem slamParticles;
    }

    [System.Serializable]
    public struct _HorizontalPierce
    {
        public int pierceDamage;
        public float knockback;
        public float distanceToPlayer;
        public float pierceOfsset;
        public float pirceRange;
        public float prepDuration;
        public float recoverDuration;
        public int crystalDamage;
        public float crystalSpawnDelay;
        public float nCrystals;
        public Vector2 crystalRange;
        public float crystalReleaseDuration;
        public float crystalRecallSpeed;
        public float crystalRecallAcceleration;
        public float crystalRecallDelay;
        public float lagTime;
        public CrystalDestroyer crystalObject;
        public PierceDestroyer pierceObject;
    }

    #endregion

    #region Basic functions

    protected override void Init()
    {
        base.Init();

        //Init cooldowns
        abilityAvailable = new List<bool>();
        int nAbilities = System.Enum.GetNames(typeof(Ability)).Length - 1;
        for (int i = 0; i < nAbilities; i++)
        {
            abilityAvailable.Add(true);
        }
        ppCamera = FindObjectOfType<PixelPerfectCamera>();
        StartCoroutine(IPresentation());
    }

    protected override void UpdateEnemy()
    {
        UpdateDashTrail();

        if (Input.GetKeyDown("j"))
        {
            Slash();
        }
        if (Input.GetKeyDown("k"))
        {
            Dash(true);
        }
        if (Input.GetKeyDown("l"))
        {
            Dash(false);
        }
        if (Input.GetKeyDown("o"))
        {
            CrystalsUp();
        }
        if (Input.GetKeyDown("m"))
        {
            OrbitalStrike();
        }
        if (Input.GetKeyDown("n"))
        {
            SwordsCircle();
        }
        if (Input.GetKeyDown("h"))
        {
            DashSlam();
        }
        if (Input.GetKeyDown("g"))
        {
            HorizontalPierce();
        }
    }

    public void EvaluateSituation()
    {
        if (fase == 0)
        {
            EvaluateFase0();
        }
        else if (fase == 1)
        {
            EvaluateFase1();
        }
        else if (fase == 2)
        {
            EvaluateFase2();
        }
    }

    public void EvaluateFase0()
    {
        int distancia = CheckDistanceToPlayer();
        if (distancia == 0)
        {
            CastAbility(Ability.Slash);
        }
        else if (distancia == 1)
        {
            float rand = Random.value;
            if (rand < fase0Medium[0])
            {
                CastAbility(Ability.Slash);
            }
            else if (rand < fase0Medium[1])
            {
                CastAbility(Ability.HorizontalPierce);
            }
            else if (rand < fase0Medium[2])
            {
                CastAbility(Ability.Dash, false);
            }
            else
            {
                CastAbility(Ability.SwordsCircle);
            }
        }
        else if (distancia == 2)
        {
            float rand = Random.value;
            if (rand < fase0Long[0])
            {
                CastAbility(Ability.CrystalsUp);
            }
            else if (rand < fase0Long[1])
            {
                CastAbility(Ability.Dash);
            }
            else
            {
                CastAbility(Ability.HorizontalPierce);
            }
        }
    }

    public void EvaluateFase1()
    {
        int distancia = CheckDistanceToPlayer();
        if (distancia == 0)
        {
            float rand = Random.value;
            if (rand < fase1Short[0])
            {
                CastAbility(Ability.Slash);
            }
            else
            {
                CastAbility(Ability.DashSlam);
            }
        }
        else if (distancia == 1)
        {
            float rand = Random.value;
            if (rand < fase1Medium[0])
            {
                CastAbility(Ability.Slash);
            }
            else if (rand < fase1Medium[1])
            {
                CastAbility(Ability.HorizontalPierce);
            }
            else if (rand < fase1Medium[2])
            {
                CastAbility(Ability.DashSlam);
            }
            else if (rand < fase1Medium[3])
            {
                CastAbility(Ability.Dash, false);
            }
            else
            {
                CastAbility(Ability.SwordsCircle);
            }
        }
        else if (distancia == 2)
        {
            float rand = Random.value;
            if (rand < fase1Long[0])
            {
                CastAbility(Ability.CrystalsUp);
            }
            else if (rand < fase1Long[1])
            {
                CastAbility(Ability.OrbitalStrike);
            }
            else if (rand < fase1Long[2])
            {
                CastAbility(Ability.Dash);
            }
            else
            {
                CastAbility(Ability.HorizontalPierce);
            }
        }
    }

    public void EvaluateFase2()
    {
        CastAbility(Ability.Slash);
    }

    // Retorna un enter que indica la distancia del boss al jugador.
    // 0-> Curta, 1 -> Mitja, 2 -> Llarga
    public int CheckDistanceToPlayer()
    {
        float distance = DistanceToPlayer();
        if (distance <= shortDistance)
        {
            return 0;
        }
        else if (distance <= mediumDistance)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    public float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    private Vector3 FindRandomMovePoint(float minimumDistanceToPlayer, float maximumDistanceToPlayer, float minDistance = 0)
    {
        int n = 0;
        Vector3 destPoint = walkableArea.RandomPoint();
        while ((Vector3.Distance(destPoint, player.transform.position) < minimumDistanceToPlayer || Vector3.Distance(destPoint, player.transform.position) > maximumDistanceToPlayer || Vector3.Distance(destPoint, realPos) < minDistance) && n < 100)
        {
            destPoint = walkableArea.RandomPoint();
        }

        return destPoint;
    }


    public bool CheckFaseChange()
    {
        if (CheckDamageFase() != fase)
        {
            StartFase(CheckDamageFase());
            return true;
        }
        return false;
    }

    private IEnumerator IPresentation()
    {
        yield return new WaitForSeconds(1f);
        CastAbility(Ability.Dash);
        StartFase(0);
    }

    public override void Hit(Attack attack)
    {
        base.Hit(attack);
        CheckFaseChange();
    }

    public override void Die()
    {
        base.Die();
        StopAllCoroutines();
        StartCoroutine(IQuickTimeEvent());
    }

    private IEnumerator IQuickTimeEvent()
    {
        float duration = 4, delay = 1 ;
        gm.SlowDownGame(0.2f, duration);
        gm.BlockInputs(true);

        
        bool attacked = false;
        float t = 0;
        while(t < duration && !attacked)
        {
            t += Time.unscaledDeltaTime;
            if (t > delay)
            {
                if(!ButtonPopUp.instance.IsShowing())
                    ButtonPopUp.instance.Show("Attack");
                if (Input.GetButtonDown("Attack"))
                {
                    attacked = true;
                    ButtonPopUp.instance.Hide();
                    StartEndAnim();
                }
            }
            yield return null;
        }
        if (!attacked)
        {
            health = (int)(maxHealth * ratiosDamageFase[2])/2;
            Dash(false);
            fase = 2;
            gm.BlockInputs(false);
            ButtonPopUp.instance.Hide();
        }
    }

    private void StartEndAnim()
    {
        state = State.Dead;
        ChangeLayerIgnore();
        Globals.gameState = GameState.End;
        System.TimeSpan span = System.DateTime.Now.Subtract(Globals.startTimeStamp);
        Globals.totalTime = (float)span.TotalMilliseconds;
        SaveSystem.SaveGame();
        gm.SlowDownGame(1f,0);
        StartCoroutine(IEndAnim());
    }

    private IEnumerator IEndAnim()
    {
        player.DashTo(realPos + new Vector3(-0.5f,-2.5f));
        CameraManager.instance.mainCamera.SetDestination(realPos);
        yield return new WaitForSeconds(player.dashTime + 0.5f);
        player.gameObject.SetActive(false);
        animator.SetTrigger("Die");
        ppCamera.refResolutionX = 480;
        ppCamera.refResolutionY = 270;
        yield return new WaitForSeconds(0.1f);
        ppCamera.refResolutionX = 400;
        ppCamera.refResolutionY = 125;
        yield return new WaitForSeconds(0.1f);
        ppCamera.refResolutionX = 320;
        ppCamera.refResolutionY = 180;
        gm.SlowDownGameLerp(0.1f, 0.5f, 5f);

        yield return new WaitForSecondsRealtime(2);
        gm.RedToBlackFilter(2, 2);

    }

    private IEnumerator IDieAnim()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(IDie());
        Instantiate(dieParticles, transform.position, Quaternion.Euler(-90, 0, 0));
    }

    private void UpdateSpriteFlip(Vector3 target)
    {
        spriteRenderer.flipX = (target - realPos).x > 0;
    }

    private int GetDirection(Vector3 dest)
    {
        return MathFunctions.GetDirection(dest - realPos);
    }

    #endregion

    #region Abilities functions

    #region Functions callers

    public void CastAbility(Ability ability, bool dashIn = true)
    {
        if (abilityAvailable[(int)ability])
        {
            if (ability == Ability.Slash)
            {
                Slash();
            }
            else if (ability == Ability.Dash)
            {
                Dash(dashIn);
            }
            else if (ability == Ability.CrystalsUp)
            {
                CrystalsUp();
            }
            else if (ability == Ability.OrbitalStrike)
            {
                OrbitalStrike();
            }
            else if (ability == Ability.SwordsCircle)
            {
                SwordsCircle();
            }
            else if (ability == Ability.DashSlam)
            {
                DashSlam();
            }
            else if (ability == Ability.HorizontalPierce)
            {
                HorizontalPierce();
            }
        }
        else
        {
            EvaluateSituation();
        }
    }

    public void Slash()
    {
        currentAbility = Ability.Slash;
        StopAbility();
        currentAbilityRoutine = ISlash();
        StartCoroutine(currentAbilityRoutine);
    }

    public void Dash(bool dashIn)
    {
        currentAbility = Ability.Dash;
        StopAbility();
        currentAbilityRoutine = IDash(dashIn);
        StartCoroutine(currentAbilityRoutine);
    }

    public void CrystalsUp()
    {
        currentAbility = Ability.CrystalsUp;
        StopAbility();
        currentAbilityRoutine = ICrystalsUp();
        StartCoroutine(currentAbilityRoutine);
    }

    public void OrbitalStrike()
    {
        currentAbility = Ability.OrbitalStrike;
        StopAbility();
        currentAbilityRoutine = IOrbitalStrike();
        StartCoroutine(currentAbilityRoutine);
    }

    public void SwordsCircle()
    {
        currentAbility = Ability.SwordsCircle;
        StopAbility();
        currentAbilityRoutine = ISwordsCircle();
        StartCoroutine(currentAbilityRoutine);
    }

    public void DashSlam()
    {
        currentAbility = Ability.DashSlam;
        StopAbility();
        currentAbilityRoutine = IDashSlam();
        StartCoroutine(currentAbilityRoutine);
    }

    public void HorizontalPierce()
    {
        currentAbility = Ability.HorizontalPierce;
        StopAbility();
        currentAbilityRoutine = IHorizontalPierce();
        StartCoroutine(currentAbilityRoutine);
    }

    public void StopAbility()
    {
        if (currentAbilityRoutine != null)
        {
            StopCoroutine(currentAbilityRoutine);
        }
    }

    #endregion

    #region Routine functions

    private IEnumerator ISlash()
    {
        int iSlash = 0;

        //while (iSlash < slashStats.slashesPerFase[fase] || (iSlash == slashStats.slashesPerFase[fase] && CheckDistanceToPlayer() == 0 && fase <= 1))

        while(iSlash < slashStats.slashesPerFase[fase] || (iSlash == slashStats.slashesPerFase[fase] && CheckDistanceToPlayer() == 0))
        {
            yield return new WaitForSeconds(slashStats.timeBetweenSlashes[fase]);
            if(iSlash%2 == 0)
                animator.SetTrigger("Slash");
            else 
                animator.SetTrigger("Slash2");
            animator.SetInteger("Direction", GetDirection(player.transform.position));
            UpdateSpriteFlip(player.transform.position);
            float t = 0;

            Vector2 vectorToPlayer = player.transform.position - realPos;
            int dir = MathFunctions.GetDirection(vectorToPlayer);
            slashStats.slashObject.transform.eulerAngles = new Vector3(0, 0, dir * 90);
            slashStats.slashObject.damage = slashStats.damage;
            slashStats.slashObject.knockback = slashStats.knockback;

            Vector2 startPoint = realPos;
            Vector2 destPoint = Vector2.ClampMagnitude(vectorToPlayer * 10000, slashStats.attackMoveDistance) + startPoint;

            while (t < slashStats.moveDuration || t < slashStats.timeActive)
            {
                t += Time.deltaTime;
                if (t <= slashStats.moveDuration)
                {
                    realPos = MathFunctions.EaseOutExp(t, startPoint, destPoint, slashStats.moveDuration, 5);
                    PixelPerfectMovement.Move(realPos, rb);
                }
              
                yield return null;
            }

            iSlash += 1;
            if (fase == 2 && Random.value < slashStats.extraSlashProb)
            {
                iSlash = -1;
            }
        }
        animator.SetTrigger("EndSlash");

        EndAbility(Ability.Slash, slashStats.lagTime);
    }

    private IEnumerator IDash(bool dashIn)
    {
        Vector3 startPoint = realPos;
        Vector3 destPoint;

        StartCoroutine(ICooldownDash());

        if (dashIn)
        {
            destPoint = Random.Range(0, 2) == 0 ? FindRandomMovePoint(0, shortDistance, dashStats.minDistance) : FindRandomMovePoint(shortDistance, mediumDistance, dashStats.minDistance);
        }
        else
        {
            destPoint = FindRandomMovePoint(mediumDistance, 10000, dashStats.minDistance);
        }

        state = State.Dash;
        ChangeLayerIgnore();
        SpawnDashParticles(startPoint, destPoint);

        animator.SetTrigger("Dash");
        animator.SetInteger("Direction", GetDirection(destPoint));
        UpdateSpriteFlip(destPoint);

        float t = 0;
        while (t < dashStats.duration)
        {
            t += Time.deltaTime;
            realPos = MathFunctions.EaseOutExp(t, startPoint, destPoint, dashStats.duration, 5);
            PixelPerfectMovement.Move(realPos, rb);
            yield return null;
        }
        state = State.Idle;
        ResetLayer();
        UpdateSpriteFlip(player.transform.position);
        EndAbility(Ability.Dash, dashStats.lagTime);
    }


    private IEnumerator ICrystalsUp()
    {
        animator.SetTrigger("PalmDown");
        UpdateSpriteFlip(player.transform.position);

        GameObject circlesObject = Instantiate(crystalsUpStats.circleObject, transform);
        circlesObject.transform.localScale = Vector3.one * crystalsUpStats.spawnMaxRadius;

        yield return new WaitForSeconds(crystalsUpStats.prepDuration);

        List<CrystalDestroyer> crystalList = new List<CrystalDestroyer>();

        while (crystalList.Count < crystalsUpStats.nCrystals)
        {
            CrystalDestroyer crystal = Instantiate(crystalsUpStats.crystalObject);
            crystal.damage = crystalsUpStats.damage;
            crystal.knockback = crystalsUpStats.knockback;
            Vector3 spawnPoint = Random.insideUnitCircle.normalized * Random.Range(crystalsUpStats.spawnMinRadius, crystalsUpStats.spawnMaxRadius);
            while (IsNearOthersCrystals(spawnPoint + transform.position, crystalList, crystalsUpStats.crystalMinDistance / crystalsUpStats.nCrystals))
            {
                spawnPoint = Random.insideUnitCircle.normalized * Random.Range(crystalsUpStats.spawnMinRadius, crystalsUpStats.spawnMaxRadius);
            }
            crystal.transform.position = transform.position + spawnPoint;
            crystalList.Add(crystal);
            crystal.Float();
            yield return new WaitForSeconds(crystalsUpStats.crystalSpawnDuration / crystalsUpStats.nCrystals);
        }

        animator.SetTrigger("EndPalmDown");

        yield return new WaitForSeconds(crystalsUpStats.castDelay);

        foreach (CrystalDestroyer crystal in crystalList)
        {
            crystal.AttackPlayer(crystalsUpStats.crystalSpeed, crystalsUpStats.crystalDuration);
            yield return new WaitForSeconds(crystalsUpStats.crystalCastDuration / crystalsUpStats.nCrystals);
        }

        EndAbility(Ability.CrystalsUp, crystalsUpStats.lagTime);
    }

    private IEnumerator IOrbitalStrike()
    {
        //GameObject circlesObject = Instantiate(orbitalStrikeStats.circleObject, transform);
        //circlesObject.transform.localScale = Vector3.one * orbitalStrikeStats.circleRadius;

        animator.SetTrigger("PalmDown");
        UpdateSpriteFlip(player.transform.position);

        yield return new WaitForSeconds(orbitalStrikeStats.prepDuration);

        OrbitalStrike strike = Instantiate(orbitalStrikeStats.orbitalObject, player.transform.position, Quaternion.identity);
        strike.damage = orbitalStrikeStats.damage;
        strike.transform.localScale = Vector3.one * orbitalStrikeStats.orbitalRadius;
        strike.StartStrike(orbitalStrikeStats.orbitalDuration, orbitalStrikeStats.orbitalDelay, orbitalStrikeStats.damageInterval);

        animator.SetTrigger("EndPalmDown");

        yield return new WaitForSeconds(orbitalStrikeStats.stopDuration);

        EndAbility(Ability.OrbitalStrike, orbitalStrikeStats.lagTime);

    }

    private IEnumerator ISwordsCircle()
    {
        StartCoroutine(ICooldownSwordsCircle());
        StartCoroutine(ISwordsCircleMove());

        animator.SetTrigger("SwordsCircle");
        UpdateSpriteFlip(player.transform.position);

        yield return new WaitForSeconds(swordsCircleStats.prepDuration);
        List<SwordDestroyer> swords = new List<SwordDestroyer>();
        int motionDir = Random.Range(0, 2) == 0 ? 1 : -1;
        while (swords.Count < swordsCircleStats.nSwords)
        {
            SwordDestroyer sword = Instantiate(swordsCircleStats.swordObject);
            sword.damage = swordsCircleStats.damage;
            sword.knockback = swordsCircleStats.knockback;
            sword.transform.localScale = swordsCircleStats.swordSize;
            sword.StartRotating(MathFunctions.HzToRadSeg(swordsCircleStats.swordFrequency), swordsCircleStats.spawnRadius, motionDir, this, swordsCircleStats.chargeDuration, swordsCircleStats.minRadius);
            swords.Add(sword);
            yield return new WaitForSeconds((1 / swordsCircleStats.swordFrequency) / swordsCircleStats.nSwords);
        }

        foreach (SwordDestroyer sword in swords)
        {
            sword.Charge(swords);
        }

        yield return new WaitForSeconds(swordsCircleStats.chargeDuration + swordsCircleStats.alertDuration + swordsCircleStats.castDelay);

        animator.SetTrigger("EndSwordsCircle");

        EndAbility(Ability.SwordsCircle, swordsCircleStats.lagTime);

    }

    private IEnumerator ISwordsCircleMove()
    {
        float t = 0;
        Vector3 destPoint = FindRandomMovePoint(0, mediumDistance, 10);
        float floatAmplitude = 0.01f, floatAngularSpeed = 1;
        while (t < swordsCircleStats.prepDuration + (1 / swordsCircleStats.swordFrequency) + swordsCircleStats.chargeDuration)
        {
            t += Time.deltaTime;
            if (Vector3.Distance(realPos, destPoint) > 0.2f)
            {
                UpdateSpriteFlip(player.transform.position);
                realPos += swordsCircleStats.moveSpeed * (destPoint - transform.position).normalized * Time.deltaTime + new Vector3(0, floatAmplitude * Mathf.Sin(floatAngularSpeed * t * Mathf.PI));
                PixelPerfectMovement.Move(realPos, rb);
            }
            yield return null;
        }
    }


    private IEnumerator IDashSlam()
    {

        yield return new WaitForSeconds(dashSlamStats.prepDuration);
        Vector3 startPoint = realPos;
        Vector3 destPoint = player.transform.position + (Vector3)player.lastDir.normalized * player.movementValue.magnitude * player.speed * dashSlamStats.dashDuration;

        state = State.Dash;
        ChangeLayerIgnore();
        SpawnDashParticles(startPoint, destPoint);

        animator.SetTrigger("DashSlam");
        UpdateSpriteFlip(player.transform.position);

        float t = 0;
        while (t < dashSlamStats.dashDuration * 0.3f)
        {
            t += Time.deltaTime;
            //realPos = MathFunctions.EaseInExp(t, startPoint, destPoint, dashSlamStats.dashDuration, 3);
            realPos = MathFunctions.EaseOutExp(t, startPoint, destPoint, dashSlamStats.dashDuration, 3);
            //realPos = Vector3.Lerp(startPoint, destPoint, t / dashSlamStats.dashDuration);
            PixelPerfectMovement.Move(realPos, rb);
            yield return null;
        }

        ResetLayer();


        DashSlam slam = dashSlamStats.slamObject;
        slam.transform.localScale = dashSlamStats.slamRadius * new Vector3(1,0.5f);
        slam.damage = dashSlamStats.damage;
        slam.knockback = dashSlamStats.knockback;

        startPoint = realPos;
        t = 0;
        yield return new WaitForSeconds(dashSlamStats.slamDelay);

        UpdateSpriteFlip(player.transform.position);
        animator.SetTrigger("DashSlamDown");
        animator.SetTrigger("EndDashSlam");
        while (t < dashSlamStats.dashDuration * 0.7f)
        {
            t += Time.deltaTime;
            realPos = MathFunctions.EaseInExp(t, startPoint, destPoint, dashSlamStats.slamDuration, 3);
            //realPos = MathFunctions.EaseOutExp(t, startPoint, destPoint, dashSlamStats.dashDuration, 3);
            //realPos = Vector3.Lerp(startPoint, destPoint, t / dashSlamStats.dashDuration);
            PixelPerfectMovement.Move(realPos, rb);
            yield return null;
        }
        state = State.Idle;
        slam.gameObject.SetActive(true);
        ParticleSystem slamParticles = Instantiate(dashSlamStats.slamParticles, transform.position + Vector3.up * -1, Quaternion.identity);
        ParticleSystem.ShapeModule shape = slamParticles.shape;
        shape.radius = dashSlamStats.slamRadius;
        slamParticles.transform.GetChild(0).localScale = dashSlamStats.slamRadius * new Vector3(1, 0.5f);
        Destroy(slamParticles, 2f);
        yield return new WaitForSeconds(dashSlamStats.slamColliderDuration);
        slam.gameObject.SetActive(false);



        EndAbility(Ability.DashSlam, dashStats.lagTime);
    }

    private IEnumerator IHorizontalPierce()
    {
        Vector3 startPoint = realPos;
        int dir = Random.Range(0, 2) == 0 ? 1 : -1;
        Vector3 destPoint = player.transform.position + horizontalPierceStats.distanceToPlayer * Vector3.right * -dir;
        if (!walkableArea.isInside(destPoint))
        {
            dir *= -1;
            destPoint = player.transform.position + horizontalPierceStats.distanceToPlayer * Vector3.right * -dir;
            Debug.Log(walkableArea.isInside(destPoint) + " " + destPoint);
        }

        state = State.Dash;
        ChangeLayerIgnore();
        SpawnDashParticles(startPoint, destPoint);

        animator.SetTrigger("Dash");
        animator.SetInteger("Direction", GetDirection(destPoint));
        UpdateSpriteFlip(destPoint);

        float t = 0;
        while (t < dashStats.duration)
        {
            t += Time.deltaTime;
            realPos = MathFunctions.EaseOutExp(t, startPoint, destPoint, dashStats.duration, 5);
            PixelPerfectMovement.Move(realPos, rb);
            yield return null;
        }

        ResetLayer();
        state = State.Idle;

        animator.SetTrigger("HorizontalPierce");
        spriteRenderer.flipX = dir == -1;

        yield return new WaitForSeconds(horizontalPierceStats.prepDuration);

        horizontalPierceStats.pierceObject.gameObject.SetActive(true);
        horizontalPierceStats.pierceObject.ReleaseCrystals(dir, horizontalPierceStats);
        horizontalPierceStats.pierceObject.transform.eulerAngles = new Vector3(0, 0, dir == 1 ? 0 : 180);
        yield return new WaitForSeconds(horizontalPierceStats.recoverDuration);
        horizontalPierceStats.pierceObject.gameObject.SetActive(false);

        EndAbility(Ability.HorizontalPierce, horizontalPierceStats.lagTime);
    }
    #endregion

    #region End functions

    public void EndAbility(Ability ability, float lagTime)
    {
        previousAbility = currentAbility;
        currentAbility = Ability.None;

        if (currentAbilityRoutine != null)
        {
            StopCoroutine(currentAbilityRoutine);
        }
        currentAbilityRoutine = ILagTime(ability, lagTime);
        StartCoroutine(currentAbilityRoutine);
    }

    public IEnumerator ILagTime(Ability ability, float lagTime)
    {
        yield return new WaitForSeconds(lagTime * lagTimeMultiplierPerFase[fase]);

        CheckAbilityEnd(ability);
    }

    public void CheckAbilityEnd(Ability ability)
    {
        EvaluateSituation();
    }

    #endregion

    #region Extra functions

    private bool IsNearOthersCrystals(Vector3 candidate, List<CrystalDestroyer> crystals, float minDistance)
    {
        foreach (CrystalDestroyer crystal in crystals)
        {
            if (Vector3.Distance(crystal.transform.position, candidate) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    #endregion

    #region CoolDowns

    private IEnumerator ICooldownDash()
    {
        abilityAvailable[(int)Ability.Dash] = false;
        yield return new WaitForSeconds(dashStats.cooldown);
        abilityAvailable[(int)Ability.Dash] = true;
    }
    private IEnumerator ICooldownSwordsCircle()
    {
        abilityAvailable[(int)Ability.SwordsCircle] = false;
        yield return new WaitForSeconds(swordsCircleStats.cooldown);
        abilityAvailable[(int)Ability.SwordsCircle] = true;
    }

    #endregion

    #region Other

    public State GetState()
    {
        return state;
    }

    private void UpdateDashTrail()
    {
        dashTrail.emitting = state == State.Dash;
    }

    private void SpawnDashParticles(Vector3 startPos, Vector3 endPos)
    {
        float speedMod = (endPos - startPos).magnitude;
        DashParticles particlesInstance = Instantiate(dashParticles);
        particlesInstance.transform.position = startPos;
        particlesInstance.PlayParticles((endPos - startPos).normalized, speedMod * 10);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shortDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, mediumDistance);

        if (crystalsUpStats.drawGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, crystalsUpStats.spawnMinRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, crystalsUpStats.spawnMaxRadius);
        }

    }
}
