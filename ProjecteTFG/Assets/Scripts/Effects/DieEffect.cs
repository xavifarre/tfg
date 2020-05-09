using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEffect : MonoBehaviour
{
    public float duration = 1;
    public Material disolveMaterialDie;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TriggerDie()
    {
        StartCoroutine(IDie());
    }

    IEnumerator IDie()
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            spriteRenderer.material = disolveMaterialDie;
            spriteRenderer.material.SetFloat("_Fade", Mathf.Lerp(1, 0, t / duration));
            yield return null;
        }
        Destroy(gameObject);
    }
}
