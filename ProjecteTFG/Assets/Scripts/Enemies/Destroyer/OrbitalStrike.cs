using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class OrbitalStrike : Attack
{
    private Player player;
    private Collider2D playerCollider;
    private Collider2D coll;
    private SpriteRenderer sprite;
    public SoundController soundController;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        player = FindObjectOfType<Player>();
        playerCollider = player.GetComponent<Collider2D>();
        soundController.PlaySound("orbital_charge");
    }

    public void StartStrike(float duration, float delay, float timeInterval)
    {
        StartCoroutine(IStrike(duration, delay, timeInterval));
    }

    private IEnumerator IStrike(float duration, float delay, float timeInterval)
    {
        sprite = GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(delay);
        soundController.PlaySound("orbital_strike");
        GetComponent<Animator>().SetTrigger("Strike");
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

        Destroy(gameObject, 3f);
    }
}
