using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public Volume rpVolume;

    public static GameManager instance;

    private ColorAdjustments volumeColors;
    private Bloom volumeBloom;

    public SpriteRenderer deathScreen;
    public HitScreen hitScreen;

    [HideInInspector]
    public float tLastHit = 0;
    private Player player;

    [HideInInspector]
    public bool gamePaused = false;

    private void Start()
    {
        instance = this;

        PopupTextController.Initialize();
        player = FindObjectOfType<Player>();

        //Initialize postProcessiong volume
        rpVolume.profile.TryGet(out volumeColors);
        rpVolume.profile.TryGet(out volumeBloom);
    }

    private void Update()
    {
        tLastHit += Time.deltaTime;
        if (Input.GetButtonDown("Pause"))
        {
            gamePaused = gamePaused ? ResumeGame() : PauseGame();
        }
    }

    public bool PauseGame()
    {
        Time.timeScale = 0;
        PauseCanvas.Show();
        return true;
    }

    public bool ResumeGame()
    {
        Time.timeScale = 1;
        PauseCanvas.Hide();
        return false;
    }

    public void SlowDownGame(float tScale, float time)
    {
        StartCoroutine(ISlowTime(tScale, time));
    }

    private IEnumerator ISlowTime(float tScale, float time)
    {
        Time.timeScale = tScale;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
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
            t+= Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            deathScreen.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }

    //Camera shake
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(IShake(duration, magnitude));
    }

    IEnumerator IShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        Vector3 originalCamPos = Camera.main.transform.position;
        Debug.Log(originalCamPos);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            Camera.main.transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

            yield return null;
        }

        Camera.main.transform.position = originalCamPos;
    }

}
