using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //Basic variables
    public int damage;
    public int size;
    public int health;
    public float knockBackValue = 1;

    //Is vulnerable
    protected bool vulnerable = true;

    //Variable de control d'accions
    protected float tAction;

    //RigidBody
    protected Rigidbody2D rb;

    protected Player player;

    //Sprite Renderer
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Modificar ordre de layer segons la posició y
    private void LateUpdate()
    {
        spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(this.spriteRenderer.bounds.min).y * -1;
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        CheckDeath();
    }

    protected void CheckDeath()
    {
        if(health <= 0)
        {
            Die();
        }
    }

    protected abstract void Init();
    public abstract void Hit(Attack attack);
    public abstract void Die();
}
