using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainObject : MonoBehaviour
{
    private SoundController soundController;
    // Start is called before the first frame update
    void Start()
    {
        soundController = GetComponent<SoundController>();
        soundController.PlaySound("rain");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (Vector2)Camera.main.transform.position;
    }
}
