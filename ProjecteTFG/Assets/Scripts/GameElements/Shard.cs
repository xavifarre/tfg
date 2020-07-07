using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shard : Attack {

    //Basic
    public float speed = 5f;
    public float rotSpeed = 10f;
    public float maxActiveTime = 10f;
    public float maxSpeed = 50f;

    //Range
    public float maxHomingDistance = 3;

    //Damage
    public float damageVelRatio= 10f;

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
    Vector3 targetPos;
    List<Vector3> targetPoints = new List<Vector3>();
    int iTarget = 0;
    Vector3 lastPos;
    int nCurves = 0;

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

    //Trail
    private TrailRenderer trail;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        StartCoroutine(ICheckDeath());
        gameObject.layer = LayerMask.NameToLayer("CrystalInactive"); //ShardInactive layer

        originPos = transform.position;
        if (targetPoints.Count == 0)
        {
            targetPoints.Add(transform.position);
        }

        enemies = GameObject.Find("Enemies");

        //Randomitzar rotació
        rotationValue = new Vector3(Random.Range(1, 11), Random.Range(1, 11), Random.Range(1, 11)).normalized;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        trail = GetComponentInChildren<TrailRenderer>();
    }
	
	void Update () {

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

    //private void LateUpdate()
    //{
    //    spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(this.spriteRenderer.bounds.min).y * -1;
    //}

    public void TriggerRecall(Vector3 target, float timeOffset)
    {
        StartCoroutine(ITriggerRecall(target, timeOffset));
    }

    public void Recall(Vector3 target)
    {
        Ray ray = new Ray(transform.position, target - transform.position);
        List<Vector3> enemyTargets = new List<Vector3>();
        foreach(Transform tr in enemies.transform)
        {
            if (tr.gameObject.activeSelf)
            {
                float distanceToRay = MathFunctions.DistanceToLine(ray, tr.position);

                if (distanceToRay < maxHomingDistance && MathFunctions.ProjectPointOnLineSegmentSide(transform.position, target, tr.position) == 0)
                {
                    enemyTargets.Add(tr.position);
                }

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
            ShardEnemyManager.MarkEnemy(randomEnemy);

            //Calcular la curva
            Vector3[] curvePoints = new Vector3[] { transform.position, MathFunctions.CalculateMiddlePoint(transform.position, target, 0.5f, enemyTarget), target };
            points = MathFunctions.MakeSmoothCurve(curvePoints, 5);
            
        }
        else
        {
            points = NewCurve(transform.position, Vector3.zero, target);
        }
        iTarget = 1;
        MoveShards(points);
        lastPos = transform.position;
        recalling = true;
        gameObject.layer = LayerMask.NameToLayer("CrystalActive"); //ShardActive layer
    }


    public void MoveShards(Vector3[] target)
    {
        tMov = 0;
        moving = true;
        targetPoints.Clear();
        targetPoints.AddRange(target);
        targetPos = target[target.Length - 1];
        originPos = transform.position;
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
        //Vector2 dir = (targetPoints - transform.position).normalized;
        //aSpeed += Time.deltaTime * acceleration;
        //rb.MovePosition(rb.position + dir * Time.deltaTime * (speed + aSpeed));

        //
        //if (Vector3.Distance(targetPoints, transform.position) < 0.1)
        //{
        //    Stop();
        //}

        float dur;
        
        if (recalling)
        {
            targetPos = player.transform.position + Vector3.up * 0.5f;
            lastPos = transform.position;

            tVel = tVel + acceleration * Time.deltaTime;
            float vel = Mathf.Clamp(startVel + Mathf.Pow(1 + growRate, tVel), 0, maxSpeed);
            //rb.velocity = (targetPoints[iTarget] - transform.position).normalized * vel;

            float rDamage = Mathf.Clamp(vel / damageVelRatio, 1, 5);
            damage = (int)(player.baseShardDamage + (player.maxShardDamage - player.baseShardDamage) * (rDamage - 1) / 4);


            //Move along spline
            Vector3 lastPoint = transform.position;
            float movementValue = vel * Time.deltaTime;
            bool movementCompleted = false;
            while (!movementCompleted)
            {
                //Si ha arribat al fi del spline
                if (iTarget >= targetPoints.Count - 1)
                {
                    //Comprovem distancia amb la posició destí. Si es molt petita aproximem el moviment la destí
                    if (Vector3.Distance(lastPoint, targetPos) > 0.2f)
                    {
                        Vector3[] newCurve = NewCurve(lastPoint, targetPoints[iTarget - 3], targetPos);
                        targetPoints.AddRange(newCurve);
                        iTarget++;
                        nCurves++;
                    }
                    else
                    {
                        movementCompleted = true;
                        lastPoint = targetPos;
                        DestroyShard();
                    }
                }
                else
                {
                    if (movementValue - (targetPoints[iTarget] - lastPoint).magnitude <= 0)
                    {
                        movementCompleted = true;
                        lastPoint = lastPoint + (targetPoints[iTarget] - lastPoint).normalized * movementValue;

                    }
                    else
                    {
                        movementValue -= (targetPoints[iTarget] - lastPoint).magnitude;
                        lastPoint = targetPoints[iTarget];
                        trail.AddPosition(lastPoint);
                        iTarget++;
                    }
                }
            }

            rb.MovePosition(lastPoint);
        }
        else
        {
            dur = 0.5f;
            nextPos = MathFunctions.EaseOutExp(tMov, originPos, targetPos, dur, 3);
            rb.MovePosition(nextPos);

            if (tMov >= dur)
            {
                Stop();
            }
        }

        tMov += Time.deltaTime;
    }

    //Calcular la curva de nou
    private Vector3[] NewCurve(Vector3 lastPoint, Vector3 previousPoint, Vector3 targetPoint)
    {
        Vector3 middlePoint = MathFunctions.VectorMiddlePoint(lastPoint, targetPoint);
        Vector3 curvePoint;
        if (previousPoint != Vector3.zero)
        {
            Vector3 perpendicular = MathFunctions.PerpendicularVector(targetPoint - lastPoint, Vector3.Angle(targetPoint - lastPoint, lastPoint - previousPoint) / 180 * Vector3.Distance(targetPoint, lastPoint), MathFunctions.AngleDir(lastPoint - previousPoint, targetPoint - lastPoint) > 0 ? 0 : 1);
            curvePoint = middlePoint + perpendicular;
        }
        else
        {
            curvePoint = middlePoint;
        }
        
        Vector3[] curvePoints = new Vector3[] { lastPoint, MathFunctions.CalculateMiddlePoint(lastPoint, targetPoint, 0.5f, curvePoint), targetPoint };
        return MathFunctions.MakeSmoothCurve(curvePoints, 3);
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
        transform.position = new Vector3(transform.position.x,targetPoints[0].y + ySinus, transform.position.z);
    }

    public void Stop()
    {
        targetPoints[0] = transform.position;
        iTarget = 0;
        tMov = 0;
        moving = false;
        gameObject.layer = LayerMask.NameToLayer("CrystalInactive"); //ShardInactive layer
        rb.velocity = Vector2.zero;
    }

    public void DestroyShard()
    {
        Stop();
        player.activeShards.Remove(this);
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

    private IEnumerator ITriggerRecall(Vector3 target, float timeOffset)
    {
        yield return new WaitForSeconds(timeOffset);
        Recall(target);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        else if (collision.gameObject.tag == "Barrel")
        {
            BarrelProximity barrel = collision.GetComponent<BarrelProximity>();
            if (barrel.IsHitable())
            {
                barrel.Hit(this);
            }
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
            for (int i = 0; i < targetPoints.Count - 1; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(targetPoints[i], targetPoints[i + 1]);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPoints[i], 0.1f);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPoints[targetPoints.Count - 1], 0.1f);
        }
    }
}
