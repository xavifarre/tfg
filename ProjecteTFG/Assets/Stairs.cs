using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public bool vertical;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!vertical)
        {
            player.speedModifier = new Vector3(Globals.stairSpeedModifier, 1, 1);
        }
        else
        {
            player.speedModifier = new Vector3(1, Globals.stairSpeedModifier,1);
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player.speedModifier = Vector3.one;
    }
}
