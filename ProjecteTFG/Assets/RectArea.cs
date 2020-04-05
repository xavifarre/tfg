using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectArea : MonoBehaviour
{
    public Vector2 spawnAreaSize = new Vector2(10f, 5f);

    private void OnDrawGizmosSelected()
    {
        if(transform.parent.GetComponent<Spawner>().type == 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, (Vector3)spawnAreaSize + Vector3.forward);
        }
    }
}
