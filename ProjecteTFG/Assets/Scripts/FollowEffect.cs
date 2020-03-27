using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEffect : MonoBehaviour
{
    public GameObject target;
    public int delay = 10;
    public string animationStringConstraint;
    public bool replicateSprite;
    public Sprite fixedSprite;

    private SpriteRenderer targetRenderer;
    private SpriteRenderer ownRenderer;
    
    

    private Queue<Vector3> positionQueue = new Queue<Vector3>();
    private Queue<Sprite> spriteQueue = new Queue<Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        targetRenderer = target.GetComponent<SpriteRenderer>();
        ownRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

        positionQueue.Enqueue(target.transform.position);
        if (replicateSprite)
        {
            spriteQueue.Enqueue(targetRenderer.sprite);
        }

        if(positionQueue.Count > delay)
        {
            positionQueue.Clear();
            spriteQueue.Clear();
        }
        else if(positionQueue.Count == delay)
        {
            transform.position = positionQueue.Dequeue();
            if (replicateSprite)
            {
                ownRenderer.sprite = spriteQueue.Dequeue();
            }
        }
        
        if (animationStringConstraint != "" && ownRenderer.sprite.name.Contains(animationStringConstraint))
        {
            ownRenderer.enabled = targetRenderer.sprite.name.Contains(animationStringConstraint);
        }

    }
}
