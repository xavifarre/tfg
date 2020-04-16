using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perserver : Boss
{
    #region Variables

    //State
    public enum PerserverState { Idle, Move, Ability, Damaged, Die, Start };
    public PerserverState state;

    //Ability
    public enum Ability { Spin, SpinHeal, ExpandingSpin, UndodgeableSpin, PowderDrop, BarrelPop, BarrelToss, BarrelDrop, DoubleSlash, Heal, None };
    public Ability ability;

    [Header("Blades")]
    //Blades
    public float basicRadius;
    public float basicAngularSpeed;
    private List<Blade> blades;


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

    #endregion


    #region Abilities Structs

    [System.Serializable]
    public struct _Spin
    {
        public float angularSpeed;
        public float radius;
        public float duration;
        public float growDuration;
    }

    [System.Serializable]
    public struct _SpinHeal
    {
        public float angularSpeed;
        public float maxAngularSpeed;
        public float maxRadius;
        public float recallSpeed;
        public float recallDuration;
        public float duration;
    }

    [System.Serializable]
    public struct _ExpandingSpin
    {
        public float angularSpeed;
        public float maxAngularSpeed;
        public float maxRadius;
        public float recallDuration;
        public float duration;
    }

    [System.Serializable]
    public struct _UndodgeableSpin
    {
        public float angularSpeed;
        public float angularMaxSpeed;
        public float duration;
    }

    [System.Serializable]
    public struct _PowderDrop
    {
        public float radius;
        public float spinSpeed;
        public float maxSpinSpeed;
        public float spinDuration;
        public float powderDuration;
        public float duration;
        //public Powder powderObject;
    }

    [System.Serializable]
    public struct _BarrelPop
    {
        public float popSpeed;
        public float recallSpeed;
        public float distanceToPlayer;
        public float placementDuration;
        public float recallDuration;
        //public Barrel barrelObject;
    }

    [System.Serializable]
    public struct _BarrelToss
    {
        public float spinSpeed;
        public float maxSpinSpeed;
        public float spinDuration;
        public float barrelSpeed;
        //public Barrel barrelObject;
    }

    [System.Serializable]
    public struct _BarrelDrop
    {
        public int nBarrels;
        public float minimumSeparation;
        public float explosionDelay;
        public float castInterval;
        //public Barrel barrelObject;
    }

    [System.Serializable]
    public struct _DoubleSlash
    {
        public float expandDistance;
        public float expandDelay;
        public float speed;
        public float chargeDuration;
        public float recoverDuration;
    }

    [System.Serializable]
    public struct _Heal
    {
        public float healAmount;
        public float healRate;
        public float evaluationRate;
        public float firstEvaluationMultiplier;
    }

    #endregion

    protected override void Init()
    {
        blades = new List<Blade>();
        foreach(Blade blade in transform.GetComponentsInChildren<Blade>())
        {
            blades.Add(blade);
            blade.basicRadius = basicRadius;
            blade.basicAngularSpeed = basicAngularSpeed;
            blade.InitPosition();
        }
        
    }

    protected override void UpdateEnemy()
    {
        if(Input.GetKeyDown("l"))
        {
            ShowHeal(Random.Range(5,20));
        }
        if (Input.GetKeyDown("j"))
        {
            Spin();
        }
        if (Input.GetKeyDown("h"))
        {
            ExpandingSpin();
        }
    }


    public void Spin()
    {
        foreach(Blade blade in blades)
        {
            blade.StartAbility(Blade.BladeAbility.Spin);
        }
    }

    public void ExpandingSpin()
    {
        foreach (Blade blade in blades)
        {
            blade.StartAbility(Blade.BladeAbility.ExpandingSpin);
        }
    }
}

