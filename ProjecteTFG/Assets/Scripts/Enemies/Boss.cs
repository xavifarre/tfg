using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : Enemy
{
    [Header("Boss stats")]
    //Fase
    public int fase = 0;

    //Vida fases
    public List<float> ratiosDamageFase = new List<float>();

    //Knockback
    protected Vector3 startKnockback;
    protected Vector3 endKnockback;
    protected float timeKnockback;

    [Header("Boss kill slowdown")]
    public float killSlowTime = 3f;
    public float killSlowScale = 0.2f;

    public Material disolveMaterialDie;

    public override void Die()
    {
        Globals.killCount += 1;
    }

    public override void Hit(Attack attack)
    {
        if (vulnerable)
        {
            GetDamage(attack.damage);
        }
    }

    protected override void Init()
    {

    }

    protected override void UpdateEnemy()
    {
       
    }


    protected virtual void KnockBack(float knockBack, float knockbackTime)
    {
        startKnockback = transform.position;
        endKnockback = startKnockback + (Vector3)player.lastDir * knockBack;
        timeKnockback = knockbackTime;
        tAction = 0;
    }

    protected virtual int CheckDamageFase()
    {
        for (int i = 0; i < ratiosDamageFase.Count; i++)
        {
            if (health > maxHealth * ratiosDamageFase[i])
            {
                return i;
            }
        }
        return -1;
    }
    protected IEnumerator IDie()
    {
        float t = 0;
        float shakeDuration = 3;

        StartCoroutine(IDieDisolve(shakeDuration));
        Vector3 diePosition = transform.position;

        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            transform.position = new Vector3(Random.Range(-100000, 100000) / 100000f, Random.Range(-100000, 100000) / 100000f).normalized * t / shakeDuration / 2 + diePosition;
            yield return new WaitForFixedUpdate();
        }
        SaveSystem.SaveGame();
        
        yield return new WaitForSeconds(6f);
        if (Globals.gameState == GameState.End)
        {
            SceneManager.LoadScene("Credits");
        }
        else
        {
            SceneManager.LoadScene("ToriiLevel");
        }

        Destroy(gameObject);
    }

    protected IEnumerator IDieDisolve(float shakeDuration)
    {
        float t = 0;
        spriteRenderer.material = disolveMaterialDie;
        GetComponentInChildren<ShadowCopySprite>().gameObject.SetActive(false);
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            spriteRenderer.material.SetFloat("_Fade", Mathf.Lerp(1, 0, t / shakeDuration));
            yield return null;
        }
    }

    protected void ResetLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Boss");
    }

    protected virtual void StartFase(int f)
    {
        fase = f;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    //if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerDash"))
    //    //{
    //    //    collision.gameObject.SendMessage("DashCrash", this);
    //    //}
    //}
}
