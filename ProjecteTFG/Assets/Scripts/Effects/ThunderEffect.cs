using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float t = 0;

    public float minIntervalTime = 1;
    public float probabilityPerSecond = 0.3f;
    public float duration;
    public bool fade;

    private Color initialColor;

    public SoundController soundController1;
    public SoundController soundController2;
    public SoundController soundController3;

    private int lastRand = -1;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if(t > minIntervalTime)
        {
            float rand = Random.value;

            if (rand < probabilityPerSecond * Time.deltaTime)
            {
                StartThunder();
                t = 0;
            }
        }
    }

    private void StartThunder()
    {
        spriteRenderer.enabled = true;
        spriteRenderer.color = initialColor;
        StartCoroutine(IThunder());
    }

    private IEnumerator IThunder()
    {

        float ti = 0;

        int rand = Random.Range(0, 3);
        while(lastRand == rand)
        {
            rand = Random.Range(0, 3);
        }
        lastRand = rand;
        if(rand == 0)
        {
            soundController1.PlaySound("thunder01");
            soundController1.RandomPitch(0.5f, 1.5f);
        }
        else if(rand == 1)
        {
            soundController2.PlaySound("thunder02");
            soundController2.RandomPitch(0.5f, 1.5f);
        }        
        else if(rand == 2)
        {
            soundController3.PlaySound("thunder03");
            soundController3.RandomPitch(0.5f, 1.5f);
        }


        while (ti < duration)
        {
            ti += Time.deltaTime;
            if (fade)
            {
                spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(initialColor.a, 0, ti/duration));
            }

            yield return null;
        }
        spriteRenderer.enabled = false;
    }
}
