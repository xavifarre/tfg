using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;

    private Image deathScreen;
    private HitScreen hitScreen;
    private Image blackScreen;

    private Player player;

    private void Start()
    {
        instance = this;
        player = FindObjectOfType<Player>();
        deathScreen = GameObject.Find("DeathImage").GetComponent<Image>();
        hitScreen = FindObjectOfType<HitScreen>();
        blackScreen = GameObject.Find("ScreenOverlay").GetComponent<Image>();
    }

    public void ShowHitScreen()
    {
        hitScreen.ShowScreen();
    }

    public void ShowDeathScreen()
    {
        player.GetComponent<SpriteRenderer>().sortingLayerName = "DeathScreen";
        player.GetComponent<SpriteRenderer>().sortingOrder = 2;
        HealthBar.Hide();
        StartCoroutine(IDeathScreen());
    }

    private IEnumerator IDeathScreen()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        float fadeDuration = 2;
        float t = 0;
        deathScreen.enabled = true;
        Color c = deathScreen.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            deathScreen.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(5f);

        SceneManager.LoadScene("ToriiLevel");
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
            t += Time.unscaledDeltaTime;
            blackScreen.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, t / duration));
            yield return null;
        }
    }

    public void StartFadeHideScreen(float duration, float delay = 0)
    {
        StartCoroutine(IFadeHideScreen(duration, delay));
    }

    private IEnumerator IFadeHideScreen(float duration, float delay)
    {
        float t = 0;
        blackScreen.color = new Color(0, 0, 0, 0);
        yield return new WaitForSeconds(delay);
        t = 0;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            blackScreen.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, t / duration));
            yield return null;
        }
    }
}
