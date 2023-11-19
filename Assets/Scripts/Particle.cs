using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public ParticleType particleType;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ParticleSystemRenderer rend;

    public void SetColor(Color color) => rend.material.color = color;

    public void Play() => particle.Play();

    public void Stop() => particle.Stop();

    public void Clear() => particle.Clear();

    public bool isStopped => particle.isStopped;

}


public enum ParticleType { Particle1, Particle2, ParticleStar }