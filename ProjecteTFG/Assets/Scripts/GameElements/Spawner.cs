using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, ISpawner
{

    public GameObject obj;
    public GameObject spawnContainer;
    public float spawnInterval = 1;
    public int spawnBurst = 1;
    public int maxLoops = 1;

    public enum Type {Areas, Points}
    public Type type;

    private List<RectArea> areas;

    public enum Order { Sequential, RandomNoRepeat, FullRandom}
    public Order order;
    public bool mirror = false;

    private List<Vector2> points;
    private int iSeq = 0;
    private List<int> iUsed;


    [Header("Minimum Distances")]
    public float minimumDistanceToPlayer = 0;
    public float minimumDistanceToOther = 0;

    private float t = 0;
    private bool spawning = false;
    private int spawnLoops = 0;
    private Player player;
    private Summoner summoner;


    // Use this for initialization
    void Start ()
    {
        player = FindObjectOfType<Player>();
        summoner = FindObjectOfType<Summoner>();

        if (type == Type.Points)
        {
            points = new List<Vector2>();
            foreach (Transform tr in transform)
            {
                points.Add(tr.position);
            }
        }

        if (type == Type.Areas)
        {
            areas = new List<RectArea>();
            foreach (Transform tr in transform)
            {
                areas.Add(tr.GetComponent<RectArea>());
            }
        }

        //if (obj && spawnContainer)
        //{
        //    StartSpawning();
        //}
    }

    // Update is called once per frame
    void Update ()
    {
        if (spawning)
        {
            t += Time.deltaTime;
            if (t >= spawnInterval)
            {
                TriggerSpawn();

                t = t % spawnInterval;
                spawnLoops++;
                if (spawnLoops >= maxLoops)
                {
                    StopSpawning();
                }
            }
        }
	}

    private void TriggerSpawn()
    {
        for(int i=0; i < spawnBurst; i++)
        {
            Vector2 pos = Vector2.zero;
            if (type == Type.Areas)
            {
                pos = SpawnOnAreas();
            }
            else if(type == Type.Points)
            {
                pos = SpawnOnPoints();
            }
            Spawn(pos);

            if (mirror)
            {
                Vector3 v = ((Vector3)pos - transform.position)*-1;
                Spawn(transform.position + v);
                i++;
            }
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(IStartSpawining());
    }

    private IEnumerator IStartSpawining()
    {
        summoner.NotifySummon(SpawnType());
        yield return new WaitForSeconds(0.5f);
        t = spawnInterval;
        spawnLoops = 0;
        iSeq = 0;
        iUsed = new List<int>();
        spawning = true;
    }

    private int SpawnType()
    {
        if(obj.GetComponent<Biter>() != null)
        {
            return 0;
        }
        else if(obj.GetComponent<LivingBomb>() != null)
        {
            return 1;
        }
        else if(obj.GetComponent<Slasher>() != null)
        {
            return 2;
        }
        else if(obj.GetComponent<Sentinel>() != null)
        {
            return 3;
        }
        else
        {
            return -1;
        }

    }

    public void StopSpawning()
    {
        spawning = false;
    }
    private void Spawn(Vector3 pos)
    {
        GameObject spawnedObject = Instantiate(obj, pos, Quaternion.identity, spawnContainer.transform);

    }

    private Vector3 SpawnOnAreas()
    {
        Vector2 pos = Vector2.zero;
        if (order == Order.Sequential)
        {
            pos = RandomPos(areas[iSeq % areas.Count]);
            while (!MinDistanceValid(pos))
            {
                pos = RandomPos(areas[iSeq % areas.Count]);
            }
            iSeq++;
        }
        else if(order == Order.RandomNoRepeat)
        {
            int rand = Random.Range(0, areas.Count);
            while(iUsed.Contains(rand))
            {
                rand = Random.Range(0, areas.Count);
            }

            iUsed.Add(rand);

            if (iUsed.Count >= areas.Count)
            {
                iUsed.Clear();
            }

            pos = RandomPos(areas[rand]);
        }
        else if(order == Order.FullRandom)
        {
            int rand = Random.Range(0, areas.Count);
            pos = RandomPos(areas[rand]);
        }

        return pos;
    }

    private Vector3 SpawnOnPoints()
    {
        Vector2 pos = Vector2.zero;
        if (order == Order.Sequential)
        {
            pos = points[iSeq % points.Count];
            iSeq++;
        }
        else if (order == Order.RandomNoRepeat)
        {
            int rand = Random.Range(0, points.Count);
            while (iUsed.Contains(rand))
            {
                rand = Random.Range(0, points.Count);
            }

            iUsed.Add(rand);

            if (iUsed.Count >= points.Count)
            {
                iUsed.Clear();
            }

            pos = points[rand];
        }
        else if (order == Order.FullRandom)
        {
            int rand = Random.Range(0, points.Count);
            pos = points[rand];
        }

        return pos;
    }

    private Vector3 RandomPos(RectArea area)
    {
        return area.RandomPoint();
    }

    private bool MinDistanceValid(Vector3 pos)
    {
        return MinDistancePlayer(pos) && MinDistanceOther(pos);
    }

    private bool MinDistancePlayer(Vector3 pos)
    {
        return Vector3.Distance(pos, player.transform.position) >= minimumDistanceToPlayer; ;
    }

    private bool MinDistanceOther(Vector3 pos)
    {
        foreach(Transform other in spawnContainer.transform)
        {
            if (Vector3.Distance(pos, other.position) < minimumDistanceToOther)
            {
                return false;
            }
        }
        return true;
    }


    private void OnDrawGizmosSelected()
    {
        if(type == Type.Areas)
        {
            //foreach (Transform tr in transform)
            //{
            //    RectArea area = tr.GetComponent<RectArea>();
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawWireCube(transform.position, (Vector3)area.spawnAreaSize + Vector3.forward);
            //}
        }
        else if(type == Type.Points)
        {
            foreach(Transform tr in transform)
            {
                Vector2 p = tr.position;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(p - Vector2.one, p + Vector2.one);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(p - Vector2.right + Vector2.up, p + Vector2.right - Vector2.up);
            }
        }
    }
}
