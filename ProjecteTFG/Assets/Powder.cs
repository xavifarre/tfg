using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powder : MonoBehaviour
{
    public float delay;
    public float radius;
    public Explosion explosion;

    private float t;
    private bool exploded = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one * radius;
        Destroy(gameObject, delay + explosion.duration);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if(t >= delay && !exploded)
        {
            Explode();
        }
    }

    public void Explode()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        Explosion exp = Instantiate(explosion, transform);
        exploded = true;
    }
}
