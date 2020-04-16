using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    //Basic stats
    public float basicRadius = 1f;
    public float basicAngularSpeed = 1f;
    public int bladeId;

    //Circle movement
    private float currentAngle;
    private float currentRadius;
    private float angularSpeed;

    //Center point
    private Vector3 circleCenter;

    //Default position
    private Vector3 defaultPosition;

    //Parent
    private Perserver perserver;

    //Ability
    public enum BladeAbility { Spin, SpinHeal, ExpandingSpin, UndodgeableSpin, PowderDrop, BarrelPop, BarrelToss, BarrelDrop, DoubleSlash, Heal };

    // Start is called before the first frame update
    void Start()
    {
        perserver = transform.parent.GetComponent<Perserver>();
        angularSpeed = basicAngularSpeed;
        circleCenter = Vector3.zero;

        currentAngle = bladeId == 0 ? 0 : Mathf.PI;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitPosition()
    {
        Vector3 circlePos = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * basicRadius;
        transform.localPosition = circleCenter + circlePos;
        defaultPosition = transform.localPosition;
    }

    public void StartAbility(BladeAbility ability)
    {
        if(ability == BladeAbility.Spin)
        {
            StartCoroutine(ISpin(perserver.spinStats));
        }
        else if (ability == BladeAbility.SpinHeal)
        {

        }
        else if (ability == BladeAbility.ExpandingSpin)
        {
            StartCoroutine(IExpandingSpin(perserver.expandingSpinStats));
        }
        else if (ability == BladeAbility.UndodgeableSpin)
        {

        }
        else if (ability == BladeAbility.PowderDrop)
        {

        }
        else if (ability == BladeAbility.BarrelPop)
        {

        }
        else if (ability == BladeAbility.BarrelToss)
        {

        }
        else if (ability == BladeAbility.DoubleSlash)
        {

        }
        else if (ability == BladeAbility.Heal)
        {

        }
        
    }

    private void CircleMovement()
    {
        currentAngle += angularSpeed * Time.deltaTime;
        Vector3 circlePos = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * currentRadius;
        transform.localPosition = circleCenter + circlePos;
    }

    public IEnumerator ISpin(Perserver._Spin stats)
    {
        float t = 0;
        
        while(t < stats.duration)
        {
            if(t <= stats.growDuration)
            {
                currentRadius = Mathf.Lerp(basicRadius, stats.radius, t / stats.growDuration);
                angularSpeed = Mathf.Lerp(basicAngularSpeed,stats.angularSpeed, t / stats.growDuration);
            }
            else if(t >= stats.duration - stats.growDuration)
            {
                currentRadius = Mathf.Lerp(-stats.radius, -basicRadius, t - stats.duration + stats.growDuration / stats.growDuration) * -1;
                angularSpeed = Mathf.Lerp( -stats.angularSpeed, -basicAngularSpeed, t - stats.duration + stats.growDuration / stats.growDuration) * -1;
            }
            else
            {
                currentRadius = stats.radius;
                angularSpeed = stats.angularSpeed;
            }

            CircleMovement();
            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator IExpandingSpin(Perserver._ExpandingSpin stats)
    {
        float t = 0;

        while (t < stats.duration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(basicRadius, stats.maxRadius, t / stats.duration);
            angularSpeed = Mathf.Lerp(stats.angularSpeed, stats.maxAngularSpeed, t / stats.duration);
            CircleMovement();

            yield return null;
        }
        t %= stats.duration;
        while (t < stats.recallDuration)
        {
            t += Time.deltaTime;
            currentRadius = Mathf.Lerp(-stats.maxRadius,  -basicRadius, t / stats.recallDuration) * -1;
            angularSpeed = Mathf.Lerp(-stats.maxAngularSpeed, -basicAngularSpeed, t / stats.recallDuration) * -1;
            CircleMovement();
            yield return null;
        }
    }
}
