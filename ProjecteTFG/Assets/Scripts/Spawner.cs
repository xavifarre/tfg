using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Vector2 spawnArea1 = new Vector2(-9f, -5f);
    public Vector2 spawnArea2 = new Vector2(9f, 5f);

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
            Vector2 pos = new Vector2(Random.Range(spawnArea1.x, spawnArea2.x),Random.Range(spawnArea1.y, spawnArea2.y));

            GameObject spawnedObject = Instantiate(obj,pos, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f),Vector3.forward));

            Shard shard = obj.GetComponent<Shard>();

            if (shard)
            {
                player.activeShards.Add(shard);
            }

            t = 0;
        }
	}
}
