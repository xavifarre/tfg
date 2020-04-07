using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Attack
{
    public GameObject particles;

    private void Start()
    {
        if (particles)
        {
            Instantiate(particles, transform.position, Quaternion.identity);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            //Envia el hit al enemic
            //collider.GetComponent<Enemy>().Hit(this);
        }

        if (collider.gameObject.tag == "Player")
        {
            //Envia el hit al player
            collider.GetComponent<Player>().Hit(this);
        }
    }
}
