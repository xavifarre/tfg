using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //Basic variables
    public int damage;
    public float speed;
    public int size;
    public int health;
    public float knockBackValue = 1;

    //Is vulnerable
    protected bool vulnerable = true;
    protected int maxHealth;

    //Variable de control d'accions
    protected float tAction;

    //RigidBody
    protected Rigidbody2D rb;
    protected Vector3 realPos;

    protected Player player;

    //Sprite Renderer
    private SpriteRenderer spriteRenderer;

    //GameManager
    protected GameManager gm;

    // Start is called before the first frame update
    protected void Start()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        gm = FindObjectOfType<GameManager>();

        realPos = transform.position;
        maxHealth = health;

        Init();
    }

    // Update is called once per frame
    protected void Update()
    {
        UpdateEnemy();
    }

    //Modificar ordre de layer segons la posició y
    private void LateUpdate()
    {
        if (spriteRenderer)
        {
            spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(this.spriteRenderer.bounds.min).y * -1;
        }
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        CheckDeath();
        ShowDamage(damage);
        gm.tLastHit = 0;
    }

    protected void CheckDeath()
    {
        if(health <= 0)
        {
            Die();
        }
    }

    protected void ShowDamage(int damage)
    {
        PopupTextController.CreatePopupText(damage.ToString(), realPos);
    }

    protected abstract void Init();
    protected abstract void UpdateEnemy();
    protected abstract void PlayerHit();

    public abstract void Hit(Attack attack);
    public abstract void Die();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerHit();
        }
    }
}
