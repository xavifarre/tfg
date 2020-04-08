using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public int area;
    private CameraManager cm;
    // Start is called before the first frame update
    void Start()
    {
        cm = FindObjectOfType<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            cm.AreaChange(area);
        }
    }
}
