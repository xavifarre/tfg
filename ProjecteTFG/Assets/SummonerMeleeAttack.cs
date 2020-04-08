using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerMeleeAttack : Attack
{
    private Collider2D coll;
    public float activeTime = 0.3f;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    public void PerformAttack(Vector2 vdir)
    {
        int dir = MathFunctions.GetDirection(vdir);
        transform.eulerAngles = new Vector3(0, 0, dir * 90);
        StartCoroutine(IAttack());
    }

    IEnumerator IAttack()
    {
        coll.enabled = true;
        yield return new WaitForSeconds(activeTime);
        coll.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<Player>().Hit(this);
        }
    }
}
