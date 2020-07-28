using UnityEngine;
using System.Collections;

public class SlashDestroyer : Attack
{    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Player>().Hit(this);
        }
    }
}
