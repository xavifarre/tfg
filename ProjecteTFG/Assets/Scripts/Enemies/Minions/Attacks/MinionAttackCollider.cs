using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAttackCollider : MonoBehaviour
{

    private Enemy parent;

    private void Start()
    {
        parent = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            parent.PlayerHit();
        }
    }
}
