using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seal : MonoBehaviour
{

    public float floatAngularSpeed;
    public float floatAmplitude;
    public Material disolveMaterial;

    private float tAction = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tAction += Time.deltaTime;
        transform.position = transform.position + FloatY() * Vector3.up;
    }

    private float FloatY()
    {
        return floatAmplitude * Mathf.Sin(floatAngularSpeed * tAction * Mathf.PI);
    }

    public void Fade(float fadeDuration)
    {
        StartCoroutine(IFade(fadeDuration));
    }

    private IEnumerator IFade(float fadeDuration)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.material = disolveMaterial;
        float t = 0;
        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            sprite.material.SetFloat("_Fade", Mathf.Lerp(1, 0, t / fadeDuration));
            yield return null;
        }
    }
}
