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

    [Header("Float")]
    public float floatAmplitude;
    public float floatAngularSpeed;
    private ShadowController shadowController;

    protected override void Init()
    {
        UpdateSpriteFlip();
        shadowController = GetComponentInChildren<ShadowController>();
    }

    //Move
    protected override void UpdateMove()
    {
        tAction += Time.deltaTime;
        Vector3 direction = (player.transform.position - realPos).normalized;
        realPos = realPos + speed * direction * Time.deltaTime;

        Vector3 floatingPosition = realPos + FloatY() * Vector3.up;
        shadowController.height = FloatY();

        UpdateSpriteFlip();

        if (Vector3.Distance(player.transform.position, realPos) < attackRange)
        {
            StopToExplode();
        }

        PixelPerfectMovement.Move(floatingPosition, rb);
    }

    //Fa flotar el enemic en forma de funció sinusoidal
    private float FloatY()
    {
        return floatAmplitude * Mathf.Sin(floatAngularSpeed * tAction * Mathf.PI);
    }


    public void StopToExplode()
    {
        animator.SetTrigger("Explode");
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
        shadowController.gameObject.SetActive(false);
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
