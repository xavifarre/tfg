using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enumerador d'estats genèric utilitzat pel player
public enum State {
    Idle,
    Attack,
    Dash,
    KnockBack,
    Fall,
    Dead
}; 

//Enumerador d'estats genèric utilitzat pels minions
public enum MinionState
{
    Idle,
    Move,
    Attack,
    Charge,
    KnockBack,
    Fall,
    Dead
};

//Enumerador d'estats genèric utilitzat pels minions
public enum GameState
{
    Zero,
    Started,
    SwordPicked,
    SummonerDefeated,
    PerserverDefeated,
    End
};


