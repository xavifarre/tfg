using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevelManager : MonoBehaviour
{
    public SoundController rainSound;
    public List<SoundController> thunderSounds;
    
    private void Start()
    {
        
        ScreenManager.instance.StartFadeHideScreen(0);
        ScreenManager.instance.StartFadeShowScreen(10f,2);
        rainSound.FadeInSound(5, rainSound.GetSource().volume,1);
        StartCoroutine(IStartLevel());
        foreach(SoundController sc in thunderSounds)
        {
            sc.FadeInSound(5, sc.GetSource().volume,1);
        }
    }

    private IEnumerator IStartLevel()
    {
        GameManager.instance.BlockInputs(true);
        yield return new WaitForSeconds(5f);
        GameManager.instance.BlockInputs(false);
    }
}
