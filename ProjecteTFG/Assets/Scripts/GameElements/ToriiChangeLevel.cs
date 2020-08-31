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

    public int dir;
    public void TransitionToLevel()
    {
        if (soundController != null)
        {
            soundController.FadeOutSound(transitionDuration);
        }
        StartCoroutine(ITransitionLevel());
    }

    private IEnumerator ITransitionLevel()
    {
        GameManager.instance.BlockInputs(true);

        if(dir == 0){
            FindObjectOfType<Player>().MoveToDir(Vector3.up);
        }
        else if (dir == 1)
        {
            FindObjectOfType<Player>().MoveToDir(Vector3.right);
        }
        else if (dir == 2)
        {
            FindObjectOfType<Player>().MoveToDir(Vector3.left);
        }

        ScreenManager.instance.StartFadeHideScreen(transitionDuration);
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadScene(level);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TransitionToLevel();
    }

}
