using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceDestroyer : Attack
{
    public void ReleaseCrystals(int dir, Destroyer._HorizontalPierce stats)
    {
        StartCoroutine(IPierce(dir, stats));
    }

    private IEnumerator IPierce(int dir, Destroyer._HorizontalPierce stats)
    {
        yield return new WaitForSeconds(stats.crystalSpawnDelay);

        for (int i = 0; i < stats.nCrystals; i++)
        {
            CrystalDestroyer crystal = Instantiate(stats.crystalObject);
            crystal.damage = stats.crystalDamage;
            crystal.transform.position = transform.position + (Vector3.right * (Random.Range(0, stats.pirceRange) + stats.pierceOfsset) * dir);
            crystal.CrystalPierce(dir, stats, transform.position);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Player>().Hit(this);
        }
    }
}
