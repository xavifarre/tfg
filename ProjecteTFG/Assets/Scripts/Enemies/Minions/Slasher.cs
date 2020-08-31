using UnityEngine;
using System.Collections;

public class Slasher : Minion
{
    [Header("Attack")]
    //Charge
    public float chargeRange = 6f;
    public float chargeTime = 1f;

    //Attack
    public float attackDuration = 1;
    public float attackDistance = 10f;

    protected override void Init()
    {
        base.Init();
        soundController.PlaySound("slasher_spawn", 0.5f);
    }

    //Move
    protected override void UpdateMove()
    {
        Vector3 direction = (player.transform.position - realPos).normalized;
        realPos = realPos + speed * direction * Time.deltaTime;

        UpdateSpriteFlip();

        if (Vector3.Distance(player.transform.position, realPos) < chargeRange)
        {
            Charge();
        }
        PixelPerfectMovement.Move(realPos, rb);
    }

    //Attack
    protected override void UpdateAttack()
    {
        Vector3 nextPos = MathFunctions.EaseOutExp(tAction, startActionPoint, endActionPoint, attackDuration, 5);
        realPos = nextPos;
        PixelPerfectMovement.Move(nextPos, rb);

        tAction += Time.deltaTime;

        if (tAction >= attackDuration)
        {
            animator.SetTrigger("EndAction");
            UpdateSpriteFlip();
            StartIdle();
        }
    }

    private void Charge()
    {
        Debug.Log(state);
        UpdateSpriteFlip();
        state = MinionState.Charge;
        animator.SetBool("AttackDown", player.transform.position.y - realPos.y < 0);
        animator.ResetTrigger("EndAction");
        animator.SetTrigger("Attack");
        StartCoroutine(ICharge());
        Debug.Log(state);
        soundController.PlaySound("slasher_attack");
    }

    private void Attack()
    {
        tAction = 0;
        state = MinionState.Attack;
        UpdateSpriteFlip();
    }

    private IEnumerator ICharge()
    {
        yield return new WaitForSeconds(chargeTime * 0.7f);
        startActionPoint = realPos;
        endActionPoint = realPos + (player.transform.position + (Vector3)player.lastDir.normalized * player.movementValue.magnitude * player.speed * chargeTime * 0.5f - startActionPoint).normalized * attackDistance;
        yield return new WaitForSeconds(chargeTime * 0.3f);
        if (state == MinionState.Charge)
        {
            Attack();
        }
    }

    public override void Die()
    {
        base.Die();
        soundController.PlaySound("slasher_die");
    }

    public override void GetDamage(int damage)
    {
        soundController.PlaySound("slasher_hit");
        base.GetDamage(damage);
    }

    protected override void KnockBack(float knockBack)
    {
        base.KnockBack(knockBack);
        animator.SetTrigger("EndAction");
        UpdateSpriteFlip();
    }
}
