using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public int fase = 0;

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    public override void Hit(Attack attack)
    {
        throw new System.NotImplementedException();
    }

    protected override void Init()
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerDash"))
        {
            collision.gameObject.SendMessage("DashCrash", this);
        }
    }
}
