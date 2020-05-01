using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMelee : Attack
{
    public float timeActive = 0.2f;
    public GameObject hitParticles;

    private Player player;
    private Collider2D col;
    private float t;
    private bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<Player>();
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PerformAttack(Vector2 lastDir)
    {
        int dir = MathFunctions.GetDirection(lastDir);
        col.enabled = false;
        transform.eulerAngles = new Vector3(0, 0, dir * 90);
        
        StopAllCoroutines();
        StartCoroutine(ColliderTime());
    }

    public void StopAttack()
    {
        stop = true;
    }

    IEnumerator ColliderTime()
    {
        col.enabled = true;
        t = 0;
        stop = false;
        while (t < timeActive && !stop)
        {
            t+=Time.deltaTime;
            yield return null;
        }
        col.enabled = false;
    }


    private void Impact(Collider2D collider)
    {
        float impactOffset = 1;
        if (collider.tag == "Enemy")
        {
            //Impacte depenent de la mida
            impactOffset = collider.gameObject.GetComponent<Enemy>().size / 2;
        }
        Vector3 impactPoint = collider.transform.position + (transform.position - collider.transform.position).normalized * impactOffset;

        //Instanciar particules
        GameObject particles = Instantiate(hitParticles);
        particles.transform.position = impactPoint;

        //Crida a la funció de generació de cristalls
        player.GenerateShards(impactPoint);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {

        if(collider.gameObject.tag == "Enemy")
        {
            Impact(collider);
            //Envia el hit al enemic
            collider.GetComponent<Enemy>().Hit(this);
        }
        else if(collider.gameObject.tag == "Barrel")
        {
            BarrelProximity barrel = collider.GetComponent<BarrelProximity>();
            if (barrel.IsHitable())
            {
                Impact(collider);
                //Envia el hit al barril
                barrel.Hit(this);
            }
        }
    }
}
