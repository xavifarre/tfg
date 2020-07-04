using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndodgeableSpinCollider : Attack
{
    public void Initialize(Perserver._UndodgeableSpin stats)
    {
        transform.localScale = new Vector3(stats.radius, stats.radius);
        damage = stats.damage;
        knockback = stats.knockback;
        Disable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            //Envia el hit al player
            collider.GetComponent<Player>().Hit(this);
        }
    }
}
