using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biter : Minion
{
    //Charge
    public float chargeRange = 3f;
    public float chargeTime = 0.5f;

    //Attack
    public float attackDuration = 1;
    public float attackDistance = 5f;

    //Move
    protected override void UpdateMove()
    {
        Vector3 direction = (player.transform.position - realPos).normalized;
        realPos = realPos + speed * direction * Time.deltaTime;

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
            StartIdle();
        }
    }

    private void Charge()
    {
        state = MinionState.Charge;
        StartCoroutine(ICharge());
    }

    private void Attack()
    {
        startActionPoint = realPos;
        endActionPoint = realPos + (player.transform.position - startActionPoint).normalized * attackDistance;
        tAction = 0;
        state = MinionState.Attack;
    }

    IEnumerator ICharge()
    {
        yield return new WaitForSeconds(chargeTime);
        if (state == MinionState.Charge)
        {
            Attack();
        }
    }
}
