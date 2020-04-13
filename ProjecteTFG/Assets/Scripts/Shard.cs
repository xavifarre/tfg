using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shard : MonoBehaviour {

    //Basic
    public float speed = 5f;
    public float rotSpeed = 10f;
    public float maxActiveTime = 10f;
    public float maxSpeed = 50f;

    //Range
    public float maxHomingDistance = 3;

    //Damage
    public float damageVelRatio= 10f;
    private int damage;

    //Velocity
    public float acceleration = 5f;
    public float growRate = 0.5f;
    private float startVel = 0;
    private float tVel = 0;

    bool moving = false;
    bool recalling = false;

    Player player;
    Rigidbody2D rb;

    //Origin
    Vector3 originPos;
    //Target dest
    Vector3[] targetPos = new Vector3[1];
    int iTarget = 0;
    Vector3 lastPos;

    //Last position
    Vector3 nextPos;

    //Rotation
    Vector3 rotationValue;
    public float rotationMultiplier = 10f;
    public float minSpeedRotation = 0.3f;

    //Float values
    private float tFloat = 0;

    private float tMov = 0;

    private SpriteRenderer spriteRenderer;

    private GameObject enemies;

    //Sprite
    public SpriteRenderer sprite;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        StartCoroutine(ICheckDeath());
        gameObject.layer = LayerMask.NameToLayer("CrystalInactive"); //ShardInactive layer

        originPos = transform.position;
        if (targetPos[0] == Vector3.zero)
        {
            targetPos[0] = transform.position;
        }

        enemies = GameObject.Find("Enemies");

        //Randomitzar rotació
        rotationValue = new Vector3(Random.Range(1, 11), Random.Range(1, 11), Random.Range(1, 11)).normalized;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
	
	void FixedUpdate () {

        if (moving)
        {
            UpdateMovement();
        }
        else
        {
            //FloatAround();
        }

        UpdateRotation();
	}

    //private void LateUpdate()
    //{
    //    spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(this.spriteRenderer.bounds.min).y * -1;
    //}

    public void Recall(Vector3 target)
    {

        Ray ray = new Ray(transform.position, target - transform.position);
        List<Vector3> enemyTargets = new List<Vector3>();
        foreach(Transform tr in enemies.transform)
        {
            float distanceToRay = MathFunctions.DistanceToLine(ray, tr.position);
           
            if (distanceToRay  < maxHomingDistance && MathFunctions.ProjectPointOnLineSegmentSide(transform.position, target, tr.position) == 0)
            {
                enemyTargets.Add(tr.position);
            }
        }

        Vector3[] points;
        if (enemyTargets.Count > 0)
        {
            int randomEnemy = Random.Range(0, enemyTargets.Count);
            if (ShardEnemyManager.EnemyIsMarked(randomEnemy) && ShardEnemyManager.enemiesMarked.Count < enemyTargets.Count)
            {
                randomEnemy = Random.Range(0, enemyTargets.Count);
            }
            Vector3 enemyTarget = enemyTargets[randomEnemy];
            Debug.Log(randomEnemy);
            ShardEnemyManager.MarkEnemy(randomEnemy);
            Vector3[] curvePoints = new Vector3[] { transform.position, MathFunctions.CalculateMiddlePoint(transform.position, target, 0.5f, enemyTarget), target };
            points = MathFunctions.MakeSmoothCurve(curvePoints, 3);
            iTarget = 1;
        }
        else
        {
            points = new Vector3[1] { target };
        }
        lastPos = transform.position;
        MoveShards(points);
        recalling = true;
        gameObject.layer = LayerMask.NameToLayer("CrystalActive"); //ShardActive layer
    }


    public void MoveShards(Vector3[] target)
    {
        tMov = 0;
        moving = true;
        targetPos = target;
        originPos = transform.position;
    }

    public void MoveShards(Vector3 target)
    {
        tMov = 0;
        moving = true;
        targetPos[0] = target;
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

        float dur;
        
        if (recalling)
        {

            if ((lastPos - targetPos[iTarget]).magnitude < (lastPos - transform.position).magnitude  && (transform.position - targetPos[iTarget]).magnitude < (lastPos - transform.position).magnitude)
            {
                iTarget++;
            }
            lastPos = transform.position;
            if(iTarget >= targetPos.Length)
            {
                DestroyShard();
            }
            else
            {
                targetPos[targetPos.Length - 1] = player.transform.position - Vector3.up * 0.5f;

                tVel = tVel + acceleration * Time.deltaTime;
                float vel = Mathf.Clamp(startVel + Mathf.Pow(1 + growRate, tVel), 0, maxSpeed);
                rb.velocity = (targetPos[iTarget] - transform.position).normalized * vel;

                float rDamage = Mathf.Clamp(vel / damageVelRatio, 1, 5);
                damage = (int)(player.baseShardDamage + (player.maxShardDamage - player.baseShardDamage) * (rDamage - 1) / 4);
            }

        }
        else
        {
            dur = 0.5f;
            nextPos = MathFunctions.EaseOutExp(tMov, originPos, targetPos[0], dur, 3);
            rb.MovePosition(nextPos);

            if (tMov >= dur)
            {
                Stop();
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
        transform.position = new Vector3(transform.position.x,targetPos[0].y + ySinus, transform.position.z);
    }

    public void Stop()
    {
        targetPos[0] = transform.position;
        iTarget = 0;
        tMov = 0;
        moving = false;
        gameObject.layer = LayerMask.NameToLayer("CrystalInactive"); //ShardInactive layer
        rb.velocity = Vector2.zero;
    }

    public void DestroyShard()
    {
        Stop();
        player.GetComponent<Player>().activeShards.Remove(this);
        sprite.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        spriteRenderer.enabled = false;
        Destroy(gameObject,1f);
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log(collision.gameObject.layer);
            //DestroyShard();
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            //collision.gameObject.SendMessage("Damage");
            //DestroyShard();
            collision.GetComponent<Enemy>().GetDamage(damage);
        }
        else
        {
            Stop();
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (recalling)
        {
            for (int i = 0; i < targetPos.Length-1; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(targetPos[i], targetPos[i + 1]);
            }
        }
    }
}
