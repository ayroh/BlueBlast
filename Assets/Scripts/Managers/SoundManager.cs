using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip balloonPopClip;
    [SerializeField] private AudioClip cubeCollectClip;
    [SerializeField] private AudioClip cubeExplodeClip;
    [SerializeField] private AudioClip sheepPopClip;

    public void Play(Sound sound)
    {
        switch (sound)
        {
            case Sound.BalloonPop:
                audioSource.PlayOneShot(balloonPopClip);
                break;
            case Sound.CubeCollect:
                audioSource.PlayOneShot(cubeCollectClip);
                break;
            case Sound.CubeExplode:
                audioSource.PlayOneShot(cubeExplodeClip);
                break;
            case Sound.SheepPop:
                audioSource.PlayOneShot(sheepPopClip);
                break;
        }
    }

}

public enum Sound { BalloonPop, CubeCollect, CubeExplode, SheepPop };
