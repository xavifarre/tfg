using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public GameObject title;
    public float waitTitle;
    public GameObject startButton;
    public float tWait;
    public GameObject screenTransition;
    public float transitionTime; 
    public GameObject bg;
    public float startTime;

    void Start()
    {
        StartCoroutine(ILoadGame());
    }

    public IEnumerator ILoadGame()
    {

        SaveSystem.LoadGame();
        string scene = GetScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(startTime);
        bg.SetActive(true);
        yield return new WaitForSeconds(waitTitle);
        title.SetActive(true);

        float t = 0;
        while (!asyncLoad.isDone)
        {
            t += Time.deltaTime;
            if(asyncLoad.progress >= 0.9f && t > tWait)
            {
                startButton.SetActive(true);
                if (Input.GetButton("Pause") || Input.GetButton("Interact") || Input.GetButton("Attack") || Input.GetButton("Recall") || Input.anyKey)
                {
                    startButton.SetActive(false);
                    break;
                }
            }
            yield return null;
        }
        screenTransition.SetActive(true);
        yield return new WaitForSeconds(transitionTime);
        asyncLoad.allowSceneActivation = true;

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
