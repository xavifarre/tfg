using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;
    public enum Level { Summoner, Preserver, Destroyer};
    public Level level;

    public float introDelay;
    public float loopPlayDelay;
    public float loopReplayDelay;
    public bool stop;
    public SoundController introController;
    public SoundController loopController;

    private void Start()
    {
        instance = this;
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (level == Level.Summoner)
        {
            introController.PlaySound("music_intro_summoner", introDelay);
            StartCoroutine(IPlayLoop("music_loop_summoner"));
        }
        else if (level == Level.Preserver)
        {
            introController.PlaySound("music_intro_preserver", introDelay);
            StartCoroutine(IPlayLoop("music_loop_preserver"));
        }
        else if (level == Level.Destroyer)
        {
            introController.PlaySound("music_intro_destroyer", introDelay);
            StartCoroutine(IPlayLoop("music_loop_destroyer"));
        }
    }

    public void StopMusic()
    {
        stop = true;
        introController.StopSound();
        loopController.StopSound();
    }

    private IEnumerator IPlayLoop(string id)
    {
        yield return new WaitForSeconds(loopPlayDelay + introDelay);
        while (!stop)
        {
            loopController.PlaySound(id);
            yield return new WaitForSeconds(loopReplayDelay);
        }
        
    }
}
