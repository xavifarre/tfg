using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perserver : Boss
{
    #region Variables

    //State
    public enum PerserverState { Idle, Move, Ability, Damaged, Die, Start };
    public PerserverState state;

    [Header("Distance evaluation")]
    public float shortDistance = 2;
    public float mediumDistance = 6;

    //Ability
    public enum Ability { Spin, SpinHeal, ExpandingSpin, UndodgeableSpin, PowderDrop, BarrelPop, BarrelToss, BarrelDrop, DoubleSlash, Heal, None };
    public Ability currentAbility;
    private Ability previousAbility;

    //Current ability routine
    public IEnumerator currentAbilityRoutine;
    //Heal controller rotuine
    private IEnumerator healRoutine;
    private bool healing = false;

    //Player hit control for triggering some abilities
    [HideInInspector]
    public int hitControl;

    //Movement
    private Vector3 actionDest;

    [Header("Blades")]
    //Blades
    public float basicRadius;
    public float basicAngularSpeed;
    private List<Blade> blades;
    [HideInInspector]
    public Vector3 barrelPopPosition;
    [HideInInspector]
    public bool barrelPopped;

    //BarrelContainer
    private GameObject barrelContainer;

    //Abilities stats
    [Header("Abilities")]

    //Spin
    public _Spin spinStats;

    //Spin Heal
    public _SpinHeal spinHealStats;

    //Expanding Spin
    public _ExpandingSpin expandingSpinStats;

    //Undodgeable Spin
    public _UndodgeableSpin undodgeableSpinStats;

    //Powder Drop
    public _PowderDrop powderDropStats;

    //Barrel Pop
    public _BarrelPop barrelPopStats;

    //Barrel Toss
    public _BarrelToss barrelTossStats;

    //Barrel Drop
    public _BarrelDrop barrelDropStats;

    //Double Slash
    public _DoubleSlash doubleSlashStats;

    //Heal
    public _Heal healStats;

    public RectArea walkableArea;

    #endregion


    #region Abilities Structs

    [System.Serializable]
    public struct _Spin
    {
        public int damage;
        public float angularSpeed;
        public float radius;
        public float duration;
        public float growDuration;
        public float evaluationRate;
        public int iterationsForExpanding;
        public float lagTime;
        public float minimumDistanceToPlayer;
    }


    [System.Serializable]
    public struct _ExpandingSpin
    {
        public int damage;
        public float angularSpeed;
        public float maxAngularSpeed;
        public float maxRadius;
        public float recallDuration;
        public float duration;
        public float lagTime;
    }

    [System.Serializable]
    public struct _UndodgeableSpin
    {
        public int damage;
        public float angularMaxSpeed;
        public float duration;
        public float recoverDuration;
        public float lagTime;
    }

    [System.Serializable]
    public struct _PowderDrop
    {
        public float radius;
        public float spinSpeed;
        public float spinDuration;
        public float powderDelay;
        public Powder powderObject;
        public float lagTime;
    }

    [System.Serializable]
    public struct _BarrelPop
    {
        public float popSpeed;
        public float recallSpeed;
        public float distanceToPlayer;
        public float preparationRadius;
        public float preparationSpeed;
        public float placementDuration;
        public float popDuration;
        public float recallDuration;
        public BarrelPopable barrelObject;
        public float lagTime;
    }

    [System.Serializable]
    public struct _BarrelToss
    {
        public float spinSpeed;
        public float spinDuration;
        public float barrelSpeed;
        public BarrelThrowable barrelObject;
        public float lagTime;
    }

    [System.Serializable]
    public struct _BarrelDrop
    {
        public List<int> nBarrels;
        public float castInterval;
        public float preparationDuration;
        public float preparationRadius;
        public float launchSpeed;
        public float gravityScale;
        public float minimumDistanceToBoss;
        public float minimumDistanceToOther;
        public int throwArcIterations;
        public BarrelProximity barrelObject;
        public float lagTime;
    }

    [System.Serializable]
    public struct _DoubleSlash
    {
        public int damage;
        public float expandDistance;
        public float expandDelay;
        public float speed;
        public float chargeDuration;
        public float recoverDuration;
        public float recoverDelay;
        public float lagTime;
    }

    [System.Serializable]
    public struct _Heal
    {
        public int minHealAmount;
        public int maxHealAmount;
        public float healRate;
        public float evaluationRate;
        public float firstEvaluationMultiplier;
        public float lagTime;
    }

    [System.Serializable]
    public struct _SpinHeal
    {
        public int damage;
        public float angularSpeed;
        public float maxAngularSpeed;
        public float maxRadius;
        public float duration;
        public float recallDuration;
        public float lagTime;
    }

    #endregion

    protected override void Init()
    {
        blades = new List<Blade>();
        foreach(Blade blade in transform.parent.GetComponentsInChildren<Blade>())
        {
            blades.Add(blade);
            blade.perserver = this;
            blade.InitPosition();
        }

        barrelContainer = GameObject.Find("BarrelContainer");

        EvaluateSituation();
    }

    protected override void UpdateEnemy()
    {
        if(Input.GetKeyDown("l"))
        {
            
        }
        if (Input.GetKeyDown("j"))
        {
            Spin();
        }
        if (Input.GetKeyDown("h"))
        {
            ExpandingSpin();
        }
        if (Input.GetKeyDown("g"))
        {
            DoubleSlash();
        }
        if (Input.GetKeyDown("y"))
        {
            UndodgeableSpin();
        }
        if (Input.GetKeyDown("t"))
        {
            PowderDrop();
        }
        if (Input.GetKeyDown("o"))
        {
            BarrelDrop();
        }
        if (Input.GetKeyDown("m"))
        {
            BarrelToss();
        }
        if (Input.GetKeyDown("n"))
        {
            BarrelPop();
        }
        if (Input.GetKeyDown("b"))
        {
            SpinHeal();
        }
    }


    public void EvaluateSituation()
    {
        if(fase == 0)
        {
            EvaluateFase0();
        }
        else if(fase == 1)
        {
            EvaluateFase1();
        }
        else if (fase == 2)
        {
            EvaluateFase2();
        }
        else if (fase == 3)
        {
            EvaluateFase3();
        }
    }

    public void EvaluateFase0()
    {
        int distancia = CheckDistanceToPlayer();
        if(distancia == 0)
        {
            Spin();
        }
        else if (distancia == 1)
        {
            DoubleSlash();
        }
        else if (distancia == 2)
        {
            if(previousAbility == Ability.BarrelDrop)
            {
                DoubleSlash(true);
            }
            else
            {
                BarrelDrop();
            }
        }
    }

    public void EvaluateFase1()
    {

    }

    public void EvaluateFase2()
    {

    }

    public void EvaluateFase3()
    {

    }

    public void StartFase()
    {
        PowderDrop();
    }

    // Retorna un enter que indica la distancia del boss al jugador.
    // 0-> Curta, 1 -> Mitja, 2 -> Llarga
    public int CheckDistanceToPlayer()
    {
        float distance = DistanceToPlayer();
        if(distance <= shortDistance)
        {
            return 0;
        }
        else if(distance <= mediumDistance)
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

    private void MoveToPlayer(float speedMultiplier = 1)
    {
        Vector3 direction = (player.transform.position - realPos).normalized;
        float movSpeed = speed + speed * speedMultiplier;
        realPos = realPos + movSpeed * direction * Time.deltaTime;
        PixelPerfectMovement.Move(realPos, transform);
    }

    private void MoveToDest(float speedMultiplier = 1)
    {
        Vector3 direction = (actionDest - realPos).normalized;
        float movSpeed = speed + speed * speedMultiplier;
        realPos = realPos + movSpeed * direction * Time.deltaTime;
        PixelPerfectMovement.Move(realPos, transform);
    }

    private Vector3 FindRandomMovePoint(float minimumDistanceToPlayer = 0)
    {
        int n = 0;
        Vector3 destPoint = walkableArea.RandomPoint();
        while (Vector3.Distance(destPoint,player.transform.position) < minimumDistanceToPlayer && n < 20)
        {
            destPoint = walkableArea.RandomPoint();
        }

        return destPoint;
    }

    /*********************************************
    *                  ABILITIES                 *
    *********************************************/

    #region Abilities functions

    #region Basic functions

    public void Spin()
    {
        currentAbility = Ability.Spin;
        currentAbilityRoutine = ISpin();
        StartCoroutine(currentAbilityRoutine);
        CastAbilityBlade(Blade.BladeAbility.Spin);
    }

    public void ExpandingSpin()
    {
        currentAbility = Ability.ExpandingSpin;
        CastAbilityBlade(Blade.BladeAbility.ExpandingSpin, false);
    }

    public void UndodgeableSpin()
    {
        currentAbility = Ability.UndodgeableSpin;
        CastAbilityBlade(Blade.BladeAbility.UndodgeableSpin, false);
    }

    public void PowderDrop()
    {
        currentAbility = Ability.PowderDrop;
        CastAbilityBlade(Blade.BladeAbility.PowderDrop);
    }

    public void BarrelPop()
    {
        currentAbility = Ability.BarrelPop;
        CastAbilityBlade(Blade.BladeAbility.BarrelPop);
    }

    public void BarrelToss()
    {
        currentAbility = Ability.BarrelToss;
        CastAbilityBlade(Blade.BladeAbility.BarrelToss);
    }

    public void BarrelDrop()
    {
        currentAbility = Ability.BarrelDrop;
        CastAbilityBlade(Blade.BladeAbility.BarrelDrop);
    }

    public void DoubleSlash(bool aproaching = false)
    {
        currentAbility = Ability.DoubleSlash;
        actionDest = FindRandomMovePoint();
        currentAbilityRoutine = IDoubleSlash(aproaching);
        StartCoroutine(currentAbilityRoutine);
        CastAbilityBlade(Blade.BladeAbility.DoubleSlash);
    }

    public void SpinHeal()
    {
        currentAbility = Ability.SpinHeal;
        CastAbilityBlade(Blade.BladeAbility.SpinHeal);
    }

    public void CastAbilityBlade(Blade.BladeAbility ability, bool randomMotion = true)
    {
        int motionDirection = Random.Range(0, 2) * 2 - 1;
        foreach (Blade blade in blades)
        {
            if (randomMotion)
            {
                blade.motionDirection = motionDirection;
            }
            blade.StartAbility(ability);      
        }
    }

    #endregion

    #region Ability Rutines

    private IEnumerator ISpin()
    {
        Debug.Log(currentAbility.ToString());
        while(currentAbility == Ability.Spin)
        {
            if(DistanceToPlayer() > spinStats.minimumDistanceToPlayer)
            {
                MoveToPlayer();
            }
            yield return null;
        }
    }

    private IEnumerator IDoubleSlash(bool aproaching)
    {
        float t = 0;
        while (currentAbility == Ability.DoubleSlash && t < doubleSlashStats.chargeDuration)
        {
            if (aproaching && DistanceToPlayer() > mediumDistance)
            {
                MoveToPlayer();
            }
            else
            {
                MoveToDest();
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    #endregion

    #region Extra functions
    public void SpawnPowder()
    {
        Powder powder = Instantiate(powderDropStats.powderObject, transform.position, Quaternion.identity);
        powder.radius = powderDropStats.radius;
        powder.delay = powderDropStats.powderDelay;
    }

    public void StartBarrelDrop()
    {
        currentAbilityRoutine = IBarrelDrop();
        StartCoroutine(currentAbilityRoutine);
    }

    public void LaunchBarrel()
    {
        BarrelProximity barrel = Instantiate(barrelDropStats.barrelObject, transform.position, Quaternion.identity);
        
        Vector3 destPos = walkableArea.RandomPoint();
        while (!BarrelMinDistanceValid(destPos))
        {
            destPos = walkableArea.RandomPoint();
        }
        barrel.transform.parent = barrelContainer.transform;

        barrel.LaunchBarrel(transform.position, destPos, barrelDropStats.launchSpeed, barrelDropStats.throwArcIterations, barrelDropStats.gravityScale);
    }

    private bool BarrelMinDistanceValid(Vector3 pos)
    {
        return MinDistanceBoss(pos) && MinDistanceOther(pos);
    }

    private bool MinDistanceBoss(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) >= barrelDropStats.minimumDistanceToBoss;
    }

    private bool MinDistanceOther(Vector3 pos)
    {
        foreach (Transform other in barrelContainer.transform)
        {
            if (Vector3.Distance(pos, other.GetComponent<BarrelProximity>().destPos) < barrelDropStats.minimumDistanceToOther)
            {
                return false;
            }
        }
        return true;
    }

    public void ThrowBarrel()
    {
        BarrelThrowable barrel = Instantiate(barrelTossStats.barrelObject, transform.position, Quaternion.identity);
        barrel.ThrowToPosition(barrelTossStats.barrelSpeed, player.transform.position);
    }

    public void PlaceBarrel(Vector3 pos)
    {
        BarrelPopable barrel = Instantiate(barrelPopStats.barrelObject, pos, Quaternion.identity);
        barrel.perserver = this;
        barrelPopPosition = pos;
        barrelPopped = false;
    }

    public IEnumerator IBarrelDrop()
    {
        for (int i = 0; i < barrelDropStats.nBarrels[fase]; i++)
        {
            LaunchBarrel();
            yield return new WaitForSeconds(barrelDropStats.castInterval);
        }

        EndAbility(Ability.BarrelDrop, barrelDropStats.lagTime);

        currentAbilityRoutine = null;
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

    public void EndSpin()
    {
        EvaluateSituation();
    }

    public void EndExpandingSpin()
    {
        if(CheckDistanceToPlayer() == 0)
        {
            UndodgeableSpin();
        }
        else
        {
            EvaluateSituation();
        }
    }

    public void EndUndodgeableSpin()
    {
        EvaluateSituation();
    }

    public void EndPowderDrop()
    {

    }

    public void EndBarrelPop()
    {

    }

    public void EndBarrelToss()
    {

    }

    public void EndBarrelDrop()
    {
        EvaluateSituation();
    }

    public void EndDoubleSlash()
    {
        if(hitControl > 0 && CheckDistanceToPlayer() == 0)
        {
            Spin();
        }
        else if(CheckDistanceToPlayer() == 1)
        {
            DoubleSlash();
        }
        else
        {
            EvaluateSituation();
        }
    }

    public void EndHeal()
    {

    }

    public void EndSpinHeal()
    {

    }

    public void CheckAbilityEnd(Ability ability)
    {
        if (ability == Ability.Spin)
        {
            EndSpin();
        }
        else if (ability == Ability.ExpandingSpin)
        {
            EndExpandingSpin();
        }
        else if (ability == Ability.UndodgeableSpin)
        {
            EndUndodgeableSpin();
        }
        else if (ability == Ability.PowderDrop)
        {
            EndPowderDrop();
        }
        else if (ability == Ability.BarrelPop)
        {
            EndBarrelPop();
        }
        else if (ability == Ability.BarrelToss)
        {
            EndBarrelToss();
        }
        else if (ability == Ability.BarrelDrop)
        {
            EndBarrelDrop();
        }
        else if (ability == Ability.DoubleSlash)
        {
            EndDoubleSlash();
        }
        else if (ability == Ability.Heal)
        {
            EndHeal();
        }
        else if (ability == Ability.SpinHeal)
        {
            EndSpinHeal();
        }
    }

    public IEnumerator ILagTime(Ability ability, float lagTime)
    {
        yield return new WaitForSeconds(lagTime);

        CheckAbilityEnd(ability);
    }

    #endregion

    #endregion

    /*********************************************
    *                  HEAL                     *
    *********************************************/
    #region Heal

    public void StartBasicHeal()
    {
        currentAbilityRoutine = IHealEvaluation();
        StartCoroutine(currentAbilityRoutine);

        StartHeal();
    }

    public void StartHeal()
    {
        healing = true;
        healRoutine = IHeal();
        StartCoroutine(healRoutine);
    }

    public void Heal()
    {
        int amount = Random.Range(healStats.minHealAmount, healStats.maxHealAmount);
        health += amount;
        ShowHeal(amount);
    }

    public void StopHeal()
    {
        healing = false;
        StopCoroutine(healRoutine);
    }

    public IEnumerator IHealEvaluation()
    {
        float firstEvalutationDelay = Random.Range(healStats.evaluationRate * healStats.firstEvaluationMultiplier * 100, healStats.evaluationRate * 100) / 100;

        yield return new WaitForSeconds(firstEvalutationDelay);

        while(CheckDistanceToPlayer() == 2)
        {
            yield return new WaitForSeconds(healStats.evaluationRate);
        }

        StopHeal();
    }

    public IEnumerator IHeal()
    {
        while (healing)
        {
            Heal();
            yield return new WaitForSeconds(healStats.healRate);
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shortDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, mediumDistance);
    }
}