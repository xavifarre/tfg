using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateFollowEffect : MonoBehaviour
{
    public GameObject target;

    public State state;

    public int delay = 10;
    public bool replicateSprite;
    public Sprite fixedSprite;
    public float minDistance = 0.1f;

    private SpriteRenderer targetRenderer;
    private SpriteRenderer ownRenderer;

    private IState targetState;

    private string lastSprite;
    private Queue<Vector3> positionQueue = new Queue<Vector3>();
    private Queue<Sprite> spriteQueue = new Queue<Sprite>();

    

    // Start is called before the first frame update
    void Start()
    {
        targetRenderer = target.GetComponent<SpriteRenderer>();
        ownRenderer = GetComponent<SpriteRenderer>();

        targetState = target.GetComponent<IState>();
    }

    private void FixedUpdate()
    {
        if (replicateSprite)
        {
            UpdatePositionOnDelay();
            UpdateSpriteOnDelay();
            UnableOnMinDistance();
        }

    }

    private void UpdatePositionOnDelay()
    {
        positionQueue.Enqueue(target.transform.position);

        if (positionQueue.Count > delay)
        {
            positionQueue.Clear();
        }
        else if (positionQueue.Count == delay)
        {
            transform.position = positionQueue.Dequeue();
        }
    }

    private void UpdateSpriteOnDelay()
    {
        if (targetState.GetState() == state)
        {
            spriteQueue.Enqueue(targetRenderer.sprite);
        }
        else
        {
            spriteQueue.Clear();
        }

        if (spriteQueue.Count > delay)
        {
            spriteQueue.Clear();
        }
        else if (spriteQueue.Count == delay)
        {
            ownRenderer.sprite = spriteQueue.Dequeue();
            ownRenderer.enabled = true;
        }
        else
        {
            ownRenderer.enabled = false;
        }
    }

    private void UnableOnMinDistance()
    {
        ownRenderer.enabled = ownRenderer.enabled && Vector3.Distance(transform.position, target.transform.position) > minDistance;
    }
}
