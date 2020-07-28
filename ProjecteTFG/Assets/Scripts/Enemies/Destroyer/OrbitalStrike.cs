using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class OrbitalStrike : Attack
{
    private Player player;
    private Collider2D playerCollider;
    private Collider2D coll;
    private SpriteRenderer sprite;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        player = FindObjectOfType<Player>();
        playerCollider = player.GetComponent<Collider2D>();
    }

    public void StartStrike(float duration, float delay, float timeInterval)
    {
        StartCoroutine(IStrike(duration, delay, timeInterval));
    }

    private IEnumerator IStrike(float duration, float delay, float timeInterval)
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.4f);
        yield return new WaitForSeconds(delay);
        sprite.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.4f);
        float t = 0;
        while(t < duration)
        {
            t += timeInterval;
            if (coll.IsTouching(playerCollider))
            {
                player.HitByTime((int)(damage * timeInterval/duration));
            }
            yield return new WaitForSeconds(timeInterval);
            
        }
        Destroy(gameObject);
    }
}
