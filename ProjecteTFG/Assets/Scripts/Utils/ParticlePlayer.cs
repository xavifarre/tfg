using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public List<ParticleSystem> particleList;

    public void PlayParticles()
    {
        foreach(ParticleSystem particles in particleList)
        {
            particles.Play();
        }
    }

    public void StopParticles()
    {
        foreach (ParticleSystem particles in particleList)
        {
            particles.Stop();
        }
    }
}
