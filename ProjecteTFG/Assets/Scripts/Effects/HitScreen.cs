using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitScreen : MonoBehaviour
{
    public float fadeTime = 1;

    public Image hitScreenImage;

    public void ShowScreen()
    {
        StopAllCoroutines();
        StartCoroutine(IShowScreen());
    }

    private IEnumerator IShowScreen()
    {
        Color c = hitScreenImage.color;

        float t = 0;
        while(t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            hitScreenImage.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, t / fadeTime));
            yield return null;
        }
    }
}
