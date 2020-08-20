using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : Attack
{
    //Knockback factor
    public float speedKnockbackFactor = 3;

    //Basic stats
    private float basicRadius = 1f;
    private float basicAngularSpeed = 1f;
    private float basicAngle;
    public float inclination;
    public int bladeId;
    public float bladeRecoveryTime = 1;

    //Circle movement
    private float currentAngle;
    private float currentRadius;
    private float angularSpeed;
    private float startingAngle;
    [HideInInspector]
    public float motionDirection;

    //Center point
    private Vector3 circleCenter;

    //Default position
    private Vector3 defaultPosition;

    //Parent
    public Perserver perserver;

    //Player
    private Player player;

    //Trail
    private TrailRenderer trail;

    //Ability
    public enum BladeAbility { Spin, SpinHeal, ExpandingSpin, UndodgeableSpin, PowderDrop, BarrelPop, BarrelToss, BarrelDrop, DoubleSlash, Heal };
    public IEnumerator currentAbilityRoutine;

    //BarrelPop blade
    public bool poppingBlade;

    //Last position
    Vector3 lastPos;

    //PreviousAngle
    private float previousAngle;

    //RigidBody
    private Rigidbody2D rb;

    private SpriteRenderer spriteRenderer;
    //Layer
    private int defaultLayer;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        trail = transform.GetComponentInChildren<TrailRenderer>();
        defaultLayer = gameObject.layer;

    }

    public void InitPosition()
    {
        angularSpeed = basicAngularSpeed = perserver.basicAngularSpeed;
        circleCenter = Vector3.zero;

        basicAngle = bladeId == 0 ? 0 : Mathf.PI;
        currentAngle = basicAngle + Mathf.PI / 2;

        startingAngle = currentAngle;
        currentRadius = basicRadius = perserver.basicRadius;
        motionDirection = 1;

        Vector3 circlePos = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * basicRadius;
        transform.localPosition = circleCenter + circlePos;
        defaultPosition = transform.localPosition;

        RotateBlades();
    }


    private void CircleMovement()
    {
        lastPos = transform.position;
        previousAngle = currentAngle;
        currentAngle += angularSpeed * Time.deltaTime * motionDirection * perserver.GetBladeSpeedMultiplier();
        Vector3 circlePos = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)/ inclination) * currentRadius;
        Vector3 nextPosition = circleCenter + circlePos;

        RotateBlades();

        SmoothTrail();

        transform.localPosition = nextPosition;
    }   

    private void RotateBlades()
    {
        transform.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(transform.localPosition, Vector3.right));
    }

    public float GetCurrentAngle()
    {
        float angle = MathFunctions.Mod(Vector2.SignedAngle(transform.localPosition, Vector3.right) * Mathf.Deg2Rad + Mathf.PI / 2, 2*Mathf.PI);
        //Debug.Log("Blade " + bladeId + " angle " + (Vector2.SignedAngle(transform.localPosition, Vector3.right) * Mathf.Deg2Rad) + " angle2 " + angle + " pos " + transform.localPosition);
        return angle;
    }

    public float GetAngle(Vector2 pos)
    {
        float angle = MathFunctions.Mod(Vector2.SignedAngle(pos, Vector3.right) * Mathf.Deg2Rad + Mathf.PI / 2, 2*Mathf.PI);
        //Debug.Log(angle);
        return angle;
    }

    public float GetAngleToPlayer()
    {
        float angle = MathFunctions.Mod(Vector2.SignedAngle(player.transform.position - perserver.transform.position, Vector3.right) * Mathf.Deg2Rad + basicAngle, 2 * Mathf.PI);
        return angle;
    }

    private void BladeAngleLookAtPlayer()
    {
        float angleToPlayer = Vector2.SignedAngle(player.transform.position - perserver.transform.position, Vector3.right) * Mathf.Deg2Rad + basicAngle;
        float sinToPlayer = Mathf.Sin(angleToPlayer);
        float cosToPlayer = Mathf.Cos(angleToPlayer);
        //Debug.Log(sinToPlayer + " " + cosToPlayer + " " + angleToPlayer);
        Vector3 circlePos = new Vector2(sinToPlayer, cosToPlayer / inclination) * currentRadius;
        Vector3 nextPosition = circleCenter + circlePos;
        transform.localPosition = nextPosition;
    }

    private void SmoothTrail()
    {
        int iterations = (int)(angularSpeed * perserver.bladeSpeedMultiplierPerFase[perserver.fase]) / 10;
        for (int i = 1; i <= iterations; i++)
        {
            float angle = Mathf.Lerp(previousAngle, currentAngle, i / (iterations*1f) );
            Vector3 circlePos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)/ inclination) * currentRadius;
            Vector3 nextPosition = perserver.transform.position + circleCenter + circlePos;
            trail.AddPosition(nextPosition);
        }
    }


    public void StartAbility(BladeAbility ability)
    {
        ResetLayer();
        if (currentAbilityRoutine != null)
        {
            StopCoroutine(currentAbilityRoutine);
        }

        if (ability == BladeAbility.Spin)
        {
            currentAbilityRoutine = ISpin(perserver.spinStats);
        }
        else if (ability == BladeAbility.SpinHeal)
        {
            currentAbilityRoutine = ISpinHeal(perserver.spinHealStats);
        }
        else if (ability == BladeAbility.ExpandingSpin)
        {
            currentAbilityRoutine = IExpandingSpin(perserver.expandingSpinStats);
        }
        else if (ability == BladeAbility.UndodgeableSpin)
        {
            currentAbilityRoutine = IUndodgeableSpin(perserver.undodgeableSpinStats);
        }
        else if (ability == BladeAbility.PowderDrop)
        {
            currentAbilityRoutine = IPowderDrop(perserver.powderDropStats);
        }
        else if (ability == BladeAbility.BarrelPop)
        {
            currentAbilityRoutine = IBarrelPop(perserver.barrelPopStats);
        }
        else if (ability == BladeAbility.BarrelToss)
        {
            currentAbilityRoutine = IBarrelToss(perserver.barrelTossStats);
        }
        else if (ability == BladeAbility.BarrelDrop)
        {
            currentAbilityRoutine = IBarrelDrop(perserver.barrelDropStats);
        }
        else if (ability == BladeAbility.DoubleSlash)
        {
            currentAbilityRoutine = IDoubleSlash(perserver.doubleSlashStats);
        }
        else if (ability == BladeAbility.Heal)
        {

        }

        StartCoroutine(currentAbilityRoutine);
    }


    public IEnumerator ISpin(Perserver._Spin stats)
    {
        damage = stats.damage;

        float t = 0;

        while(t < stats.growDuration)
        {
            currentRadius = Mathf.Lerp(basicRadius, stats.radius, t / stats.growDuration);
            angularSpeed = Mathf.Lerp(basicAngularSpeed, stats.angularSpeed, t / stats.growDuration);

            CircleMovement();
            t += Time.deltaTime;
            yield return null;
        }

        t %= stats.growDuration;
        float tEv = 0, tNear = 0;
        while (t < stats.duration)
        {
            if(tEv >= stats.evaluationRate && perserver.CheckDistanceToPlayer() == 0)
            {
                tNear++;
                tEv %= stats.evaluationRate;
                if(tNear > stats.iterationsForExpanding)
                {
                    perserver.SpinTrigger();
                }
            }

            CircleMovement();
            t += Time.deltaTime;
            tEv += Time.deltaTime;
            yield return null;
        }

        t %= stats.duration;
        while (t < stats.growDuration)
        {
            currentRadius = Mathf.Lerp(stats.radius, basicRadius, t /stats.growDuration);
            angularSpeed = Mathf.Lerp(stats.angularSpeed, basicAngularSpeed, t / stats.growDuration);

            CircleMovement();
            t += Time.deltaTime;
            yield return null;
        }

        //Recover
        yield return new WaitForSeconds(perserver.bladeRecoverDelay);

        float previousAngle = GetCurrentAngle();
        currentRadius = basicRadius;
        t = 0;
        while (t < bladeRecoveryTime)
        {
            t += Time.deltaTime;
            currentAngle = Mathf.Lerp(previousAngle, GetAngle(defaultPosition), t / bladeRecoveryTime);
            CircleMovement();
            yield return null;
        }


        if (bladeId == 0)
        {
            perserver.EndAbility(Perserver.Ability.Spin, stats.lagTime);
        }

        currentAbilityRoutine = null;
    }

    public IEnumerator IExpandingSpin(Perserver._ExpandingSpin stats)
    {
        damage = stats.damage;

        float t = 0;

        float previusRadius = currentRadius;
        float previousSpeed = angularSpeed;

        while (t < stats.expandDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(previusRadius, stats.maxRadius, t / stats.expandDuration);
            angularSpeed = Mathf.Lerp(previousSpeed, stats.maxAngularSpeed, t / stats.expandDuration);
            CircleMovement();

            yield return null;
        }
        t %= stats.expandDuration;

        while(t < stats.duration[perserver.fase])
        {
            t += Time.deltaTime;
            CircleMovement();
            yield return null;
        }
        t %= stats.duration[perserver.fase];
        while (t < stats.recallDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(stats.maxRadius, basicRadius, t / stats.recallDuration);
            angularSpeed = Mathf.Lerp(stats.maxAngularSpeed, basicAngularSpeed, t / stats.recallDuration);
            CircleMovement();
            yield return null;
        }

        //Recover
        yield return new WaitForSeconds(perserver.bladeRecoverDelay);

        float previousAngle = GetCurrentAngle();
        currentRadius = basicRadius;

        t = 0;
        while (t < bladeRecoveryTime)
        {
            t += Time.deltaTime;
            currentAngle = Mathf.Lerp(previousAngle, GetAngle(defaultPosition), t / bladeRecoveryTime);
            CircleMovement();
            yield return null;
        }


        if (bladeId == 0)
        {
            perserver.EndAbility(Perserver.Ability.ExpandingSpin, stats.lagTime);
        }

        currentAbilityRoutine = null;
    }

    public IEnumerator IUndodgeableSpin(Perserver._UndodgeableSpin stats)
    {
        damage = stats.damage;

        float t = 0;
        float initialRadius = currentRadius;
        float initialSpeed = angularSpeed;

        while (currentRadius != stats.radius)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(initialRadius, stats.radius, t / stats.expandDuration);
            angularSpeed = Mathf.Lerp(initialSpeed, stats.angularMaxSpeed, t / stats.expandDuration);
            CircleMovement();

            yield return null;
        }

        stats.collider.Enable();

        t = 0;

        while (currentRadius != basicRadius)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(stats.radius, basicRadius, t / stats.recoverDuration);
            angularSpeed = Mathf.Lerp(stats.angularMaxSpeed, 0, t / stats.recoverDuration);
            CircleMovement();

            yield return null;
        }

        //Recover
        yield return new WaitForSeconds(perserver.bladeRecoverDelay);

        float previousAngle = GetCurrentAngle();
        currentRadius = basicRadius;

        t = 0;
        while (t < bladeRecoveryTime)
        {
            t += Time.deltaTime;
            currentAngle = Mathf.Lerp(previousAngle, GetAngle(defaultPosition), t / bladeRecoveryTime);
            CircleMovement();
            yield return null;
        }


        if (bladeId == 0)
        {
            perserver.EndAbility(Perserver.Ability.UndodgeableSpin, stats.lagTime);
        }

        currentAbilityRoutine = null;
    }

    public IEnumerator IPowderDrop(Perserver._PowderDrop stats)
    {
        float t = 0;

        ChangeLayerIgnore();

        angularSpeed = stats.spinSpeed;
        while (t < stats.spinDuration/2)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(basicRadius, stats.radius, t / (stats.spinDuration/2));
            CircleMovement();
            yield return null;
        }

        //Spawn powder
        if(bladeId == 0)
        {
            perserver.SpawnPowder();
        }

        t %= stats.spinDuration / 2;

        while (t < stats.spinDuration/2)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(stats.radius, basicRadius, t / (stats.spinDuration/2));
            CircleMovement();
            yield return null;
        }


        //Recover
        float previousAngle = GetCurrentAngle();
        currentRadius = basicRadius;

        t = 0;
        while (t < bladeRecoveryTime)
        {
            t += Time.deltaTime;
            currentAngle = Mathf.Lerp(previousAngle, GetAngle(defaultPosition), t / bladeRecoveryTime);
            CircleMovement();
            yield return null;
        }

        if (stats.powderDelay - t > 0)
        {
            yield return new WaitForSeconds(stats.powderDelay - t);
        }


        if (bladeId == 0)
        {
            perserver.EndAbility(Perserver.Ability.PowderDrop, stats.lagTime);
        }

        currentAbilityRoutine = null;
    }

    public IEnumerator IBarrelPop(Perserver._BarrelPop stats)
    {
        float t = 0;
        
        Vector3 startPos = transform.localPosition;

        Vector3 randomOffset = new Vector3(Random.value, Random.value, 0).normalized * stats.distanceToPlayer;

        poppingBlade = bladeId == 0 && motionDirection == -1 || bladeId == 1 && motionDirection == 1;

        if (!poppingBlade)
        {
            ChangeLayerIgnore();
        }

        while (t < stats.placementDuration)
        {
            t += Time.deltaTime;
            if (!poppingBlade)
            {
                transform.localPosition = Vector3.Lerp(startPos, player.transform.position - perserver.transform.position + randomOffset, t / stats.placementDuration);
            }
            else
            {
                currentRadius = Mathf.Lerp(basicRadius, stats.preparationRadius, t / stats.placementDuration);
                angularSpeed = Mathf.Lerp(basicAngularSpeed, stats.preparationSpeed, t / stats.placementDuration);
                CircleMovement();
            }
            yield return null;
        }

        if (!poppingBlade)
        {
            perserver.PlaceBarrel(transform.position);
        }

        t %= stats.placementDuration;

        if (poppingBlade)
        {
            float previousRadius = currentRadius;
            while (t < stats.popDuration)
            {
                t += Time.deltaTime;
                Vector2 vectorToBarrel = (perserver.transform.position - perserver.barrelPopPosition);
                float radiusToBarrel = new Vector2(vectorToBarrel.x, vectorToBarrel.y * inclination).magnitude;
                currentRadius = Mathf.Lerp(previousRadius, radiusToBarrel, t / stats.popDuration);
                angularSpeed = Mathf.Lerp(stats.preparationSpeed, stats.popSpeed, t / stats.popDuration);
                CircleMovement();
                yield return null;
            }
        }
        else
        {
            while (t < stats.placementDuration)
            {
                t += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(perserver.barrelPopPosition - perserver.transform.position, defaultPosition, t / stats.placementDuration);
                yield return null;
            }
        }

        t = 0;

        if (poppingBlade)
        {
            while (!perserver.barrelPopped)
            {
                CircleMovement();
                yield return null;
            }
            float previousRadius = currentRadius;
            while (t < stats.recallDuration)
            {
                t += Time.deltaTime;
                currentRadius = Mathf.Lerp(previousRadius, basicRadius, t / stats.recallDuration);
                angularSpeed = Mathf.Lerp(stats.recallSpeed, basicAngularSpeed, t / stats.recallDuration);
                CircleMovement();
                yield return null;
            }
        }
        else
        {
            ResetLayer();
        }

        //Recover
        yield return new WaitForSeconds(perserver.bladeRecoverDelay);

        float previousAngle = GetCurrentAngle();
        currentRadius = basicRadius;

        t = 0;
        while (t < bladeRecoveryTime)
        {
            t += Time.deltaTime;
            currentAngle = Mathf.Lerp(previousAngle, GetAngle(defaultPosition), t / bladeRecoveryTime);
            CircleMovement();
            yield return null;
        }

        if (bladeId == 0)
        {
            perserver.EndAbility(Perserver.Ability.BarrelPop, stats.lagTime);
        }

        currentAbilityRoutine = null;
    }

    public IEnumerator IBarrelToss(Perserver._BarrelToss stats)
    {
        float t = 0;

        while (t < stats.spinDuration)
        {
            angularSpeed = Mathf.Lerp(basicAngularSpeed, stats.spinSpeed, t / stats.spinDuration);
            t += Time.deltaTime;
            CircleMovement();

            yield return null;
        }
        transform.localPosition = defaultPosition;


        if (bladeId == 0)
        {
            perserver.ThrowBarrel();
            perserver.EndAbility(Perserver.Ability.BarrelToss, stats.lagTime);
        }

        //Recover
        yield return new WaitForSeconds(perserver.bladeRecoverDelay);

        float previousAngle = GetCurrentAngle();
        currentRadius = basicRadius;

        t = 0;
        while (t < bladeRecoveryTime)
        {
            t += Time.deltaTime;
            currentAngle = Mathf.Lerp(previousAngle, GetAngle(defaultPosition), t / bladeRecoveryTime);
            CircleMovement();
            yield return null;
        }

        currentAbilityRoutine = null;
    }

    public IEnumerator IBarrelDrop(Perserver._BarrelDrop stats)
    {
        float t = 0;

        angularSpeed = 0;
        while (t < stats.preparationDuration / 2)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(basicRadius, stats.preparationRadius, t / (stats.preparationDuration / 2));
            CircleMovement();
            yield return null;
        }

        t %= stats.preparationDuration / 2;

        while (t < stats.preparationDuration / 2)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(stats.preparationRadius, basicRadius, t / (stats.preparationDuration / 2));
            CircleMovement();
            yield return null;
        }


        if (bladeId == 0)
        {
            perserver.StartBarrelDrop();
        }

        currentAbilityRoutine = null;
    }

    public IEnumerator IDoubleSlash(Perserver._DoubleSlash stats)
    {
        damage = stats.damage;
        //Charge
        angularSpeed = 0;

        float t = 0;

        //Lerp angle to player
        float previousAngle = GetCurrentAngle();
        currentRadius = basicRadius;

        while (t < 0.5f)
        {
            t += Time.deltaTime;
            currentAngle = Mathf.Lerp(previousAngle, GetAngleToPlayer(), t / 0.5f);
            CircleMovement();

            yield return null;
        }

        while (t < stats.chargeDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(basicRadius, basicRadius + stats.expandDistance, t / stats.chargeDuration);
            BladeAngleLookAtPlayer();
            RotateBlades();
            yield return null;
        }

        t %= stats.chargeDuration;

        //Brief charge delay on one blade
        if (bladeId == 1)
        {
            while(t < stats.expandDelay)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        //Slash
        //Player predicted position
        Vector3 destPos = player.transform.position + player.movementValue * 2;
        Vector3 slashDir = (destPos - transform.position).normalized;
        Vector3 movementValue;
        bool reachedDest = false;

        while (!reachedDest)
        {
            movementValue = slashDir * stats.speed * Time.deltaTime;
            if(movementValue.magnitude >= Vector3.Distance(transform.position, destPos))
            {
                movementValue = destPos - transform.position;
                reachedDest = true;
            }
            transform.position = transform.position + movementValue;
            yield return null;
        }

        yield return new WaitForSeconds(stats.recoverDelay + (1 - bladeId) * stats.expandDelay);

        t = 0;


        //Blade recover
        while (t < stats.recoverDuration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(destPos - transform.parent.position, defaultPosition, t / stats.recoverDuration);
            RotateBlades();
            yield return null;
        }

        //Recover
        yield return new WaitForSeconds(perserver.bladeRecoverDelay);


        currentAngle = GetCurrentAngle();
        currentRadius = basicRadius;

        if (bladeId == 0)
        {
            perserver.EndAbility(Perserver.Ability.DoubleSlash, stats.lagTime);
        }

        currentAbilityRoutine = null;
    }


    public IEnumerator ISpinHeal(Perserver._SpinHeal stats)
    {
        damage = stats.damage;

        float t = 0;

        float previusRadius = currentRadius;
        float previousSpeed = angularSpeed;

        if (bladeId == 0)
        {
            perserver.StartHeal();
        }

        while (t < stats.expandDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(previusRadius, stats.maxRadius, t / stats.expandDuration);
            angularSpeed = Mathf.Lerp(previousSpeed, stats.maxAngularSpeed, t / stats.expandDuration);
            CircleMovement();

            yield return null;
        }
        t %= stats.expandDuration;
        while (t < stats.duration[perserver.fase])
        {
            t += Time.deltaTime;
            CircleMovement();
            yield return null;
        }

        if (bladeId == 0)
        {
            perserver.StopHeal();
        }

        t %= stats.duration[perserver.fase];
        while (t < stats.recallDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(stats.maxRadius, basicRadius, t / stats.recallDuration);
            angularSpeed = Mathf.Lerp(stats.maxAngularSpeed, basicAngularSpeed, t / stats.recallDuration);
            CircleMovement();
            yield return null;
        }

        //Recover
        yield return new WaitForSeconds(perserver.bladeRecoverDelay);

        float previousAngle = GetCurrentAngle();
        currentRadius = basicRadius;

        t = 0;
        while (t < bladeRecoveryTime)
        {
            t += Time.deltaTime;
            currentAngle = Mathf.Lerp(previousAngle, GetAngle(defaultPosition), t / bladeRecoveryTime);
            CircleMovement();
            yield return null;
        }

        if (bladeId == 0)
        {
            perserver.EndAbility(Perserver.Ability.SpinHeal, stats.lagTime);
        }

        currentAbilityRoutine = null;
    }


    public Vector3 GetBladeDirection()
    {
        return (transform.position - lastPos).normalized;
    }

    public Vector3 GetKnockbackDirectionFromCenter()
    {
        Vector3 direction = (transform.localPosition - circleCenter).normalized;
        return direction;
    }

    public float GetKnockbackValue()
    {
        if(perserver.currentAbility == Perserver.Ability.DoubleSlash)
        {
            return perserver.doubleSlashStats.knockback;
        }
        return Mathf.Clamp(angularSpeed * perserver.GetBladeSpeedMultiplier() / speedKnockbackFactor, 0, knockback);
    }

    private void ChangeLayerIgnore()
    {
        gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
    }

    private void ResetLayer()
    {
        gameObject.layer = defaultLayer;
    }

    public void DisableBlade()
    {
        StopAllCoroutines();
        enabled = false;
    }

    public void DieBlade()
    {
        trail.enabled = false;
        DieEffect dieEffect = GetComponent<DieEffect>();
        if (dieEffect)
        {
            dieEffect.TriggerDie();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(currentAbilityRoutine != null)
            {
                Vector3 knockbackDir = (GetBladeDirection() + GetKnockbackDirectionFromCenter()).normalized;
                Vector3 knockbackMotion = knockbackDir * GetKnockbackValue();
                collision.GetComponent<Player>().Hit(this, knockbackMotion);
            }
        }
    }
}
