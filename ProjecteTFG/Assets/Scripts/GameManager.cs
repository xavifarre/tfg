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

    [HideInInspector]
    public float tLastHit = 0;
    private Player player;

    [HideInInspector]
    public bool gamePaused = false;
    [HideInInspector]
    public bool inputsBlocked = false;

    private GameObject enemyContainer;
    private IEnumerator slowCoroutine;
    private void Start()
    {
        instance = this;

        PopupTextController.Initialize();
        player = FindObjectOfType<Player>();

        //Initialize postProcessiong volume
        rpVolume.profile.TryGet(out volumeColors);
        rpVolume.profile.TryGet(out volumeBloom);

        enemyContainer = GameObject.Find("Enemies");
    }

    private void Update()
    {
        tLastHit += Time.deltaTime;
        if (Input.GetButtonDown("Pause"))
        {
            if (gamePaused) ResumeGame();
            else PauseGame();
        }
    }

    public void StopGame()
    {
        Time.timeScale = 0;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        PauseCanvas.Show();
        gamePaused = true;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        PauseCanvas.Hide();
        gamePaused = false;
        AudioListener.pause = false;
    }

    public void BlockInputs(bool block)
    {
        inputsBlocked = block;
    }

    public void SlowDownGame(float tScale, float time)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = ISlowTime(tScale, time);
        StartCoroutine(slowCoroutine);
    }

    private IEnumerator ISlowTime(float tScale, float time)
    {
        Time.timeScale = tScale;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }

    public void SlowDownGameLerp(float tScale, float lerpTime, float time)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = ISlowTimeLerp(tScale,lerpTime, time);
        StartCoroutine(slowCoroutine);
    }

    private IEnumerator ISlowTimeLerp(float tScale, float lerpTime, float time)
    {
        float t = 0;
        while (t < lerpTime)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1, tScale, t / lerpTime);
            yield return null;
        }
        yield return new WaitForSeconds(time - lerpTime - lerpTime);
        t = 0;
        while (t < lerpTime)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(tScale,1, t / lerpTime);
            yield return null;
        }
        Time.timeScale = 1;
    }



    //Camera shake
    public void Shake(float duration, float magnitude, float tInc = 0)
    {
        StartCoroutine(IShake(duration, magnitude, tInc));
    }

    IEnumerator IShake(float duration, float magnitude, float tInc)
    {
        float elapsed = 0.0f;
        float tMagnitude = 0;
        Vector3 originalCamPos = Camera.main.transform.position;
        Debug.Log(originalCamPos);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            tMagnitude = Mathf.Lerp(0, magnitude, elapsed / tInc);
            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= tMagnitude * damper;
            y *= tMagnitude * damper;

            Camera.main.transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

            yield return null;
        }

        Camera.main.transform.position = originalCamPos;
    }

    public void DisableEnemies()
    {
        if (enemyContainer)
        {
            foreach (Transform child in enemyContainer.transform)
            {
                Enemy enemy = child.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.StopAllCoroutines();
                    enemy.DisableEnemy();
                }
            }
        }
    }

    public void RedToBlackFilter(float redDuration, float blackDuration)
    {

        StartCoroutine(IRedToBlack(redDuration, blackDuration));
    }

    private IEnumerator IRedToBlack(float redDuration, float blackDuration)
    {
        
        float t = 0;
        while(t < redDuration)
        {
            t += Time.unscaledDeltaTime;
            volumeColors.colorFilter.value = Color.Lerp(Color.white, Color.red, t / redDuration);
            volumeColors.SetAllOverridesTo(true);

            yield return null;
        }

        t = 0;

        while (t < blackDuration)
        {
            t += Time.unscaledDeltaTime;
            volumeColors.colorFilter.value =Color.Lerp(Color.red, Color.black, t / blackDuration);
            volumeColors.SetAllOverridesTo(true);
            yield return null;
        }

    }

}
