using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Vector2 spawnAreaSize = new Vector2(10f, 5f);

    public float spawnTime = 1;

    float t = 0;

    public GameObject obj;
    public Player player;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        if(t>= spawnTime)
        {
            Vector2 pos = new Vector2(Random.Range(transform.position.x - spawnAreaSize.x/2, transform.position.x + spawnAreaSize.x / 2),Random.Range(transform.position.y - spawnAreaSize.y / 2, transform.position.y + spawnAreaSize.y / 2));

            GameObject spawnedObject = Instantiate(obj,pos, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f),Vector3.forward));

            Shard shard = obj.GetComponent<Shard>();

            if (shard)
            {
                player.activeShards.Add(shard);
            }

            t = 0;
        }
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, (Vector3)spawnAreaSize + Vector3.forward);
    }
}
