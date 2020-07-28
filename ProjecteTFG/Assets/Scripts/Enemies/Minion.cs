using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Enemy, IFallableObject
{
    
    [HideInInspector]
    public MinionState state;

    //Action
    protected Vector3 startActionPoint;
    protected Vector3 endActionPoint;

    [Header("Minion stats")]
    //Idle
    public float idleTime = 0.3f;

    //Fall
    public float fallTime = 1f;

    public ParticleSystem spawnParticles;

    [Header("Knockback")]
    //KnockBack
    public float knockBackResistance = 1;
    public float knockBackDuration = 0.2f;


    protected override void Init()
    {
        if (spawnParticles)
        {
            ParticleSystem p = Instantiate(spawnParticles);
            p.transform.position = realPos;
            Destroy(p.gameObject, p.main.duration);
        }
        UpdateSpriteFlip();
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
            if (tAction > idleTime)
            {
                StartMove();
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
            StartIdle();
        }
    }

    protected virtual void StartMove()
    {
        tAction = 0;
        state = MinionState.Move;
    }

    protected virtual void StartIdle()
    {
        state = MinionState.Idle;
        tAction = 0;
    }

    public override void Hit(Attack attack)
    {
        if (vulnerable)
        {
            GetDamage(attack.damage);
            if(state != MinionState.Dead)
            {
                KnockBack(attack.knockback);
            }
        }
    }

    protected virtual void KnockBack(float knockBack)
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
        ChangeLayerIgnore();
        animator.enabled = false;

        FadeShadow();

        Globals.killCount += 1;

        DieEffect dieEffect = GetComponent<DieEffect>();
        if (dieEffect)
        {
            dieEffect.TriggerDie();
        }
        else
        {
            Destroy(gameObject);
        }
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

    protected virtual void UpdateSpriteFlip()
    {
        if ((player.transform.position.x - realPos.x) > 0)
        {
            //spriteRenderer.flipX = true;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            //spriteRenderer.flipX = false;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
