using UnityEngine;
using System.Collections;

[System.Serializable]
public class Globals
{
    //Game state
    public static GameState gameState = GameState.Zero;
    
    //Stats
    public static float deathCount = 0;
    public static float damageReceivedCount = 0;
    public static float killCount = 0;
    public static float damageDealtCount = 0;
    public static float crystalCount = 0;

    public Globals GetGlobals()
    {
        return this;
    }

    public static void LoadGlobals(SaveData saveData)
    {
        gameState = saveData.gameState;
        deathCount = saveData.deathCount;
        damageReceivedCount = saveData.damageReceivedCount;
        killCount = saveData.killCount;
        damageDealtCount = saveData.damageDealtCount;
        crystalCount = saveData.crystalCount;
    }

    public static void ResetGlobals()
    {
        gameState = GameState.Zero; 
        deathCount = 0;
        damageReceivedCount = 0;
        killCount = 0;
        damageDealtCount = 0;
        crystalCount = 0;
    }
}
