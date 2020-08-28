using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RainObject : MonoBehaviour
{
    
    private SoundController soundController;
    // Start is called before the first frame update
    void Start()
    {
        soundController = GetComponent<SoundController>();
        soundController.PlaySound("rain",0, true);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (Vector2)Camera.main.transform.position;
    }

    public void EndRain()
    {
        foreach(SoundController sound in GetComponentsInChildren<SoundController>())
        {
            sound.FadeOutSound(3,sound.GetComponent<AudioSource>().volume);
        }
        
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }

        Destroy(gameObject, 5);
    }
}
