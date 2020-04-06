﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerGroup : MonoBehaviour, ISpawner
{

    public enum SpawnType { All, RandomWeighted, FullRandom };
    public SpawnType spawnType;
    public int spawnNumber = 1;

    [Header("Random")]
    public float antiRepeatWheight = 0;
    private List<ISpawner> spawnerGroup;
    private ISpawner hitSpawner;

    public List<float> probability = new List<float>();

    private List<int> usedArray;

    // Start is called before the first frame update
    void Start()
    {
        usedArray = new List<int>();
        spawnerGroup = new List<ISpawner>();
        
        foreach(Transform child in transform)
        {
            if(child.tag == "SpawnLoop")
            {
                spawnerGroup.Add(child.GetComponent<ISpawner>());
            }
            if(child.tag == "SpawnHit")
            {
                hitSpawner = child.GetComponent<ISpawner>();
            }
        }

        foreach(ISpawner s in spawnerGroup)
        {
            if (probability.Count < spawnerGroup.Count)
            {
                probability.Add(1.0f / spawnerGroup.Count);
            }
        }
    }

    public void StartSpawning()
    {
        if(spawnType == SpawnType.All)
        {
            foreach (ISpawner sp in spawnerGroup)
            {
                sp.StartSpawning();
            }
        }
        else if (spawnType == SpawnType.RandomWeighted)
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                int randIndex = MathFunctions.RandomProbability(probability);
                while (usedArray.Contains(randIndex) && Random.Range(0, 100) / 100f < antiRepeatWheight)
                {
                    randIndex = MathFunctions.RandomProbability(probability);
                }

                if (!usedArray.Contains(randIndex))
                {
                    usedArray.Add(randIndex);
                }

                if (usedArray.Count == spawnerGroup.Count)
                {
                    usedArray.Clear();
                }

                spawnerGroup[randIndex].StartSpawning();
            }
        }
        else if (spawnType == SpawnType.FullRandom)
        {
            for(int i = 0; i < spawnNumber; i++)
            {
                int randIndex = MathFunctions.RandomProbability(probability);
                spawnerGroup[randIndex].StartSpawning();
            }
        }
    }

    public void StopSpawning()
    {
        foreach (ISpawner sp in spawnerGroup)
        {
            sp.StopSpawning();
        }
        hitSpawner.StopSpawning();

        usedArray.Clear();
    }

    public void StartSpawningHit()
    {
        hitSpawner.StartSpawning();
    }

}