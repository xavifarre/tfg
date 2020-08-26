using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEffect : MonoBehaviour
{
    public GameObject target;
    public int delay = 10;
    public bool replicateSprite;
    public Sprite fixedSprite;

    private SpriteRenderer targetRenderer;
    private SpriteRenderer ownRenderer;

    private Queue<Vector3> positionQueue = new Queue<Vector3>();
    private Queue<Sprite> spriteQueue = new Queue<Sprite>();
    private Queue<bool> flipQueue = new Queue<bool>();

    // Start is called before the first frame update
    void Start()
    {
        targetRenderer = target.GetComponent<SpriteRenderer>();
        ownRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (replicateSprite)
        {
            positionQueue.Enqueue(target.transform.position);

            if (!fixedSprite)
            {
                spriteQueue.Enqueue(targetRenderer.sprite);
                flipQueue.Enqueue(targetRenderer.flipX);
            }

            if (positionQueue.Count > delay)
            {
                positionQueue.Clear();
                spriteQueue.Clear();
                flipQueue.Clear();
            }
            else if (positionQueue.Count == delay)
            {
                transform.position = positionQueue.Dequeue();
                ownRenderer.sprite = fixedSprite ? fixedSprite : spriteQueue.Dequeue();
                ownRenderer.flipX = fixedSprite ? false : flipQueue.Dequeue();
                Debug.Log(flipQueue.Count);

            }
            ownRenderer.enabled = true;
        }
        else
        {
            ownRenderer.enabled = false;
        }
    }
}
