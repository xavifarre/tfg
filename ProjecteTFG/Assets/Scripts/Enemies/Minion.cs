using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Enemy
{
    [HideInInspector]
    public State state;

    //KnockBack
    public float knockBackResistance = 1;
    public float knockBackDuration = 10;

    //Action
    protected Vector3 startActionPoint;
    protected Vector3 endActionPoint;

    //Fall
    public int fallFrames = 60;

    protected override void Init()
    {

    }

    protected override void UpdateEnemy()
    {
        //throw new System.NotImplementedException();
    }

    private void FixedUpdate()
    {
        if(state == State.KnockBack)
        {
            Vector3 nextPos = MathFunctions.EaseOutExp(tAction,startActionPoint,endActionPoint,knockBackDuration, 5);
            PixelPerfectMovement.Move(nextPos, rb);

            tAction++;

            if (tAction >= knockBackDuration)
            {
                state = State.Idle;
            }

        }
    }

    public override void Hit(Attack attack)
    {
        if (vulnerable)
        {
            GetDamage(attack.damage);
            KnockBack(attack.knockback);
        }
    }

    private void KnockBack(float knockBack)
    {
        if (knockBackResistance > 0)
        {
            float knockBackValue = knockBack / knockBackResistance;
            startActionPoint = transform.position;
            endActionPoint = startActionPoint + (Vector3)player.lastDir * knockBackValue;

            state = State.KnockBack;
            tAction = 0;
        }
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    public void Fall(Vector3 fallPosition)
    {
        state = State.Fall;
        vulnerable = false;
        StartCoroutine(FallableObject.IFallAnimation(fallPosition, gameObject, fallFrames));
    }

    public void EndFall()
    {
        state = State.Dead;
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(damage > 0)
            {
                collision.gameObject.GetComponent<Player>().EnemyHit(this);
            }
        }
    }
}
