using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    public float height;
    public GameObject shadow;
    public float minScale = 0.3f;
    public float maxHeight = 20;
    private Vector3 startingScale;
    private Vector3 startingPosition;

    private void Start()
    {
        startingPosition = shadow.transform.localPosition;
        startingScale = shadow.transform.localScale;    
    }

    private void LateUpdate()
    {
        if(height != 0)
        {
            shadow.transform.localPosition = new Vector3(shadow.transform.localPosition.x, startingPosition.y) - Vector3.up * height;
            shadow.transform.localScale = Vector3.Lerp(startingScale, startingScale * minScale, height / maxHeight);
        }
        
    }
}
