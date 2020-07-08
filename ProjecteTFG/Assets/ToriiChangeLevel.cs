using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToriiChangeLevel : MonoBehaviour
{

    public string level;
    public float transitionDuration;
    public Image blackScreen;


    public void TransitionToLevel()
    {
        StartCoroutine(ITransitionLevel());
        
    }

    private IEnumerator ITransitionLevel()
    {
        Color c = blackScreen.color;
        float t = 0;
        while(t < transitionDuration)
        {
            t += Time.deltaTime;
            blackScreen.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t / transitionDuration));
            yield return null;
        }

        SceneManager.LoadScene(level);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TransitionToLevel();
    }

}
