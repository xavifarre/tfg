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


//Classe Poligon per a comprovar si un punt està dins d'un poligon donat
public static class Poly
{
    public static bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;
    }
}