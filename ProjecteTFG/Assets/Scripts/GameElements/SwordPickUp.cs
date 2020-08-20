using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordPickUp : MonoBehaviour, IInteractuableObject
{
    private Player player;
    public Attack swordAttack;

    private float invultTime;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    public void Interact()
    {
        ToriiLevelController.instance.PickSword();
        GetComponent<Animator>().SetTrigger("PickUp");
        GetComponent<Collider2D>().enabled = false;
        invultTime = player.invulnerableTime;
        GameManager.instance.Shake(13f, 0.2f, 1f);
        player.invulnerableTime = 0;
    }

    public void DestroySword()
    {
        player.invulnerableTime = invultTime;
        Destroy(gameObject);
    }

    public void PickSword()
    {
        player.PickSword();
    }

    public void HitPlayer()
    {
        player.Hit(swordAttack);
    }
}
