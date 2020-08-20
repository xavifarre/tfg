﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IState, IFallableObject {

    //Movement speed
    public float speed;

    //Health state
    public float health;
    private float maxHealth;
    
    [HideInInspector]
    public bool dead = false;

    private Rigidbody2D rb2d;
    private Vector3 realPos;

    //Actions
    public State state;
    private float tAction;

    //LastDirection
    [HideInInspector]
    public Vector2 lastDir;
    [HideInInspector]
    public Vector3 movementValue;
    public Vector3 speedModifier = new Vector3(1,1,1);

    //Dash and other movement positions
    [HideInInspector]
    public Vector2 startPoint;
    [HideInInspector]
    public Vector2 destPoint;
    private Vector3 lastSafePosition;

    [Header("Dash")]
    //Dash
    public float dashDistance = 5;
    public float dashTime = 0.2f;
    public float dashCd = 2;
    public float dashMaxConsecutive = 3;
    public float dashMaxTimeConsecutives = 0.5f;
    private float dashConsecutiveCounter;
    private float dashLastTimestamp = -1;

    bool dashReady = true;

    public DashParticles dashLineParticles;
    //public ParticleSystem dashParticles;
    public TrailRenderer dashTrail;

    [Header("Attack")]
    //Attack
    public float attackTime = 0.4f;
    public float attackMovementTime = 0.2f;
    public float attackMovementDistance = 0.5f;
    GameObject attackCollider;
    int consecutiveAttacks = 0;


    //InputQueue
    private string queuedInput = "";

    [Header("Knockback")]
    //KnockBack
    public float knockBackTime = 20;
    public float knockBackDistance = 2;

    [Header("Fall")]
    //Fall
    public float fallTime = 1f;
    private Vector3 fallSpawnPosition;

    [Header("Shards")]
    //Shards
    public GameObject shardObject;
    public int baseShardDamage = 1;
    public int maxShardDamage  = 5;
    public Vector2Int minMaxShards = new Vector2Int(3,5);
    [HideInInspector]
    public List<Shard> activeShards;
    public float recallHealMultiplier = 1;
    public float recallHealMultiplierFromMinions = 0.5f;
    public float maxHealFromMinions = 10;
    [HideInInspector]
    public float healFromMinionsCounter = 0;
    public Vector2 shardRange = new Vector2(300,500);
    [HideInInspector]
    public bool recallReady = true;
    public ParticlePlayer recallParticles;
    public ParticleSystem shardHealParticles;


    //Invulnerable
    private bool invulnerable = false;
    [Header("Hit")]
    public float invulnerableTime = 0.2f;
    public float invulnerableBlinkTime = 0.1f;
    //Hit
    public Color hitColor = Color.red;
    public float hitColorDuration = 0.05f;
    public Material hitMaterial;
    private IEnumerator hitRoutine;
    public ParticleSystem hitParticles;

    //Animations
    private Animator animator;

    [Header("UI")]
    //UI
    public Text textActionCD;

    //Sprite renderer
    private SpriteRenderer spriteRenderer;

    //Game manager
    private GameManager gm;

    //Default material
    protected Material defaultMaterial;

    [Header("Animators")]
    public Material baseMaterial;
    public Material glowMaterial;
    public bool swordPicked;

    [Header("Audio")]
    public SoundController soundController; 
    public SoundController voiceController; 

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        attackCollider = transform.Find("Attack").gameObject;
        lastDir = new Vector2(0, -1);
        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();


        if (Globals.gameState >= GameState.SwordPicked || swordPicked)
        {
            swordPicked = true;
            ChangeSpriteToDefault();
        }
        else
        {
            ChangeSpriteToSwordless();
        }


        gm = FindObjectOfType<GameManager>();

        realPos = transform.position;
        maxHealth = health;
        HealthBar.Initialize((int)maxHealth,(int)health);
    }

    private void FixedUpdate()
    {
        //Update actions
        if (state == State.Idle)
        {
            //Move
            Vector3 mov = new Vector3(movementValue.x * speedModifier.x, movementValue.y * speedModifier.y, 1);
            realPos = realPos + mov * Time.deltaTime * speed;
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

        rb2d.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (!gm.inputsBlocked && !gm.gamePaused)
        {
            if (swordPicked)
            {
                //Check Queue attack input
                if (state == State.Attack)
                {
                    QueueAttackInput();
                }

                CheckRecallInput();
            }

            //Check inputs
            if (state == State.Idle)
            {
                CheckMoveInputs();
                if (swordPicked)
                {
                    CheckAttackInput();
                }
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
        else
        {
            movementValue = Vector3.zero;
        }
        
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
        return (state == State.Idle || state == State.Attack) && swordPicked;
    }

    void Dash()
    {
        state = State.Dash;
        invulnerable = true;
        startPoint = realPos;
        destPoint = Vector2.ClampMagnitude(lastDir * 10000, dashDistance) + startPoint;
        tAction = 0;
        ChangeLayerDash(); //Canvi de layer per a travessar enemics simples

        attackCollider.GetComponent<AttackMelee>().StopAttack();
        animator.SetTrigger("Dash");
        queuedInput = "";


        DashParticles particlesInstance = Instantiate(dashLineParticles, transform);
        particlesInstance.PlayParticles((destPoint - startPoint).normalized);
        dashTrail.emitting = true;

        //Consecutive dashes
        if (dashLastTimestamp == -1 || Time.time - dashLastTimestamp < dashMaxTimeConsecutives)
        {
            dashConsecutiveCounter++;
            if (dashConsecutiveCounter >= dashMaxConsecutive)
            {
                StartCoroutine(IDashCooldown());
            }
        }
        else
        {
            dashConsecutiveCounter = 1;
        }
       

        dashLastTimestamp = Time.time;

        if (Random.value < 0.7f)
        {
            voiceController.PlaySound("player_voice_dash0" + Random.Range(1, 3));
        }

        //Play sound
        soundController.PlaySound("player_dash");
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
            ResetLayer();
        }

        // Fi dash?
        if (tAction >= dashTime)
        {
            EndDash();
        }
    }

    void EndDash()
    {
        dashTrail.emitting = false;
        state = State.Idle;
    }

    //Atac bàsic a melee. S'alterna dreta i esquerra
    void MeleeAttack()
    {
        invulnerable = false;
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

        //Play sound
        soundController.PlaySound("player_slash0" + Random.Range(1, 4));

        if(Random.value < 1f)
        {
            if (consecutiveAttacks == 1)
            {
                voiceController.PlaySound("player_attack0" + Random.Range(1, 3));
            }
            else if (consecutiveAttacks == 2)
            {
                voiceController.PlaySound("player_attack0" + Random.Range(3, 5));
            }
            else
            {
                voiceController.PlaySound("player_attack05");
            }
        }
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
            shard.TriggerRecall(transform.position + Vector3.up * 0.5f, i * 0.02f);
            i++;
        }
        if(activeShards.Count > 0)
        {
            recallParticles.PlayParticles();
            healFromMinionsCounter = 0;
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
            }
        }
    }
    public void HitByTime(int damage)
    {
        if (!invulnerable)
        {
            GetDamage(damage, false);
        }
    }

    public void DashCrash(Enemy enemy)
    {
        GetDamage(enemy.damage);
        KnockBack(enemy.transform.position, enemy.knockBackValue);
    }

    public void GetDamage(int damage, bool setInvulnerable = true)
    {

        //Reset player layer
        ResetLayer();
        dashTrail.emitting = false;
        if (damage > 0)
        {
            health -= damage;
            HealthBar.UpdateBar((int)health);
            hitParticles.Play();
            DamageTick();
            ScreenManager.instance.ShowHitScreen();
            //gm.SlowDownGame(0, 0.5f);
            //gm.Shake(0.1f, 1f);
            voiceController.PlaySound("player_damage0" + Random.Range(1, 3));

            if (HasDied())
            {
                Die();
            }
            else if(setInvulnerable)
            {
                DamageInvulnerability();
            }

            
            ShowDamage(damage);
            Globals.damageReceivedCount += damage;
        }

        gm.tLastHit = 0;
    }

    protected void ShowDamage(int damage)
    {
        PopupTextController.CreatePopupTextDamageSelf(damage.ToString(), realPos);
    }

    public void Die()
    {
        state = State.Dead;
        //gm.DisableEnemies();
        gm.StopGame();
        ScreenManager.instance.ShowDeathScreen();
        
        ChangeLayerIgnore();
        DisableAllColliders();

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("Die");

        voiceController.PlaySound("player_die");

        Globals.deathCount += 1;
        GameManager.instance.ResumeGame();

    }

    public bool HasDied()
    {
        return health <= 0;
    }

    public void DamageInvulnerability()
    {

        invulnerable = true;
        StartCoroutine(IInvulnerabilityDamage());       
    }

    protected void DamageTick()
    {
        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }
        hitRoutine = IDamageTick();
        StartCoroutine(hitRoutine);
    }

    public void KnockBack(Vector3 pusherPos, float value)
    {
        Vector3 direction = (transform.position - pusherPos).normalized;

        startPoint = transform.position;
        destPoint = Vector2.ClampMagnitude(direction * 10000, knockBackDistance*value) + startPoint;

        UpdateKnockbackAnim(direction);

        state = State.KnockBack;
        tAction = 0;
    }


    public void KnockBack(Vector3 knockbackMotion)
    {
        startPoint = transform.position;
        destPoint = knockbackMotion + (Vector3)startPoint;

        UpdateKnockbackAnim(knockbackMotion);

        state = State.KnockBack;
        tAction = 0;
    }

    public void UpdateKnockbackAnim(Vector2 direction)
    {
        lastDir = direction.x >= 0 ? new Vector2(-1, 0) : new Vector2(1, 0);

        animator.SetTrigger("Hit");
        animator.SetInteger("Direction", MathFunctions.GetDirection(lastDir));
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
        Move(realPos);
        state = State.Idle;
        GetDamage(5);
    }

    public void ShardPicked(Shard shard)
    {
        if(shard.accumulatedHeal > 0)
        {
            ShardHeal(shard.accumulatedHeal);
        }
        Globals.crystalCount += 1;
    }

    public void ShardHeal(float accumulatedHeal)
    {
        health += accumulatedHeal;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        HealthBar.UpdateBar((int)health);
        shardHealParticles.Play();
    }

    public void PickSword()
    {
        ChangeSpriteToDefault();
        swordPicked = true;
        Globals.gameState = GameState.SwordPicked;
    }

    //Actualitzar els paràmetres d'animació
    private void UpdateAnimations()
    {
        //Actualitzar la ultima direcció
        animator.SetInteger("Direction", MathFunctions.GetDirection(lastDir));

        if (swordPicked)
        {
            //Si està en moviment activar la layer de moviment
            animator.SetLayerWeight(1, GetMovementInput().magnitude == 0 ? 0 : 1);

            //Si està realitzant una acció activar la layer d'accions
            animator.SetLayerWeight(2, state == 0 ? 0 : 1);
        }
        else
        {
            //Si està en moviment activar la layer de moviment sense espasa
            animator.SetLayerWeight(4, GetMovementInput().magnitude == 0 ? 0 : 1);
        }
    }

    private void ResetLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void ChangeLayerIgnoreEnemies()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerIgnoreEnemies");
    }

    private void ChangeLayerDash()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerDash");
    }

    protected void ChangeLayerIgnore()
    {
        gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
    }

    public void ChangeSpriteToSwordless()
    {
        spriteRenderer.material = baseMaterial;
        defaultMaterial = baseMaterial;
        animator.SetLayerWeight(3, 1);

    }

    public void ChangeSpriteToDefault()
    {
        spriteRenderer.material = glowMaterial;
        defaultMaterial = glowMaterial;
        animator.SetLayerWeight(3, 0);
        animator.SetLayerWeight(4, 0);
    }

    protected void DisableAllColliders()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Collider2D collider = transform.GetChild(i).GetComponent<Collider2D>();
            if (collider)
            {
                collider.gameObject.SetActive(false);
            }
        }
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
        while (tCooldownDash < dashCd)
        {
            yield return null;
            tCooldownDash += Time.deltaTime;
            //tCooldownDash = (float)System.Math.Round(tCooldownDash, 2);
            //textActionCD.text = "Dash CD: " + System.Math.Round((dashCd - tCooldownDash), 2);
        }

        dashReady = true;
    }

    IEnumerator IInvulnerabilityDamage()
    {
        float t = 0, tBlink = 0;
        ChangeLayerIgnoreEnemies();
        while (t < invulnerableTime)
        {
            t+=Time.unscaledDeltaTime;
            tBlink += Time.unscaledDeltaTime;
            if(tBlink > invulnerableBlinkTime)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Abs(spriteRenderer.color.a - 1));
                tBlink = 0;
            }
            yield return null;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        ResetLayer();
        invulnerable = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        realPos = transform.position;
    }

    protected IEnumerator IDamageTick()
    {
        hitMaterial.color = hitColor;
        spriteRenderer.material = hitMaterial;
        yield return new WaitForSecondsRealtime(hitColorDuration);
        spriteRenderer.material = defaultMaterial;
        hitRoutine = null;
    }

}
