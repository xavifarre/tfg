using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class SaveData
{
    //Essential
    public static int bossesDefeated;
    public static bool swordPicked;

    //Stats
    public static float deathCount;
    public static float killCount;
    public static float damageCount;
    public static float crystalCount;

    //Timestamp
    public DateTime dateTime;

    public SaveData()
    {
        bossesDefeated = Globals.bossesDefeated;
        swordPicked = Globals.swordPicked;
        deathCount = Globals.deathCount;
        killCount = Globals.killCount;
        damageCount = Globals.damageCount;
        crystalCount = Globals.crystalCount;

        dateTime = DateTime.Now;
    }
}
