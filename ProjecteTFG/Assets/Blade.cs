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
    public int bladeId;

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

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();
        trail = transform.GetComponentInChildren<TrailRenderer>();
    }


    public void InitPosition()
    {
        angularSpeed = basicAngularSpeed = perserver.basicAngularSpeed;
        circleCenter = Vector3.zero;

        currentAngle = bladeId == 0 ? Mathf.PI / 2 : Mathf.PI + Mathf.PI / 2;
        startingAngle = currentAngle;
        currentRadius = basicRadius = perserver.basicRadius;
        motionDirection = 1;

        Vector3 circlePos = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * basicRadius;
        transform.localPosition = circleCenter + circlePos;
        defaultPosition = transform.localPosition;
    }

    private void CircleMovement()
    {

        lastPos = transform.position;
        previousAngle = currentAngle;
        currentAngle += angularSpeed * Time.deltaTime * motionDirection;
        Vector3 circlePos = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * currentRadius;
        Vector3 nextPosition = circleCenter + circlePos;
        SmoothTrail();
        transform.localPosition = nextPosition;
        
    }

    private void SmoothTrail()
    {

        int iterations = (int)angularSpeed / 10;
        for (int i = 1; i <= iterations; i++)
        {
            float angle = Mathf.Lerp(previousAngle, currentAngle, i / (iterations*1f) );
            Vector3 circlePos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * currentRadius;
            Vector3 nextPosition = perserver.transform.position + circleCenter + circlePos;
            trail.AddPosition(nextPosition);
        }
    }

    public void StartAbility(BladeAbility ability)
    {
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
            if (currentAbilityRoutine != null)
            {
                StopCoroutine(currentAbilityRoutine);
            }
            currentAbilityRoutine = IExpandingSpin(perserver.expandingSpinStats);
        }
        else if (ability == BladeAbility.UndodgeableSpin)
        {
            if (currentAbilityRoutine != null)
            {
                StopCoroutine(currentAbilityRoutine);
            }
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
                if(tNear > stats.iterationsForExpanding)
                {
                    perserver.ExpandingSpin();
                }
            }

            CircleMovement();
            t += Time.deltaTime;
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

        if(bladeId == 0)
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

        while (t < stats.duration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(previusRadius, stats.maxRadius, t / stats.duration);
            angularSpeed = Mathf.Lerp(previousSpeed, stats.maxAngularSpeed, t / stats.duration);
            CircleMovement();

            yield return null;
        }
        t %= stats.duration;
        while (t < stats.recallDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(stats.maxRadius, basicRadius, t / stats.recallDuration);
            angularSpeed = Mathf.Lerp(stats.maxAngularSpeed, basicAngularSpeed, t / stats.recallDuration);
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

        while (currentRadius != basicRadius)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(initialRadius, basicRadius, t / stats.recoverDuration);
            angularSpeed = Mathf.Lerp(initialSpeed, stats.angularMaxSpeed, t / stats.recoverDuration);
            CircleMovement();

            yield return null;
        }

        t %= stats.recoverDuration;

        angularSpeed = stats.angularMaxSpeed;
        while(t < stats.duration)
        {
            t += Time.deltaTime;
            CircleMovement();

            yield return null;
        }

        currentAngle = startingAngle;
        angularSpeed = 0;
        CircleMovement();

        if (bladeId == 0)
        {
            perserver.EndAbility(Perserver.Ability.UndodgeableSpin, stats.lagTime);
        }

        currentAbilityRoutine = null;
    }

    public IEnumerator IPowderDrop(Perserver._PowderDrop stats)
    {
        float t = 0;

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

        yield return new WaitForSeconds(stats.powderDelay);

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
                float radiusToBarrel = (perserver.transform.position - perserver.barrelPopPosition).magnitude;
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

            transform.localPosition = defaultPosition;
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

        if(bladeId == 0)
        {
            perserver.ThrowBarrel();
            perserver.EndAbility(Perserver.Ability.BarrelToss, stats.lagTime);
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

        float t = 0;

        Vector3 initialPosition = transform.localPosition;

        //Charge
        angularSpeed = 0;
       
        while(t < stats.chargeDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(basicRadius, basicRadius + stats.expandDistance, t / stats.chargeDuration);
            CircleMovement();
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
            if(movementValue.magnitude > Vector3.Distance(transform.position, destPos))
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
        while(t < stats.recoverDuration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(destPos - transform.parent.position, initialPosition, t / stats.recoverDuration);
            yield return null;
        }

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

        if(bladeId == 0)
        {
            perserver.StartHeal();
        }
        

        while (t < stats.duration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(basicRadius, stats.maxRadius, t / stats.duration);
            angularSpeed = Mathf.Lerp(stats.angularSpeed, stats.maxAngularSpeed, t / stats.duration);
            CircleMovement();

            yield return null;
        }

        if (bladeId == 0)
        {
            perserver.StopHeal();
        }

        t %= stats.duration;
        while (t < stats.recallDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(stats.maxRadius, basicRadius, t / stats.recallDuration);
            angularSpeed = Mathf.Lerp(stats.maxAngularSpeed, basicAngularSpeed, t / stats.recallDuration);
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
        if ((transform.localPosition - circleCenter).magnitude < currentRadius)
        {
            return direction * -1;
        }
        return direction;
    }

    public float GetKnockbackValue()
    {
        return Mathf.Clamp(angularSpeed / speedKnockbackFactor, 0, knockback);
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
