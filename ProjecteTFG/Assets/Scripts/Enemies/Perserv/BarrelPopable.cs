using UnityEngine;
using System.Collections;

public class BarrelPopable : Barrel
{
    [HideInInspector]
    public Perserver perserver;

    private void Start()
    {
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Blade blade = collision.GetComponent<Blade>();
        if (collision.tag == "EnemyAttack" && (!blade || blade.poppingBlade))
        {
            Explode();
            perserver.barrelPopped = true;
        }
    }
}
