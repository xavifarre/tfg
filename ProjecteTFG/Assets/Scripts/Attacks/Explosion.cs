using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Attack
{
    public float duration;
    public GameObject particles;


    private void Start()
    {
        if (particles)
        {
            Instantiate(particles, transform.position, Quaternion.identity);
        }
        StartCoroutine(IDisableExplosionCollider(duration));
    }

    private IEnumerator IDisableExplosionCollider(float dur)
    {
        yield return new WaitForSeconds(dur);
        GetComponent<Collider2D>().enabled = false;
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

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}
