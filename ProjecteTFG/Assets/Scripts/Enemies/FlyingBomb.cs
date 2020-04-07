using UnityEngine;
using System.Collections;

public class FlyingBomb : Minion
{

    //Charge
    public float attackRange = 3f;

    //Range
    public float explosionRange = 4f;
    public GameObject explosionCollider;

    //Move
    protected override void UpdateMove()
    {
        Vector3 direction = (player.transform.position - realPos).normalized;
        realPos = realPos + speed * direction * Time.deltaTime;

        if (Vector3.Distance(player.transform.position, realPos) < attackRange)
        {
            Attack();
        }

        PixelPerfectMovement.Move(realPos, rb);
    }

    //Attack
    protected override void UpdateAttack()
    {
        Vector3 direction = (endActionPoint - realPos).normalized;
        realPos = realPos + speed * direction * Time.deltaTime;

        if (Vector3.Distance(endActionPoint, realPos) < 0.1f)
        {
            Explode();
        }

        PixelPerfectMovement.Move(realPos, rb);
    }

    private void Attack()
    {
        endActionPoint = player.transform.position;
        tAction = 0;
        state = MinionState.Attack;
    }

    public void Explode()
    {
        state = MinionState.Dead;

        GameObject explosion = Instantiate(explosionCollider, transform.position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * explosionRange;

        Die();
    }

    protected override void UpdateKnockBack()
    {
        Vector3 nextPos = MathFunctions.EaseOutExp(tAction, startActionPoint, endActionPoint, knockBackDuration, 5);
        realPos = nextPos;
        PixelPerfectMovement.Move(nextPos, rb);

        tAction += Time.deltaTime;

        if (tAction >= knockBackDuration)
        {
            Explode();
        }
    }

    protected override void PlayerHit()
    {
        Explode();
    }
}
