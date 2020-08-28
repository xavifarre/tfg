using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : Attack
{
    public float explosionRadius;
    public Explosion explosion;

    protected bool exploded = false;

    protected Animator animator;

    protected void Init()
    {
        animator = GetComponent<Animator>();
    }

    public void Explode()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        Explosion exp = Instantiate(explosion, transform);
        exp.transform.localScale = Vector3.one * explosionRadius;
        exp.knockback = knockback;
        exp.damage = damage;
        exploded = true;
        Destroy(gameObject, 2);
    }



}
