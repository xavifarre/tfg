using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public float tLastHit = 0;

    private void Start()
    {
        PopupTextController.Initialize();
    }

    private void Update()
    {
        tLastHit += Time.deltaTime;
    }

    public void SlowDownGame(float tScale, float time)
    {
        StartCoroutine(ISlowTime(tScale, time));
    }

    private IEnumerator ISlowTime(float tScale, float time)
    {
        Time.timeScale = tScale;
        yield return new WaitForSeconds(time);
        Time.timeScale = 1;
    }
}
