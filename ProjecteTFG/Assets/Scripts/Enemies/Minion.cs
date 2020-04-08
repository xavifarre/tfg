using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Enemy, IFallableObject
{

    [HideInInspector]
    public MinionState state;

    //KnockBack
    public float knockBackResistance = 1;
    public float knockBackDuration = 0.2f;

    //Action
    protected Vector3 startActionPoint;
    protected Vector3 endActionPoint;

    //Fall
    public float fallTime = 1f;

    protected override void Init()
    {

    }

    protected override void UpdateEnemy()
    {
        if (state == MinionState.KnockBack)
        {
            UpdateKnockBack();
        }
        else if (state == MinionState.Move)
        {
            UpdateMove();
        }
        else if (state == MinionState.Attack)
        {
            UpdateAttack();
        }
        else if (state == MinionState.Idle)
        {
            tAction += Time.deltaTime;
            if (tAction > 1)
            {
                state = MinionState.Move;
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    //Move
    protected virtual void UpdateMove()
    {

    }

    protected virtual void UpdateAttack()
    {

    }

    protected virtual void UpdateKnockBack()
    {
        Vector3 nextPos = MathFunctions.EaseOutExp(tAction, startActionPoint, endActionPoint, knockBackDuration, 5);
        realPos = nextPos;
        PixelPerfectMovement.Move(nextPos, rb);

        tAction += Time.deltaTime;

        if (tAction >= knockBackDuration)
        {
            state = MinionState.Idle;
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

            state = MinionState.KnockBack;
            tAction = 0;
        }
    }

    public override void Die()
    {
        state = MinionState.Dead;
        Destroy(gameObject);
    }

    public void Fall(Vector3 fallPosition)
    {
        state = MinionState.Fall;
        vulnerable = false;
        StartCoroutine(FallableObject.IFallAnimation(fallPosition, gameObject, fallTime));
    }

    public void EndFall()
    {
        Die();
    }

    protected override void PlayerHit()
    {
        if (damage > 0)
        {
            player.GetComponent<Player>().EnemyHit(this);
        }
    }
}
