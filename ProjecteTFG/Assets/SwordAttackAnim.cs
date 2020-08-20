using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttackAnim : Attack
{
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void HitPlayer()
    {
        player.Hit(this);
    }
}
