using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int framesActive = 10;
    public GameObject hitParticles;

    private GameObject player;
    private Collider2D col;
    private int frames;
    private bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.gameObject;
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PerformAttack(Vector2 lastDir)
    {
        int dir = player.GetComponent<Player>().GetDirection(lastDir);
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
        frames = 0;
        stop = false;
        while (frames < framesActive && !stop)
        {
            frames++;
            yield return null;
        }
        col.enabled = false;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {

        if(collider.gameObject.tag == "Enemy")
        {
            //Impacte depenent de la mida
            float impactOffset = collider.gameObject.GetComponent<Enemy>().size/2;
            Vector3 impactPoint = collider.transform.position + (transform.position - collider.transform.position).normalized * impactOffset;
            
            //Instanciar particules
            GameObject particles = Instantiate(hitParticles);
            particles.transform.position = impactPoint;

            //Crida a la funció de generació de cristalls
            player.SendMessage("GenerateShards", impactPoint);
        }
    }
}
