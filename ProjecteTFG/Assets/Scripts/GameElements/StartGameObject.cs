using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameObject : MonoBehaviour, IInteractuableObject
{
    public SoundController soundController;
    public void Interact()
    {
        soundController.FadeOutSound(5f);
        StartCoroutine(IStartGame());
    }

    private IEnumerator IStartGame()
    {
        Globals.gameState = GameState.Started;
        SaveSystem.SaveGame();
        GameManager.instance.BlockInputs(true);
        ScreenManager.instance.StartFadeHideScreen(5f);
        yield return new WaitForSeconds(5f);
        Globals.startTimeStamp = DateTime.Now;
        SceneManager.LoadScene("StartingLevel");
    }

}
