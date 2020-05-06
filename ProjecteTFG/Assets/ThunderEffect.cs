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
    private Camera cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
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
                Debug.Log(rand);
                StartThunder();
                t = 0;
            }
        }
        transform.position = (Vector2)cam.transform.position;
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

        while(ti < duration)
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
