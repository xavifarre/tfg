using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndodgeableSpinCollider : Attack
{
    private float radius;
    private Player player;

    public void Initialize(Perserver._UndodgeableSpin stats)
    {
        player = FindObjectOfType<Player>();
        radius = stats.radius;
        damage = stats.damage;
        knockback = stats.knockback;
    }

    public void Enable()
    {
        if(Vector3.Distance(player.transform.position, transform.position) <= radius)
        {
            player.Hit(this);
        }
    }
}
