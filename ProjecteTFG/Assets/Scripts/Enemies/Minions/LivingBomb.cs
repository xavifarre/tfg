using UnityEngine;
using System.Collections;

public class LivingBomb : Minion
{
    [Header("Attack")]
    //Attack
    public float attackRange = 5f;

    //Range
    public float explosionRange = 4f;
    public GameObject explosionCollider;

    public float explosionChargeTime = 0.5f;

    private bool preparingExplosion = false;

    protected override void Init()
    {
        if((player.transform.position.x - realPos.x) > 0)
        {
            spriteRenderer.flipX = true;
        }
    }

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

        if (Vector3.Distance(endActionPoint, realPos) < 1f)
        {
            StopToExplode();
        }

        PixelPerfectMovement.Move(realPos, rb);
    }

    private void Attack()
    {
        endActionPoint = player.transform.position;
        tAction = 0;
        state = MinionState.Attack;
        PrepareExplosion();
    }

    public void PrepareExplosion()
    {
        preparingExplosion = true;
        animator.SetTrigger("Explode");
    }

    public void StopToExplode()
    {
        state = MinionState.Idle;
    }

    public void Explode()
    {
        state = MinionState.Dead;

        GameObject explosion = Instantiate(explosionCollider, transform.position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * explosionRange;

        Die();
    }

    protected override void KnockBack(float knockBack)
    {
        base.KnockBack(knockBack);
        animator.SetTrigger("Explode");

    }

    protected override void UpdateKnockBack()
    {
        Vector3 nextPos = MathFunctions.EaseOutExp(tAction, startActionPoint, endActionPoint, knockBackDuration, 5);
        realPos = nextPos;
        PixelPerfectMovement.Move(nextPos, rb);

        transform.Rotate(0, 0, 300 * Time.deltaTime, Space.Self);
        tAction += Time.deltaTime;

        if (tAction >= knockBackDuration)
        {
            state = MinionState.Dead;
        }
    }

    protected override void PlayerHit()
    {
        //Explode();
    }
}
