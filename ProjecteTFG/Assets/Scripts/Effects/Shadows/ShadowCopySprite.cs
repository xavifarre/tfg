using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCopySprite : MonoBehaviour
{

    public SpriteRenderer other;
    private SpriteRenderer own;
    public bool copySprite = false;

    public float fadeTime = 1;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        own = GetComponent<SpriteRenderer>();
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
       

        if (!copySprite)
        {
            transform.localPosition = own.flipX ? new Vector3(-startPos.x, startPos.y, startPos.z) : new Vector3(startPos.x, startPos.y, startPos.z);
        }
        if (copySprite)
        {
            own.flipX = other.flipX;
            own.sprite = other.sprite;
        }
    }

    public void Fade()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(IFade());
        }  
    }

    private IEnumerator IFade()
    {
        float t = 0;
        Color color = own.color;

        while(t < fadeTime)
        {
            t += Time.deltaTime;
            own.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0, t / fadeTime));
            yield return null;
        }
    }
}
