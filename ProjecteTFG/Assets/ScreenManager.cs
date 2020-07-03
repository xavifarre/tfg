using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;

    public Image deathScreen;
    public HitScreen hitScreen;
    public Image blackScreen;

    private Player player;

    private void Start()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }

    public void ShowDeathScreen()
    {
        player.GetComponent<SpriteRenderer>().sortingLayerName = "DeathScreen";
        StartCoroutine(IDeathScreen());
    }

    private IEnumerator IDeathScreen()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        float fadeDuration = 2;
        float t = 0;
        deathScreen.gameObject.SetActive(true);
        Color c = deathScreen.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            deathScreen.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }

    public void StartFadeShowScreen(float duration, float delay = 0)
    {
        StartCoroutine(IFadeShowScreen(duration, delay));
    }

    private IEnumerator IFadeShowScreen(float duration, float delay)
    {
        float t = 0;
        blackScreen.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(delay);
        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            blackScreen.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, t / 5));
            yield return null;
        }
    }
}
