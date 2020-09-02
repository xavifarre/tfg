using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevelManager : MonoBehaviour
{
    public SoundController rainSound;
    public List<SoundController> thunderSounds;
    
    private void Start()
    {

        StartCoroutine(IStartLevel());
    }

    private IEnumerator IStartLevel()
    {

        GameManager.instance.BlockInputs(true);
        ScreenManager.instance.StartFadeHideScreen(0);
        ScreenManager.instance.StartFadeShowScreen(10f, 4);
        rainSound.FadeInSound(5, rainSound.GetSource().volume, 4);
        foreach (SoundController sc in thunderSounds)
        {
            sc.FadeInSound(5, sc.GetSource().volume, 4);
        }

        yield return new WaitForSeconds(5f);
        GameManager.instance.BlockInputs(false);
    }
}
