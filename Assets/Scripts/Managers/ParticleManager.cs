using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    public async void Play(CellElement cellElement)
    {
        Color color = Color.white;
        switch (cellElement.celltype)
        {
            case CellType.CubeYellow:
                color = Color.yellow;
                break;
            case CellType.CubeRed:
                color = Color.red;
                break;
            case CellType.CubeBlue:
                color = new Color(30 / 255f, 125 / 255f, 220 / 255f);
                break;
            case CellType.CubeGreen:
                color = Color.green;
                break;
            case CellType.CubePurple:
                color = new Color(178 / 255f, 17 / 255f, 187 / 255f);
                break;
            case CellType.Sheep:
            case CellType.RocketHorizontal:
            case CellType.RocketVertical:
            case CellType.Balloon:
            case CellType.Empty:
                return;
        }
        Particle particle1 = PoolManager.instance.GetParticle(ParticleType.Particle1);
        Particle particle2 = PoolManager.instance.GetParticle(ParticleType.Particle2);

        particle1.transform.position = cellElement.transform.position;
        particle2.transform.position = cellElement.transform.position;


        particle1.SetColor(color);
        particle2.SetColor(color);

        particle1.Play();
        particle2.Play();

        await UniTask.WaitUntil(() => particle1.isStopped && particle2.isStopped, PlayerLoopTiming.Update, GameManager.instance.GetCancellationToken());

        PoolManager.instance.ReleaseParticle(particle1);
        PoolManager.instance.ReleaseParticle(particle2);
        }


    public async void PlayStar(Vector3 pos)
    {
        Particle particleStar = PoolManager.instance.GetParticle(ParticleType.ParticleStar);

        particleStar.transform.position = pos;

        particleStar.Play();

        await UniTask.WaitUntil(() => particleStar.isStopped, PlayerLoopTiming.Update, GameManager.instance.GetCancellationToken());

        PoolManager.instance.ReleaseParticle(particleStar);
    }


}
