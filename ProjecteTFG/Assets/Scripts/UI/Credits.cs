using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public SoundController soundController;


    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            EndCredits();
        }
    }

    public void EndCredits()
    {
        SceneManager.LoadScene("StatsScreen");
    }

    private void PlayMusic()
    {
        soundController.PlaySound("music_loop_summoner");
    }
}
