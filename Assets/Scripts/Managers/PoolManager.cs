using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [Header("Parents")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private Transform particleParent;

    [Header("Prefabs")]
    [SerializeField] private Cube cubePrefab;
    [SerializeField] private Rocket rocketPrefab;
    [SerializeField] private Balloon balloonPrefab;
    [SerializeField] private Duck duckPrefab;
    [SerializeField] private Goal goalPrefab;
    [SerializeField] private Particle particle1Prefab;
    [SerializeField] private Particle particle2Prefab;
    [SerializeField] private Particle particleStarPrefab;

    [Header("Cell Sprites")]
    [SerializeField] private Sprite cubeYellowSprite;
    [SerializeField] private Sprite cubeRedSprite;
    [SerializeField] private Sprite cubeBlueSprite;
    [SerializeField] private Sprite cubeGreenSprite;
    [SerializeField] private Sprite cubePurpleSprite;
    [SerializeField] private Sprite balloonSprite;
    [SerializeField] private Sprite duckSprite;

    private Queue<Cube> availableCubes = new Queue<Cube>();
    private Queue<Rocket> availableRockets = new Queue<Rocket>();
    private Queue<Duck> availableDucks = new Queue<Duck>();
    private Queue<Balloon> availableBalloons = new Queue<Balloon>();
    private Queue<Goal> availableGoals = new Queue<Goal>();
    private Queue<Particle> availableParticle1 = new Queue<Particle>();
    private Queue<Particle> availableParticle2 = new Queue<Particle>();
    private Queue<Particle> availableParticleStar = new Queue<Particle>();

    public CellElement Get(CellType cellType)
    {
        CellElement cellElement = null;
        switch (cellType)
        {
            case CellType.CubeYellow:
                cellElement = GetCube();
                (cellElement as Cube).SetSprite(cubeYellowSprite);
                break;
            case CellType.CubeRed:
                cellElement = GetCube();
                (cellElement as Cube).SetSprite(cubeRedSprite);
                break;
            case CellType.CubeBlue:
                cellElement = GetCube();
                (cellElement as Cube).SetSprite(cubeBlueSprite);
                break;
            case CellType.CubeGreen:
                cellElement = GetCube();
                (cellElement as Cube).SetSprite(cubeGreenSprite);
                break;
            case CellType.CubePurple:
                cellElement = GetCube();
                (cellElement as Cube).SetSprite(cubePurpleSprite);
                break;
            case CellType.Duck:
                cellElement = GetDuck();
                break;
            case CellType.RocketHorizontal:
                cellElement = GetRocket();
                (cellElement as Rocket).Init(CellType.RocketHorizontal);
                break;
            case CellType.RocketVertical:
                cellElement = GetRocket();
                (cellElement as Rocket).Init(CellType.RocketVertical);
                break;
            case CellType.Balloon:
                cellElement = GetBalloon();
                break;
            case CellType.Empty:
                return null;
        }
        cellElement.SetCellType(cellType);
        cellElement.SetState(CellElementState.Active);
        cellElement.gameObject.SetActive(true);
        return cellElement;
    }


    private Cube GetCube()
    {
        Cube cube;
        if (availableCubes.Count == 0)
        {
            cube = CreateCube();
        }
        else
        {
            cube = availableCubes.Dequeue();
        }
        return cube;
    }

    private Cube CreateCube()
    {
        Cube newCube = Instantiate(cubePrefab, gridParent);
        return newCube;
    }

    private Rocket GetRocket()
    {
        Rocket rocket;
        if (availableRockets.Count == 0)
        {
            rocket = CreateRocket();
        }
        else
        {
            rocket = availableRockets.Dequeue();
        }
        return rocket;
    }

    private Rocket CreateRocket()
    {
        Rocket newRocket = Instantiate(rocketPrefab, gridParent);
        return newRocket;
    }

    private Duck GetDuck()
    {
        Duck duck;
        if (availableDucks.Count == 0)
        {
            duck = CreateDuck();
        }
        else
        {
            duck = availableDucks.Dequeue();
        }
        return duck;
    }

    private Duck CreateDuck()
    {
        Duck newDuck = Instantiate(duckPrefab, gridParent);
        return newDuck;
    }

    private Balloon GetBalloon()
    {
        Balloon balloon;
        if (availableBalloons.Count == 0)
        {
            balloon = CreateBalloon();
        }
        else
        {
            balloon = availableBalloons.Dequeue();
        }
        return balloon;
    }

    private Balloon CreateBalloon()
    {
        Balloon balloon = Instantiate(balloonPrefab, gridParent);
        return balloon;
    }


    public void Release(CellElement cellElement, bool doGoalAnimation = true)
    {
        GoalManager.instance.SendToGoal(cellElement, doGoalAnimation);
        cellElement.gameObject.SetActive(false);
        cellElement.SetState(CellElementState.Inactive);
        GridManager.instance.SetList(cellElement.index, null);
        cellElement.transform.position.Set(0f, 0f, -10f);

        switch (cellElement.celltype)
        {
            case CellType.CubeYellow:
            case CellType.CubeRed:
            case CellType.CubeBlue:
            case CellType.CubeGreen:
            case CellType.CubePurple:
                availableCubes.Enqueue(cellElement as Cube);
                break;
            case CellType.Duck:
                availableDucks.Enqueue(cellElement as Duck);
                break;
            case CellType.RocketHorizontal:
            case CellType.RocketVertical:
                availableRockets.Enqueue(cellElement as Rocket);
                break;
            case CellType.Balloon:
                availableBalloons.Enqueue(cellElement as Balloon);
                break;
            case CellType.Empty:
                return;
        }
    }



    private Goal CreateGoal()
    {
        Goal goal = Instantiate(goalPrefab);
        return goal;
    }


    public Goal GetGoal(CellType cellType)
    {
        Goal goal;
        if (availableGoals.Count == 0)
        {
            goal = CreateGoal();
        }
        else
        {
            goal = availableGoals.Dequeue();
        }

        Sprite goalSprite = null;
        switch (cellType)
        {
            case CellType.CubeYellow:
                goalSprite = cubeYellowSprite;
                break;
            case CellType.CubeRed:
                goalSprite = cubeRedSprite;
                break;
            case CellType.CubeBlue:
                goalSprite = cubeBlueSprite;
                break;
            case CellType.CubeGreen:
                goalSprite = cubeGreenSprite;
                break;
            case CellType.CubePurple:
                goalSprite = cubePurpleSprite;
                break;
            case CellType.Duck:
                goalSprite = duckSprite;
                break;
            case CellType.Balloon:
                goalSprite = balloonSprite;
                break;
            case CellType.RocketHorizontal:
            case CellType.RocketVertical:
            case CellType.Empty:
                Debug.LogWarning("PoolManager GetGoal: Wrong CellType");
                return null;
        }
        goal.SetImage(goalSprite);
        goal.SetCellType(cellType);
        goal.gameObject.SetActive(true);
        return goal;
    }

    public void ReleaseGoal(Goal goal)
    {
        goal.gameObject.SetActive(false);
        goal.SetImage(null);
        goal.SetCellType(CellType.Empty);

        availableGoals.Enqueue(goal);
    }


    private Particle CreateParticle1()
    {
        Particle particleSystem = Instantiate(particle1Prefab, particleParent);
        return particleSystem;
    }

    private Particle GetParticle1()
    {
        Particle particle1;
        if (availableParticle1.Count == 0)
        {
            particle1 = CreateParticle1();
        }
        else
        {
            particle1 = availableParticle1.Dequeue();
        }
        particle1.gameObject.SetActive(true);
        return particle1;
    }


    private Particle CreateParticle2()
    {
        Particle particleSystem = Instantiate(particle2Prefab, particleParent);
        return particleSystem;
    }

    private Particle GetParticle2()
    {
        Particle particle2;
        if (availableParticle2.Count == 0)
        {
            particle2 = CreateParticle2();
        }
        else
        {
            particle2 = availableParticle2.Dequeue();
        }
        particle2.gameObject.SetActive(true);
        return particle2;
    }

    private Particle CreateParticleStar()
    {
        Particle particleSystem = Instantiate(particleStarPrefab, particleParent);
        return particleSystem;
    }

    private Particle GetParticleStar()
    {
        Particle particleStar;
        if (availableParticleStar.Count == 0)
        {
            particleStar = CreateParticleStar();
        }
        else
        {
            particleStar = availableParticleStar.Dequeue();
        }
        particleStar.gameObject.SetActive(true);
        return particleStar;
    }
    public void ReleaseParticle(Particle particle)
    {
        particle.Stop();
        particle.Clear();
        switch (particle.particleType)
        {
            case ParticleType.Particle1:
                availableParticle1.Enqueue(particle);
                break;
            case ParticleType.Particle2:
                availableParticle2.Enqueue(particle);
                break;
            case ParticleType.ParticleStar:
                availableParticleStar.Enqueue(particle);
                break;
        }
        particle.gameObject.SetActive(false);
    }

    public Particle GetParticle(ParticleType particleType)
    {
        Particle particle = null;
        switch (particleType)
        {
            case ParticleType.Particle1:
                particle = GetParticle1();
                break;
            case ParticleType.Particle2:
                particle = GetParticle2();
                break;
            case ParticleType.ParticleStar:
                particle = GetParticleStar();
                break;
        }
        return particle;
    }

}
