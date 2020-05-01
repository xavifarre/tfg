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

    private void Start()
    {
        startingScale = shadow.transform.localScale;
    }

    private void LateUpdate()
    {
        shadow.transform.localPosition = - Vector3.up * height;
        shadow.transform.localScale = Vector3.Lerp(startingScale, startingScale * minScale, height/maxHeight);
    }
}
