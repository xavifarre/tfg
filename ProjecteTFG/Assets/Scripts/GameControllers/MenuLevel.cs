using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevel : MonoBehaviour
{
    public SoundController soundController;

    // Start is called before the first frame update
    void Start()
    {
        soundController.PlaySound("birds",0,true);
        soundController.FadeInSound(2);
        ScreenManager.instance.StartFadeShowScreen(4, 2);
        StartCoroutine(IStart());
    }

    private IEnumerator IStart()
    {
        GameManager.instance.BlockInputs(true);
        yield return new WaitForSeconds(4);
        GameManager.instance.BlockInputs(false);
    }
}
