using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string id, float delay = 0)
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.clip = SoundCollection.sounds[id];
        audioSource.PlayDelayed(delay);
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void PauseSound()
    {
        audioSource.Pause();
    }

    public void UnPause()
    {
        audioSource.UnPause();
    }
}
