using UnityEngine;
using System.Collections;

public class BarrelPopable : Barrel
{
    [HideInInspector]
    public Perserver perserver;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        Blade blade = collision.GetComponent<Blade>();
        if (collision.tag == "EnemyAttack" && (!blade || blade.poppingBlade))
        {
            Explode();
            perserver.barrelPopped = true;
        }
    }
}
