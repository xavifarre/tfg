using UnityEngine;
using System.Collections;

public class Laser : Attack
{
    public float speed;
    public float duration;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = speed * transform.up;
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            //Envia el hit al player
            collider.GetComponent<Player>().Hit(this);
        }
    }
}
