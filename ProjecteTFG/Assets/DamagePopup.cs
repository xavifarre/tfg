using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public Vector2 force = new Vector2(500,1000);

    // Start is called before the first frame update
    void Start()
    {
        Player player = FindObjectOfType<Player>();
        Vector2 vForce;
        if((player.transform.position - transform.position).x > 0.5f)
        {
            vForce = new Vector2(Random.Range(-100, -10) / 100f * force.x, 1 * force.y);
        }
        else if((player.transform.position - transform.position).x < -0.5f)
        {
            vForce = new Vector2(Random.Range(10, 100) / 100f * force.x, 1 * force.y);
        }
        else
        {
            vForce = new Vector2(Random.Range(-100, 100) / 100f * force.x, 1 * force.y); ;
        }

        GetComponent<Rigidbody2D>().AddForce(vForce);

    }
}
