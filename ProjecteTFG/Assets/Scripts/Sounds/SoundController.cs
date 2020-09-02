using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public AudioSource GetSource()
    {
        if (audioSource)
        {
            return audioSource;
        }
        return GetComponent<AudioSource>();
    }

    public void PlaySound(string id, float delay = 0, bool loop = false)
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.loop = loop;
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

    public void RandomPitch(float from, float to)
    {
        audioSource.pitch = Random.Range(from * 1000, to * 1000) / 1000;
    }

    public void FadeInSound(float duration, float maxVolume = 1, float delay = 0)
    {
        StartCoroutine(IFadeInSound(duration, maxVolume, delay));
    }

    public void FadeOutSound(float duration, float maxVolume = 1, float delay = 0)
    {
        StartCoroutine(IFadeOutSound(duration, maxVolume, delay));
    }


    private IEnumerator IFadeInSound(float duration, float maxVolume, float delay)
    {
        float t = 0;
        audioSource.volume = 0;
        yield return new WaitForSeconds(delay);
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, maxVolume, t / duration);
            yield return null;
        }
    }

    private IEnumerator IFadeOutSound(float duration, float maxVolume, float delay)
    {
        float t = 0;
        audioSource.volume = maxVolume;
        yield return new WaitForSeconds(delay);
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(maxVolume, 0, t / duration);
            yield return null;
        }
    }


    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

}
