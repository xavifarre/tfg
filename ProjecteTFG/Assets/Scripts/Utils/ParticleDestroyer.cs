using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    List<ParticleSystem> particles;
    // Start is called before the first frame update
    void Start()
    {
        particles = new List<ParticleSystem>();
        foreach(ParticleSystem p in GetComponents<ParticleSystem>())
        {
            particles.Add(p);
        }
        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
        {
            particles.Add(p);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=particles.Count-1; i>=0;i--)
        {
            if (!particles[i].isPlaying)
            {
                particles.Remove(particles[i]);
            }
        }
        
        if (particles.Count <= 0)
        {
            Destroy(gameObject);
        }
    }
}
