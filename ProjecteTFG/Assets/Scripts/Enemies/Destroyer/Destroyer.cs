using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : Boss
{
    #region Variables

    public List<float> lagTimeMultiplierPerFase;

    [Header("Distance evaluation")]
    public float shortDistance = 2;
    public float mediumDistance = 6;


    //Abilities
    public enum Ability { Slash, DashIn, DashOut, CrystalsUp, OrbitalStrike, SwordsCircle, DashSlam, HorizontalPierce, None };
    private Ability currentAbility;
    private Ability previousAbility;
    private IEnumerator currentAbilityRoutine;
    private List<bool> abilityAvailable;

    //Movement
    private Vector3 actionDest;

    //Abilities stats
    [Header("Abilities")]

    public _Slash slashStats;
    public _DashIn dashInStats;
    public _DashOut dashOutStats;
    public _CrystalsUp crystalsUpStats;
    public _OrbitalStrike orbitalStrikeStats;
    public _SwordsCircle swordsCircleStats;
    public _DashSlam dashSlamStats;
    public _HorizontalPierce horizontalPierceStats;

    [Header("Misc")]
    public RectArea walkableArea;

    #endregion

    #region Abilities Structs

    [System.Serializable]
    public struct _Slash
    {
        public int damage;
        public float knockback;
        public float attackMoveDistance;
        public List<int> slashesPerFase;
        public List<float> slashDuration;
        public List<float> timeBetweenSlashes;
        public float lagTime;
        public GameObject slashObject;
    }

    [System.Serializable]
    public struct _DashIn
    {
        public float duration;
        public float shortDistance;
        public float mediumDistance;
        public float probabilityMultiplier;
        public float lagTime;
    }

    [System.Serializable]
    public struct _DashOut
    {
        public float duration;
        public float cooldown;
        public float lagTime;
        private bool available;
    }

    [System.Serializable]
    public struct _CrystalsUp
    {
        public float damage;
        public float nCrystals;
        public float knockback;
        public float crystalSpeed;
        public float crystalDuration;
        public float prepDuration;
        public float castDelay;
        public float crystalDelay;
        public float lagTime;
        public float spawnMinRadius;
        public float spawnMaxRadius;
        public GameObject crystalObject;
    }

    [System.Serializable]
    public struct _OrbitalStrike
    {
        public float damage;
        public float knockback;
        public float prepDuration;
        public float orbitalDelay;
        public float orbitalRadius;
        public float lagTime;
        public GameObject orbitalObject;
    }

    [System.Serializable]
    public struct _SwordsCircle
    {
        public float damage;
        public float knockback;
        public float nSwords;
        public float prepDuration;
        public float swordDelay;
        public float swordSpeed;
        public float swordRotationSpeed;
        public float spawnRadius;
        public float cooldown;
        public float lagTime;
        public GameObject swordObject;
    }

    [System.Serializable]
    public struct _DashSlam
    {
        public float damage;
        public float knockback;
        public float prepDuration;
        public float dashDuration;
        public float slamRadius;
        public float lagTime;
        public GameObject slamObject;
    }

    [System.Serializable]
    public struct _HorizontalPierce
    {
        public float pierceDamage;
        public float knockback;
        public float prepDuration;
        public float crystalDamage;
        public float crystalSpawnDelay;
        public float nCrystals;
        public Vector2 crystalRange;
        public float crystalRecallSpeed;
        public float crystalRecallDelay;
        public GameObject crystalObject;
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
           //Slashes
        }
        else if (distancia == 1)
        {
            //%
        }
        else if (distancia == 2)
        {
            //%
        }
    }

    public void EvaluateFase1()
    {
        int distancia = CheckDistanceToPlayer();
        if (distancia == 0)
        {
            //%
        }
        else if (distancia == 1)
        {
            //%
        }
        else if (distancia == 2)
        {
            //%
        }
    }

    public void EvaluateFase2()
    {

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
        StartFase(0);
    }

    public override void Die()
    {
        base.Die();
        Globals.gameState = GameState.End;
    }

    #endregion


    #region Abilities functions

    #region Functions callers

    public void Slash()
    {
        currentAbility = Ability.Slash;
        currentAbilityRoutine = ISlash();
        StartCoroutine(currentAbilityRoutine);
    }

    public void DashIn()
    {
        currentAbility = Ability.DashIn;
        currentAbilityRoutine = IDashIn();
        StartCoroutine(currentAbilityRoutine);
    }

    public void DashOut()
    {
        currentAbility = Ability.DashOut;
        currentAbilityRoutine = IDashOut();
        StartCoroutine(currentAbilityRoutine);
    }

    public void CrystalsUp()
    {
        currentAbility = Ability.CrystalsUp;
        currentAbilityRoutine = ICrystalsUp();
        StartCoroutine(currentAbilityRoutine);
    }

    public void OrbitalStrike()
    {
        currentAbility = Ability.OrbitalStrike;
        currentAbilityRoutine = IOrbitalStrike();
        StartCoroutine(currentAbilityRoutine);
    }

    public void SwordsCircle()
    {
        currentAbility = Ability.SwordsCircle;
        currentAbilityRoutine = ISwordsCircle();
        StartCoroutine(currentAbilityRoutine);
    }

    public void DashSlam()
    {
        currentAbility = Ability.DashSlam;
        currentAbilityRoutine = IDashSlam();
        StartCoroutine(currentAbilityRoutine);
    }

    public void HorizontalPierce()
    {
        currentAbility = Ability.HorizontalPierce;
        currentAbilityRoutine = IHorizontalPierce();
        StartCoroutine(currentAbilityRoutine);
    }

    #endregion

    #region Routine functions

    private IEnumerator ISlash()
    {
        yield return null;
    }

    private IEnumerator IDashIn()
    {
        yield return null;
    }
    
    private IEnumerator IDashOut()
    {
        yield return null;
    }
    
    private IEnumerator ICrystalsUp()
    {
        yield return null;
    }
    
    private IEnumerator IOrbitalStrike()
    {
        yield return null;
    }
    
    private IEnumerator ISwordsCircle()
    {
        yield return null;
    }
    
    private IEnumerator IDashSlam()
    {
        yield return null;
    }

    private IEnumerator IHorizontalPierce()
    {
        yield return null;
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

    #endregion

    #region CoolDowns

    private IEnumerator ICooldownDashOut()
    {
        abilityAvailable[(int)Ability.DashOut] = false;
        yield return new WaitForSeconds(dashOutStats.cooldown);
        abilityAvailable[(int)Ability.DashOut] = true;
    }
    private IEnumerator ICooldownSwordsCircle()
    {
        abilityAvailable[(int)Ability.SwordsCircle] = false;
        yield return new WaitForSeconds(swordsCircleStats.cooldown);
        abilityAvailable[(int)Ability.SwordsCircle] = true;
    }

    #endregion
}
