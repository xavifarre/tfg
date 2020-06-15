using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss stats")]
    //Fase
    public int fase = 0;

    //Vida fases
    public List<float> ratiosDamageFase = new List<float>();

    //Knockback
    protected Vector3 startKnockback;
    protected Vector3 endKnockback;
    protected float timeKnockback;

    [Header("Boss kill slowdown")]
    public float killSlowTime = 3f;
    public float killSlowScale = 0.2f;

    public override void Die()
    {
        
    }

    public override void Hit(Attack attack)
    {
        if (vulnerable)
        {
            GetDamage(attack.damage);
        }
    }

    protected override void Init()
    {
       
    }

    protected override void UpdateEnemy()
    {
       
    }


    protected virtual void KnockBack(float knockBack, float knockbackTime)
    {
        startKnockback = transform.position;
        endKnockback = startKnockback + (Vector3)player.lastDir * knockBack;
        timeKnockback = knockbackTime;
        tAction = 0;
    }

    protected virtual int CheckDamageFase()
    {
        for (int i = 0; i < ratiosDamageFase.Count; i++)
        {
            if (health > maxHealth * ratiosDamageFase[i])
            {
                return i;
            }
        }
        return -1;
    }

    protected virtual void StartFase(int f)
    {
        fase = f;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    //if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerDash"))
    //    //{
    //    //    collision.gameObject.SendMessage("DashCrash", this);
    //    //}
    //}
}
