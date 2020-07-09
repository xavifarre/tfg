using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Basic stats")]
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
    protected SpriteRenderer spriteRenderer;

    //GameManager
    protected GameManager gm;

    protected Animator animator;

    protected Material defaultMaterial;

    [Header("Hit")]
    public Color hitColor = Color.red;
    public float hitColorDuration = 0.05f;
    public Material hitMaterial;
    private IEnumerator hitRoutine;

    protected ShadowCopySprite shadow;

    // Start is called before the first frame update
    protected void Start()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        gm = FindObjectOfType<GameManager>();

        shadow = GetComponentInChildren<ShadowCopySprite>();

        realPos = transform.position;
        maxHealth = health;

        Init();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected void Update()
    {
        UpdateEnemy();
    }

    ////Modificar ordre de layer segons la posició y
    //private void LateUpdate()
    //{
    //    if (spriteRenderer)
    //    {
    //        spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(this.spriteRenderer.bounds.min).y * -1;
    //    }
    //}

    public virtual void GetDamage(int damage)
    {
        health -= damage;
        CheckDeath();
        ShowDamage(damage);
        DamageTick();
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
        PopupTextController.CreatePopupTextDamage(damage.ToString(), realPos);
    }

    protected void ShowHeal(int heal)
    {
        PopupTextController.CreatePopupTextHeal(heal.ToString(), realPos);
    }

    protected abstract void Init();
    protected abstract void UpdateEnemy();

    public virtual void PlayerHit()
    {
        if (damage > 0)
        {
            player.GetComponent<Player>().Hit(this);
        }
    }

    public abstract void Hit(Attack attack);
    public abstract void Die();

    protected void ChangeLayerIgnore()
    {
        gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
    }

    protected void DamageTick()
    {
        if(hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }
        hitRoutine = IDamageTick();
        StartCoroutine(hitRoutine);
    }

    protected IEnumerator IDamageTick()
    {
        hitMaterial.color = hitColor;
        spriteRenderer.material = hitMaterial;
        yield return new WaitForSeconds(hitColorDuration);
        spriteRenderer.material = defaultMaterial;
        hitRoutine = null;
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        PlayerHit();
    //    }
    //}

    protected void FadeShadow()
    {
        if (shadow)
        {
            shadow.Fade();
        }
    }

    public virtual void DisableEnemy()
    {
        enabled = false;
    }

}
