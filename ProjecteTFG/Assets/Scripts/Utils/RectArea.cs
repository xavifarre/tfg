using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectArea : MonoBehaviour
{
    public Vector2 spawnAreaSize = new Vector2(10f, 5f);

    public Vector3 RandomPoint()
    {
        return new Vector2(Random.Range(transform.position.x - spawnAreaSize.x / 2, transform.position.x + spawnAreaSize.x / 2), Random.Range(transform.position.y - spawnAreaSize.y / 2, transform.position.y + spawnAreaSize.y / 2));
    }

    private void OnDrawGizmosSelected()
    {
        if(!transform.parent || !transform.parent.GetComponent<Spawner>() || transform.parent.GetComponent<Spawner>().type == 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, (Vector3)spawnAreaSize + Vector3.forward);
        }
    }
}
