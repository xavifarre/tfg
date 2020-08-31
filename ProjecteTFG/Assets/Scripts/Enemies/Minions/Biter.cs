using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biter : Minion
{
    [Header("Attack")]
    //Charge
    public float chargeRange = 3f;
    public float chargeTime = 0.5f;

    //Attack
    public float attackDuration = 1;
    public float attackDistance = 5f;

    private ParticleSystem smokeParticles;
    private Vector3 particlesStartPosition;

    private Collider2D colliderInstance;
    private Vector3 colliderStartOffset;

    public Collider2D attackCollider;

    protected override void Init()
    {
        smokeParticles = GetComponentInChildren<ParticleSystem>();
        particlesStartPosition = smokeParticles.transform.localPosition;

        colliderInstance = GetComponent<Collider2D>();
        colliderStartOffset = colliderInstance.offset;

        soundController.PlaySound("biter_spawn",0.5f);

        base.Init();
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
            StartIdle();
        }
    }

    private void Charge()
    {
        state = MinionState.Charge;
        animator.SetTrigger("Charge");
        StartCoroutine(ICharge());
        
    }

    private void Attack()
    {
        startActionPoint = realPos;
        endActionPoint = realPos + (player.transform.position - startActionPoint).normalized * attackDistance;
        tAction = 0;
        state = MinionState.Attack;
        animator.SetTrigger("Bite");
        UpdateSpriteFlip();
        soundController.PlaySound("biter_attack0" + (Random.Range(0, 2) + 1));
    }

    protected override void KnockBack(float knockBack)
    {
        base.KnockBack(knockBack);
        attackCollider.enabled = false;
    }

    private IEnumerator ICharge()
    {
        yield return new WaitForSeconds(chargeTime);
        if (state == MinionState.Charge)
        {
            Attack();
        }
    }

    public override void GetDamage(int damage)
    {
        soundController.PlaySound("biter_hit");
        base.GetDamage(damage);
    }

    protected override void UpdateSpriteFlip()
    {
        base.UpdateSpriteFlip();
        //if (spriteRenderer.flipX)
        //{
        //    colliderInstance.offset = new Vector2(-colliderStartOffset.x, colliderStartOffset.y);
        //    smokeParticles.transform.localPosition = new Vector3(-particlesStartPosition.x, particlesStartPosition.y, particlesStartPosition.z);
        //}
        //else
        //{
        //    colliderInstance.offset = new Vector2(colliderStartOffset.x, colliderStartOffset.y);
        //    smokeParticles.transform.localPosition = particlesStartPosition;
        //}
    }

    public override void Die()
    {
        base.Die();
        attackCollider.enabled = false;
        smokeParticles.Stop();
        soundController.PlaySound("biter_die");
    }
}
