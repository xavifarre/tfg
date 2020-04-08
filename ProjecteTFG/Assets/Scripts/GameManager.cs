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
}
