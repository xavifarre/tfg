using UnityEngine;
using System.Collections;

public class CrystalDestroyer : Attack
{
    private bool floating;

    public SpriteRenderer sprite;
    public float rotationMultiplier;
    private Vector3 rotationValue;
    private Player player;
    private Collider2D crystalCollider;

    private void Start()
    {
        rotationValue = new Vector3(Random.Range(1, 11), Random.Range(1, 11), Random.Range(1, 11)).normalized;
        transform.rotation = Random.rotation;
        player = FindObjectOfType<Player>();
        crystalCollider = GetComponent<Collider2D>();
        crystalCollider.enabled = false;
    }

    private void Update()
    {
        UpdateRotation();
    }

    public void Float()
    {
        floating = true;
        StartCoroutine(IFloat());
    }

    public void AttackPlayer(float speed, float duration)
    {
        floating = false;
        crystalCollider.enabled = true;
        StartCoroutine(IAttack(speed, duration));
    }

    public void CrystalPierce(int dir, Destroyer._HorizontalPierce stats, Vector3 destroyerPos)
    {
        StartCoroutine(ICrystalPierce(dir, stats, destroyerPos));
    }

    private void UpdateRotation()
    {
        sprite.transform.Rotate(rotationValue * rotationMultiplier * Time.deltaTime);
    }

    private IEnumerator IFloat()
    {
        float A = 0.1f, w = 0.8f, t = 0;
        float startY = transform.position.y;
        while (floating)
        {
            t += Time.deltaTime;
            float ySinus = A * Mathf.Sin(w * t * Mathf.PI);
            transform.position = new Vector3(transform.position.x, startY + ySinus, transform.position.z);
            yield return null;
        }
    }

    private IEnumerator IAttack(float speed, float duration)
    {
        float t = 0;
        Vector3 dir = (player.transform.position - transform.position).normalized;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position += dir * speed * Time.deltaTime;
            yield return null;
        }
        sprite.enabled = false;
        
        Destroy(gameObject, 1f);
    }

    private IEnumerator ICrystalPierce(int dir, Destroyer._HorizontalPierce stats, Vector3 destroyerPos)
    {
        Vector3 startPos = transform.position;
        Vector3 destPos = transform.position + Random.Range(stats.crystalRange.x, stats.crystalRange.y) * (Vector3)Random.insideUnitCircle;
        yield return null;
        crystalCollider.enabled = true;

        float t = 0;
        while (t < stats.crystalReleaseDuration)
        {
            t += Time.deltaTime;
            transform.position = MathFunctions.EaseOutExp(t, startPos, destPos, stats.crystalReleaseDuration, 5);
            yield return null;
            
        }

        yield return new WaitForSeconds(stats.crystalRecallDelay);
        float speed = stats.crystalRecallSpeed;
        while (!((transform.position.x > destroyerPos.x && destPos.x < destroyerPos.x) || (transform.position.x < destroyerPos.x  && destPos.x > destroyerPos.x)))
        {
            speed += stats.crystalRecallAcceleration * Time.deltaTime;
            transform.position += (destroyerPos - destPos) * speed * Time.deltaTime;
            yield return null;
        }
        sprite.gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("hit");
            collision.GetComponent<Player>().Hit(this);
        }
    }

}
