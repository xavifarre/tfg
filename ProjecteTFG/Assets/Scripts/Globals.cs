using UnityEngine;
using System.Collections;

public class Globals
{
    //Essential
    public static int bossesDefeated = 0;
    public static bool swordPicked = false;

    //Stats
    public static float deathCount = 0;
    public static float killCount = 0;
    public static float damageCount = 0;
    public static float crystalCount = 0;

    public Globals GetGlobals()
    {
        return this;
    }
}
