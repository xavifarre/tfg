using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToriiChangeLevel : MonoBehaviour
{

    public string level;
    public float transitionDuration;
    public SoundController soundController;
    public void TransitionToLevel()
    {
        StartCoroutine(ITransitionLevel());
    }

    private IEnumerator ITransitionLevel()
    {
        GameManager.instance.BlockInputs(true);
        FindObjectOfType<Player>().MoveToDir(Vector3.up);
        ScreenManager.instance.StartFadeHideScreen(transitionDuration);
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadScene(level);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TransitionToLevel();
    }

}
