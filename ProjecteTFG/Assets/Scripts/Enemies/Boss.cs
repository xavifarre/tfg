using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public int fase = 0;

    //Vida fases
    public List<float> ratiosDamageFase = new List<float>();

    public override void Die()
    {
        
    }

    public override void Hit(Attack attack)
    {
        
    }

    protected override void Init()
    {
       
    }

    protected override void UpdateEnemy()
    {
       
    }

    protected override void PlayerHit()
    {

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerDash"))
        //{
        //    collision.gameObject.SendMessage("DashCrash", this);
        //}
    }
}
