using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cliff : MonoBehaviour
{

    private Tilemap tilemap;
    private Vector3 hitTilePos;

    private float t;

    private Dictionary<int, float> offsetStates;

    private int lastState = -1;

    // Start is called before the first frame update
    void Start()
    {
        // {estat,frames}
        offsetStates = new Dictionary<int, float> { { 0, 1.5f }, { 1, 4f } };

        tilemap = transform.parent.GetComponent<Tilemap>();
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (offsetStates.ContainsKey((int)collision.gameObject.GetComponent<Player>().state))
            {
                t = 0;
            }
            else
            {
                TriggerFall(collision);
            }
        }
        else
        {
            TriggerFall(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (offsetStates.ContainsKey((int)collision.gameObject.GetComponent<Player>().state))
            {
                if(lastState == (int)collision.gameObject.GetComponent<Player>().state)
                {
                    t+= Time.deltaTime;
                }
                else
                {
                    t = 0;
                }
                lastState = (int)collision.gameObject.GetComponent<Player>().state;
                if (t >= offsetStates[(int)collision.gameObject.GetComponent<Player>().state])
                {
                    TriggerFall(collision);
                }
            }
            else
            {
                TriggerFall(collision);
            }
        }

    }

    void TriggerFall(Collision2D collision)
    {
        int n = 0;
        Vector2 contactsSum = Vector2.zero;
        foreach (ContactPoint2D hit in collision.contacts)
        {
            contactsSum += hit.point;
            n++;
        }
        contactsSum /= n;
        hitTilePos = contactsSum;
        Grid tileGrid = tilemap.layoutGrid;
        Vector3 tilePosition = tileGrid.WorldToCell(contactsSum) + new Vector3(0.5f, 0.5f, 0);
        collision.gameObject.SendMessage("Fall", tilePosition);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hitTilePos,0.1f);
    }
}
