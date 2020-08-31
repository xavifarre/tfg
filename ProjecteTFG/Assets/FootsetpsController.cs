using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootsetpsController : MonoBehaviour
{
    public float interval;
    private int currentZone;
    private Player player;
    private SoundController soundController;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<Player>();
        soundController = GetComponent<SoundController>();
        StartCoroutine(IFootstepPlayer());
    }

    private IEnumerator IFootstepPlayer()
    {
        while (true)
        {
            if(player.movementValue.magnitude > 0 && player.GetState() == State.Idle && currentZone != -1)
            {
                int rand = Random.Range(1, 7);

                string id = "";
                if (currentZone == 0)
                {
                    id = "concrete";
                }
                else if (currentZone == 1)
                {
                    id = "grass";
                }
                id += rand.ToString();
                soundController.PlaySound(id);
            }

            yield return new WaitForSeconds(interval);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Concrete")
        {
            currentZone = 0;
        }
        else if (collision.tag == "Grass")
        {
            currentZone = 1;
        }
        else if (collision.tag == "Untagged")
        {
            currentZone = -1;
        }
    }
}
