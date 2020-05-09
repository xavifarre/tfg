using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCopySprite : MonoBehaviour
{

    public SpriteRenderer other;
    private SpriteRenderer own;
    public bool copySprite = false;

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
        
        own.flipX = other.flipX;

        if (!copySprite)
        {
            transform.localPosition = own.flipX ? new Vector3(-startPos.x, startPos.y, startPos.z) : new Vector3(startPos.x, startPos.y, startPos.z);
        }
        if (copySprite)
        {
            own.sprite = other.sprite;
        }
    }
}
