using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shard : MonoBehaviour {


    public float speed = 5f;
    public float rotSpeed = 10f;
    public float acceleration = 5f;
    public float maxActiveTime = 10f;

    bool moving = false;
    bool recalling = false;
    float aSpeed;

    GameObject player;
    Rigidbody2D rb;

    //Origin
    Vector3 originPos;
    //Target dest
    Vector3 targetPos;

    //Last position
    Vector3 nextPos;

    //Rotation
    Vector3 rotationValue;
    public float rotationMultiplier = 10f;
    public float minSpeedRotation = 0.3f;

    //Float values
    private float maxFloatDistance = 2f;
    private float tFloat = 0;

    private float tMov = 0;

    //Sprite
    public SpriteRenderer sprite;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(ICheckDeath());
        gameObject.layer = 9; //ShardInactive layer

        originPos = transform.position;
        if (targetPos == Vector3.zero)
        {
            targetPos = transform.position;
        }

        //Randomitzar rotació
        rotationValue = new Vector3(Random.Range(1, 11), Random.Range(1, 11), Random.Range(1, 11)).normalized;
    }
	
	void FixedUpdate () {

        if (moving)
        {
            UpdateMovement();
        }
        else
        {
            FloatAround();
        }

        UpdateRotation();
	}
    public void Recall(Vector3 target)
    {
        MoveShards(target);
        recalling = true;
        gameObject.layer = 10; //ShardActive layer
    }

    public void MoveShards(Vector3 target)
    {
        tMov = 0;
        moving = true;
        targetPos = target;
        originPos = transform.position;
    }

    private void UpdateMovement()
    {
        //MRUA
        //Vector2 dir = (targetPos - transform.position).normalized;
        //aSpeed += Time.deltaTime * acceleration;
        //rb.MovePosition(rb.position + dir * Time.deltaTime * (speed + aSpeed));

        //
        //if (Vector3.Distance(targetPos, transform.position) < 0.1)
        //{
        //    Stop();
        //}

        //Exponecial
        //A CONSIDERAR: ACTUALMENT TOTS ELS CRISTALLS TRIGUEN EL MATEIX TEMPS A FER EL RECORREGUT INDEPENDENMENT DE LA DISTÀNCIA,
        //SERIA INTERESSANT FER QUE ELS MES PROPERS TRIGUESSIN MENYS, ES A DIR, QUE LA SEVA ACCELERACIÓ SIGUÉS LA MATEIXA
        float dur;
        
        if (recalling)
        {
            dur = 0.8f;
            targetPos = player.transform.position;
            transform.right = targetPos - transform.position;
            nextPos = MathFunctions.EaseInExp(tMov, originPos, targetPos, dur, 3);
            rb.MovePosition(nextPos);
        }
        else
        {
            dur = 0.5f;
            nextPos = MathFunctions.EaseOutExp(tMov, originPos, targetPos, dur, 3);
            rb.MovePosition(nextPos);
        }
        
        if (tMov >= dur)
        {
            Stop();
            if (recalling)
            {
                transform.position = player.transform.position;

                DestroyShard();
            }
        }
        tMov += Time.deltaTime;
    }

    //Actualitza la rotació dels fragments tenint en compte la velocitat
    private void UpdateRotation()
    {
        float velocity = GetVelocity().magnitude;
        if (!moving)
        {
            velocity = 0;
        }
        sprite.transform.Rotate(rotationValue * (velocity+minSpeedRotation) * rotationMultiplier);
    }

    //Fa flotar el fragment en forma de funció sinusoidal
    //A = amplitud, w = freqüencia angular, tFloat = comptador de temps
    private void FloatAround()
    {
        float A = 0.1f, w = 0.8f;
        tFloat += Time.deltaTime;
        float ySinus = A * Mathf.Sin(w*tFloat*Mathf.PI);
        transform.position = new Vector3(transform.position.x,targetPos.y + ySinus, transform.position.z);
    }

    public void Stop()
    {
        targetPos = transform.position;
        tMov = 0;
        moving = false;
        gameObject.layer = 9; //ShardInactive layer
        rb.velocity = Vector2.zero;
    }

    public void DestroyShard()
    {
        Stop();
        Debug.Log(player.GetComponent<Player>());
        player.GetComponent<Player>().activeShards.Remove(this);
        sprite.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(IDestroyShardOnTrailEnd());
        //Destroy(gameObject);
    }

    public Vector3 GetVelocity()
    {
        int fixedFrames = (int)Mathf.Round(1 / Time.fixedDeltaTime);
        Vector3 trackedVelocity = (nextPos - transform.position) * fixedFrames;
        return trackedVelocity;
    }

    //Timer per a destruir el fragment si ha passat el temps de vida màxim i no s'està movent
    private IEnumerator ICheckDeath()
    {
        yield return new WaitForSeconds(maxActiveTime);
        if (!moving)
        {
            DestroyShard();
        }
    }

    private IEnumerator IDestroyShardOnTrailEnd()
    {
        TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
        while (tr.positionCount > 0)
        {
            //tr.SetPosition(0, targetPos);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log(collision.gameObject.layer);
            DestroyShard();
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            //collision.gameObject.SendMessage("Damage");
            DestroyShard();
        }
        else
        {
            Stop();
        }

    }

}
