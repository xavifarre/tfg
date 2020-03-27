using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //Basic variables
    public int damage;
    public int size;
    public int health;


    //Is vulnerable
    protected bool vulnerable = true;

    //Variable de control d'accions
    protected float tAction;

    //RigidBody
    protected Rigidbody2D rb;

    protected Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        CheckDeath();
    }

    protected void CheckDeath()
    {
        if(health < 0)
        {
            Die();
        }
    }



    protected abstract void Init();
    public abstract void Hit(Attack attack);
    public abstract void Die();
}
