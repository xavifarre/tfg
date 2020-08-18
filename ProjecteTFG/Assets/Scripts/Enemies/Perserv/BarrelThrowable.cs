using UnityEngine;
using System.Collections;

public class BarrelThrowable: Barrel
{

    public float randomRangeExplosion;

    private Vector3 destPos;
    private float speed;
    private Vector3 direction;
    private float movementValue;


    // Use this for initialization
    void Start()
    {
        Init();
        animator.SetTrigger("RollThrow");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == destPos)
        {
            if (!exploded)
            {
                Explode();
            }
        }

        Vector3 movementVector = direction * speed * Time.deltaTime;
        movementValue -= movementVector.magnitude;

        if(movementValue < 0)
        {
            transform.position = destPos;
        }
        else
        {
            transform.position += movementVector;
        }
    }

    public void ThrowToPosition(float throwSpeed, Vector3 dest)
    {
        direction = (dest - transform.position).normalized;
        destPos = dest + direction * Random.Range(-randomRangeExplosion, randomRangeExplosion);
        movementValue = (dest - transform.position).magnitude;
        speed = throwSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explode();
    }
}
