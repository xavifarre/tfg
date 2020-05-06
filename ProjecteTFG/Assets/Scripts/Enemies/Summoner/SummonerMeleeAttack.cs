using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerMeleeAttack : Attack
{
    private Collider2D coll;
    public float activeTime = 0.3f;

    public ParticleSystem particles;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        particles.Stop();
    }

    public void PerformAttack(int dir)
    {
        transform.eulerAngles = new Vector3(0, 0, dir * 90);
        StartCoroutine(IAttack());
       
    }

    IEnumerator IAttack()
    {
        coll.enabled = true;
        particles.Play();
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<Player>().Hit(this);
        }
    }
}
