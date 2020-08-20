using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ILoadGame());
    }


    private IEnumerator ILoadGame()
    {
        SaveSystem.LoadGame();
        string scene = GetScene();
        yield return new WaitForSeconds(1f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            Debug.Log("hey");
            yield return null;
        }
    }

    private string GetScene()
    {
        if (Globals.gameState == GameState.Zero)
        {
            return "MenuLevel";
        }
        else if(Globals.gameState == GameState.Started)
        {
            return "StartingLevel";
        }
        else if (Globals.gameState == GameState.SwordPicked || Globals.gameState == GameState.SummonerDefeated || Globals.gameState == GameState.PerserverDefeated)
        {
            return "ToriiLevel";
        }
        else if(Globals.gameState == GameState.End)
        {
            return "MenuLevel";
        }

        return "MenuLevel";
    }

}
