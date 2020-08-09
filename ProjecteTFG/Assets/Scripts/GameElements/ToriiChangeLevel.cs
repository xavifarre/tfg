using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToriiChangeLevel : MonoBehaviour
{

    public string level;
    public float transitionDuration;

    public void TransitionToLevel()
    {
        StartCoroutine(ITransitionLevel());
    }

    private IEnumerator ITransitionLevel()
    {
        ScreenManager.instance.StartFadeHideScreen(transitionDuration);
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadScene(level);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TransitionToLevel();
    }

}
