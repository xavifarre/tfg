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
    public float healCount;
    public float killCount;
    public float damageDealtCount;
    public float crystalCount;
    public DateTime startTimeStamp;

    //Timestamp
    public DateTime dateTime;

    public SaveData()
    {
        gameState = Globals.gameState;
        deathCount = Globals.deathCount;
        healCount = Globals.healCount;
        damageReceivedCount = Globals.damageReceivedCount;
        killCount = Globals.killCount;
        damageDealtCount = Globals.damageDealtCount;
        crystalCount = Globals.crystalCount;
        startTimeStamp = Globals.startTimeStamp;

        dateTime = DateTime.Now;
    }
}
