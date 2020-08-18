using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Globals
{
    //Game state
    public static GameState gameState = GameState.Zero;
    
    //Stats
    public static float deathCount = 0;
    public static float damageReceivedCount = 0;
    public static float healCount = 0;
    public static float killCount = 0;
    public static float damageDealtCount = 0;
    public static float crystalCount = 0;
    public static DateTime startTimeStamp = DateTime.Now;
    public static float totalTime = 0;

    public Globals GetGlobals()
    {
        return this;
    }

    public static void LoadGlobals(SaveData saveData)
    {
        gameState = saveData.gameState;
        deathCount = saveData.deathCount;
        damageReceivedCount = saveData.damageReceivedCount;
        healCount = saveData.healCount;
        killCount = saveData.killCount;
        damageDealtCount = saveData.damageDealtCount;
        crystalCount = saveData.crystalCount;
        startTimeStamp = saveData.startTimeStamp;
    }

    public static void ResetGlobals()
    {
        gameState = GameState.Zero; 
        deathCount = 0;
        healCount = 0;
        damageReceivedCount = 0;
        killCount = 0;
        damageDealtCount = 0;
        crystalCount = 0;
        startTimeStamp = DateTime.Now;
    }
}
