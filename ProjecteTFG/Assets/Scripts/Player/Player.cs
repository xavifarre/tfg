using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IState, IFallableObject {

    //Movement speed
    public float speed;

    //Health state
    public float health;
    [HideInInspector]
    public bool dead = false;

    private Rigidbody2D rb2d;
    private Vector3 realPos;

    //Actions
    public State state;
    float tAction;

    //LastDirection
    [HideInInspector]
    public Vector2 lastDir;
    [HideInInspector]
    public Vector3 movementValue;

    //Dash and other movement positions
    [HideInInspector]
    public Vector2 startPoint;
    [HideInInspector]
    public Vector2 destPoint;
    Vector3 lastSafePosition;

    //Dash
    public float dashDistance = 5;
    public float dashTime = 0.2f;
    public float dashCd = 3;
    bool dashReady = true;

    //Invulnerable
    bool invulnerable = false;
    public float invulnerableTime = 0.2f;

    //Attack
    GameObject attackCollider;
    int consecutiveAttacks = 0;
    public float attackTime = 0.4f;
    public float attackMovementTime= 0.2f;
    public float attackMovementDistance = 0.5f;

    //InputQueue
    string queuedInput = "";

    //KnockBack
    public float knockBackTime = 20;
    public float knockBackDistance = 2;

    //Fall
    public float fallTime = 1f;
    private Vector3 fallSpawnPosition;


    //Shards
    public GameObject shardObject;
    public int baseShardDamage = 1;
    public int maxShardDamage  = 5;
    public Vector2Int minMaxShards = new Vector2Int(3,5);
    public List<Shard> activeShards;
    public Vector2 shardRange = new Vector2(300,500);
    [HideInInspector]
    public bool recallReady = true;

    //Animations
    private Animator animator;

    //UI
    public Text textActionCD;

    //Sprite renderer
    private SpriteRenderer spriteRenderer;

    //Game manager
    private GameManager gm;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        attackCollider = transform.Find("Attack").gameObject;
        lastDir = new Vector2(0, -1);
        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        gm = FindObjectOfType<GameManager>();

        realPos = transform.position;
    }

    private void FixedUpdate()
    {
        //Update actions
        if (state == State.Idle)
        {
            //Move
            realPos = realPos + movementValue * Time.deltaTime * speed;
            Move(realPos);
            //transform.position = transform.position + movement * Time.deltaTime * speed;
        }

        if (state == State.Dash)
        {
            UpdateDashPosition();
        }

        if (state == State.Attack)
        {
            UpdateAttack();
        }
        else
        {
            consecutiveAttacks = 0;
        }

        if (state == State.KnockBack)
        {
            UpdateKnockBack();
        }
    }

    private void Update()
    {
        //Check Queue attack input
        if (state == State.Attack)
        {
            QueueAttackInput();
        }

        CheckRecallInput();

        //Check inputs
        if (state == State.Idle)
        {
            CheckMoveInputs();
            CheckAttackInput();

            //Store last safe position (used for spawning when player falls)
            lastSafePosition = transform.position;
        }

        if (CanDash())
        {
            CheckDashInput();
        }        

        if (Input.GetKeyDown("u"))
        {
            Die();
        }
        if (Input.GetKeyDown("i"))
        {
            Fall(transform.position);
        }

        UpdateAnimations();
    }

    ////Modificar ordre de layer segons la posició y
    //private void LateUpdate()
    //{
    //    spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(this.spriteRenderer.bounds.min).y * -1;
    //}


    void CheckMoveInputs()
    {
        //Get the movement value
        movementValue = GetMovementInput();

        //Store last direction
        StoreLastDir(movementValue);
    }

    Vector3 GetMovementInput()
    {
        //Define the stick deadzone
        float deadzone = 0.50f;

        //Store the current horizontal input in the float moveHorizontal.
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        //Store the current vertical input in the float moveVertical.
        float moveVertical = Input.GetAxisRaw("Vertical");

        //Use the two store floats to create a new Vector2 variable movement.
        Vector3 movement = new Vector2(moveHorizontal, moveVertical);

        //Check if the magnitude is lesser than the deadzone.
        //If yes discard it, else normalize it.
        if (movement.magnitude < deadzone)
        {
            movement = Vector2.zero;
        }
        else
        {
            movement = movement.normalized * ((movement.magnitude - deadzone) / (1 - deadzone));
        }

        if (movement.magnitude > 1)
        {
            movement = movement.normalized;
        }

        return movement;
    }

    //Store the last direction if movement is not 0
    void StoreLastDir(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            lastDir = movement.normalized;
        }
    }

    //Move to position
    public void Move(Vector3 position)
    {
        realPos = position;
        PixelPerfectMovement.Move(position, rb2d);
    }

    void CheckAttackInput()
    {
        if (Input.GetButtonDown("Attack"))
        {
            MeleeAttack();
        }
    }

    void CheckRecallInput()
    {
        if (Input.GetButtonDown("Recall") && recallReady)
        {
            RecallShards();
        }
    }

    void CheckDashInput()
    {
        if (Input.GetButtonDown("Dash") && dashReady)
        {
            Dash();
        }
    }

    void QueueAttackInput()
    {
        if (Input.GetButtonDown("Attack"))
        {
            queuedInput = "Attack";
        }

        Vector3 movement = GetMovementInput();
        StoreLastDir(movement);
    }

    bool CanDash()
    {
        return state == State.Idle || state == State.Attack;
    }

    void Dash()
    {
        state = State.Dash;
        invulnerable = true;
        startPoint = realPos;
        destPoint = Vector2.ClampMagnitude(lastDir * 10000, dashDistance) + startPoint;
        tAction = 0;
        gameObject.layer = LayerMask.NameToLayer("PlayerDash"); //Canvi de layer per a travessar enemics simples
        StartCoroutine(IDashCooldown());
        attackCollider.GetComponent<AttackMelee>().StopAttack();
        animator.SetTrigger("Dash");

        queuedInput = "";
    }

    void UpdateDashPosition()
    {
         //-----Desacceleració exponencial----
        tAction += Time.deltaTime;
        //Move(MathFunctions.EaseOutExp(tAction, startPoint, destPoint, dashTime, 5));

        Vector3 tNextPos = MathFunctions.EaseOutExp(tAction, startPoint, destPoint, dashTime, 5);
        float tDistance = (tNextPos - MathFunctions.EaseOutExp(tAction-Time.deltaTime, startPoint, destPoint, dashTime, 5)).magnitude;
        Vector3 tDir = (destPoint - startPoint).normalized;
        //Debug.Log(tNextPos + " " + transform.position + " " + tDistance + " " + tDir);

        Move(realPos + tDir * tDistance);

        //float vDash = 2f;
        //float vt = MathFunctions.EaseOutExp(tAction, vDash, 0, dashTime, 3);

        //Debug.Log(vt);
        //Move(transform.position + (Vector3)lastDir.normalized * vt );


        //Invulnerabilitat 2/3 parts del dash
        if (tAction >= dashTime * 2 / 3)
        {
            invulnerable = false;
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        // Fi dash?
        if (tAction >= dashTime)
        {
            EndDash();
        }
    }

    void EndDash()
    {
        state = State.Idle;
    }

    //Atac bàsic a melee. S'alterna dreta i esquerra
    void MeleeAttack()
    {
        invulnerable = true;
        attackCollider.SendMessage("PerformAttack", lastDir);
        consecutiveAttacks++;

        if(consecutiveAttacks%2 == 0)
        {
            animator.SetTrigger("Slash02");
        }
        else
        {
            animator.SetTrigger("Slash01");
        }

        //Mini-dash on attack
        tAction = 0;
        startPoint = transform.position;
        destPoint = Vector2.ClampMagnitude(lastDir * 10000, attackMovementDistance) + startPoint;
        state = State.Attack;
    }

    //Moviment al atacar
    void UpdateAttack()
    {
        tAction += Time.deltaTime;

        if (tAction < attackMovementTime)
        {
            Move(MathFunctions.EaseOutExp(tAction, startPoint, destPoint, attackMovementTime, 5));
        }
        else
        {
            EndAttack();
        }

        if (tAction >= attackTime)
        {
            EndAttackSequence();
        }
    }

    void EndAttack()
    {
        invulnerable = false;
        if (queuedInput == "Attack" && consecutiveAttacks < 3)
        {
            MeleeAttack();
            queuedInput = "";
        }
    }

    void EndAttackSequence()
    {
        state = State.Idle;
        consecutiveAttacks = 0;
    }


    void RecallShards()
    {
        ShardEnemyManager.ResetEnemies();

        int i = 0;
        foreach (Shard shard in activeShards)
        {
            shard.TriggerRecall(transform.position - Vector3.up * 0.5f, i * 0.02f);
            i++;
        }

        recallReady = true;

    }

    public void GenerateShards(Vector3 pos)
    {
        int nShards = Random.Range(minMaxShards.x, minMaxShards.y+1);

        List<Vector3> spawnList = new List<Vector3>();
        List<Vector3> destList= new List<Vector3>();

        int[] spawnOrder = new int[nShards];

        for (int i = 0; i < nShards; i++){

            float randomX = Random.Range(-800, 800) / 1000f * (lastDir.y + 0.2f);
            float randomY = Random.Range(-800, 800) / 1000f * (lastDir.x + 0.2f);

            Vector3 spawnPos = new Vector3(pos.x + randomX, pos.y + randomY, 0);

            Vector3 impactDir = (spawnPos - transform.position).normalized;

            //impactDir = Quaternion.AngleAxis(30 * (consecutiveAttacks % 2 == 0 ? 1 : -1), Vector3.forward) * impactDir;

            Vector3 destDir = (new Vector3(spawnPos.x + lastDir.x/2 + impactDir.x * 3 + randomX * 4, spawnPos.y + lastDir.y/2 + impactDir.y*3 + randomY * 4, 0) - spawnPos).normalized;

            Vector3 destPos = pos + destDir * Random.Range(shardRange.x, shardRange.y) /100f;

            spawnList.Add(spawnPos);
            destList.Add(destPos);
            
            //Debug.Log(spawnPos + destPos.ToString());
        }

        StartCoroutine(SpawnGeneratedShards(spawnList,destList,spawnOrder));
    }

    IEnumerator SpawnGeneratedShards(List<Vector3> spawnList, List<Vector3> destList, int[] spawnOrder)
    {
        for(int i=0; i<spawnList.Count; i++)
        {
            GameObject spawnedObject = Instantiate(shardObject, spawnList[i], Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.forward));

            Shard shard = spawnedObject.GetComponent<Shard>();

            activeShards.Add(shard);
  
            shard.MoveShards(destList[i]);

            yield return new WaitForSeconds(0.01f);
        }
        
    }


    public void Hit(Enemy enemy)
    {
        if (!invulnerable)
        {
            GetDamage(enemy.damage);
            if (state != State.Dead)
            {
                KnockBack(enemy.transform.position, enemy.knockBackValue);
                DamageInvulnerability(enemy.damage);
            }
        }
    }

    public void Hit(Attack attack)
    {
        if (!invulnerable)
        {
            GetDamage(attack.damage);
            if (state != State.Dead)
            {
                KnockBack(attack.transform.position, attack.knockback);
                DamageInvulnerability(attack.damage);
            }
        }
    }

    public void Hit(Blade blade, Vector3 knockbackMotion)
    {
        if (!invulnerable)
        {
            GetDamage(blade.damage);
            if (state != State.Dead)
            {
                KnockBack(knockbackMotion);
                DamageInvulnerability(blade.damage);
            }
        }
    }

    public void DashCrash(Enemy enemy)
    {
        GetDamage(enemy.damage);
        KnockBack(enemy.transform.position, enemy.knockBackValue);
    }

    public void GetDamage(int damage)
    {
        //Stop all actions
        //state = State.Idle;

        //Reset player layer
        gameObject.layer = LayerMask.NameToLayer("Player");

        //Take damage
        health -= damage;

        if (HasDied())
        {
            Die();
        }

        gm.tLastHit = 0;
    }

    public void Die()
    {
        state = State.Dead;
        Debug.Log("DEAD");

        gameObject.layer = LayerMask.NameToLayer("IgnoreAll");

        animator.SetTrigger("Die");
    }

    public bool HasDied()
    {
        return health <= 0;
    }

    public void DamageInvulnerability(int damage)
    {
        invulnerable = true;
        if(damage > 0)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        StartCoroutine(IInvulnerabilityDamage());
    }

    public void KnockBack(Vector3 pusherPos, float value)
    {
        Vector3 direction = (transform.position - pusherPos).normalized;

        startPoint = transform.position;
        destPoint = Vector2.ClampMagnitude(direction * 10000, knockBackDistance*value) + startPoint;

        animator.SetTrigger("Hit");

        state = State.KnockBack;
        tAction = 0;
    }


    public void KnockBack(Vector3 knockbackMotion)
    {
        startPoint = transform.position;
        destPoint = knockbackMotion + (Vector3)startPoint;

        animator.SetTrigger("Hit");

        state = State.KnockBack;
        tAction = 0;
    }

    private void EndKnockBack()
    {
        state = State.Idle;
    }

    private void UpdateKnockBack()
    {
        tAction += Time.deltaTime;

        if (tAction < knockBackTime)
        {
            Move(MathFunctions.EaseOutExp(tAction, startPoint, destPoint, knockBackTime, 5));
        }
        else
        {
            EndKnockBack();
        }
    }

    public void Fall(Vector3 fallPosition)
    {
        state = State.Fall;
        animator.SetTrigger("Fall");

        StartCoroutine(FallableObject.IFallAnimation(fallPosition, gameObject, fallTime));
        
    }

    public void EndFall()
    {
        transform.Find("FeetCollider").gameObject.SetActive(true);
        transform.localScale = Vector2.one;
        realPos = lastSafePosition  -(Vector3)lastDir * 1;
        state = State.Idle;
        GetDamage(1);
        DamageInvulnerability(1);
    }

    //Actualitzar els paràmetres d'animació
    private void UpdateAnimations()
    {
        //Actualitzar la ultima direcció
        animator.SetInteger("Direction", MathFunctions.GetDirection(lastDir));

        //Si està en moviment activar la layer de moviment
        animator.SetLayerWeight(1, GetMovementInput().magnitude == 0 ? 0 : 1);

        //Si està realitzant una acció activar la layer d'accions
        animator.SetLayerWeight(2, state == 0 ? 0 : 1);      
    }


    /***********************************
                GETTERS
     ***********************************/

    public State GetState()
    {
        return state;
    }

    /***********************************
                CORUTINES
     ***********************************/

    IEnumerator IDashCooldown()
    {
        dashReady = false;
        float tCooldownDash = 0;
        while(tCooldownDash < dashCd)
        {
            yield return new WaitForSeconds(0.1f);
            tCooldownDash += 0.1f;
            tCooldownDash = (float)System.Math.Round(tCooldownDash,2);
            textActionCD.text = "Dash CD: " + System.Math.Round((dashCd - tCooldownDash),2);
        }
            
        dashReady = true;
    }

    IEnumerator IInvulnerabilityDamage()
    {
        float t = 0;
        while(t < invulnerableTime)
        {
            t+=Time.deltaTime;
            yield return null;
        }
        invulnerable = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        realPos = transform.position;
    }
}
