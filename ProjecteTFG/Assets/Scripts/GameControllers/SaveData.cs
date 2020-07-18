using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class SaveData
{
    //Game state
    public GameState gameState = GameState.Zero;

    //Stats
    public float deathCount;
    public float damageReceivedCount;
    public float killCount;
    public float damageDealtCount;
    public float crystalCount;

    //Timestamp
    public DateTime dateTime;

    public SaveData()
    {
        gameState = Globals.gameState;
        deathCount = Globals.deathCount;
        damageReceivedCount = Globals.damageReceivedCount;
        killCount = Globals.killCount;
        damageDealtCount = Globals.damageDealtCount;
        crystalCount = Globals.crystalCount;

        dateTime = DateTime.Now;
    }
}
