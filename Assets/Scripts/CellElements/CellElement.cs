using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CellElement : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    private float desiredYPosition = 0f;

    public CellType celltype { get; private set; }
    public CellElementState state { get; private set; }
    public Index index { get; private set; } = Index.Default;

    private IEnumerator bounceCoroutine;

    

    public async virtual UniTask SetNewYPosition(float newYPosition, bool fromSky)
    {
        desiredYPosition = newYPosition;

        if (state == CellElementState.Sliding)
        {
            return;
        }

        if (state == CellElementState.Bouncing)
        {
            StopCoroutine(bounceCoroutine);
            bounceCoroutine = null;
            state = CellElementState.Active;
        }

        if (fromSky)
        {
            await Fall();
            StartCoroutine(bounceCoroutine = Bounce(GameManager.instance.movementSO.bounceCurve.keys[^1].time, GameManager.instance.movementSO.skyFallBounceMultiplier));
        }
        else
        {
            await Fill();
            StartCoroutine(bounceCoroutine = Bounce(GameManager.instance.movementSO.bounceCurve.keys[^1].time, GameManager.instance.movementSO.gridFallBounceMultiplier));
        }

    }

    public async UniTask Fill()
    {
        SetState(CellElementState.Sliding);

        Vector3 pos = transform.position;
        float speed = GameManager.instance.movementSO.gridFallInitialSpeed;
        while (desiredYPosition < transform.position.y)
        {
            speed += GameManager.instance.movementSO.gridFallAcceleration;
            pos.y -= speed;
            transform.position = pos;
            await UniTask.NextFrame(GameManager.instance.GetCancellationToken());
        }
        pos.y = desiredYPosition;
        transform.position = pos;

        SetState(CellElementState.Active);
    }

    public async UniTask Fall()
    {
        SetState(CellElementState.Sliding);

        Vector3 pos = transform.position;
        float speed = GameManager.instance.movementSO.skyFallInitialSpeed;
        while (desiredYPosition < transform.position.y)
        {
            speed += GameManager.instance.movementSO.skyFallAcceleration;
            pos.y -= speed;
            transform.position = pos;

            await UniTask.NextFrame(GameManager.instance.GetCancellationToken());
        }

        pos.y = desiredYPosition;
        transform.position = pos;

        SetState(CellElementState.Active);
    }

    private IEnumerator Bounce(float animationTime, float timeMultiplier)
    {
        state = CellElementState.Bouncing;

        Vector3 pos = transform.position;
        float time = 0f;
        float bouncePoint = GameManager.instance.movementSO.bounceOffset + pos.y;
        float startPointY = pos.y;
        while (time < animationTime)
        {
            pos.y = Mathf.Lerp(startPointY, bouncePoint, GameManager.instance.movementSO.bounceCurve.Evaluate(time));
            transform.position = pos;
            time += Time.deltaTime * timeMultiplier;
            yield return null;
        }

        pos.y = desiredYPosition;
        transform.position = pos;
        state = CellElementState.Active;
    }

    public void SetCellType(CellType newCelltype) => celltype = newCelltype;

    public void SetState(CellElementState newState) => state = newState;

    public virtual void SetIndex(Index newIndex)
    {
        index = newIndex;
        spriteRenderer.sortingOrder = index.y + 1;
    }

    public virtual void Pop(bool doGoalAnimation = true) => Release(doGoalAnimation);

    public bool IsDestroyable() => celltype == CellType.Duck ? false : true;

    public virtual void Release(bool doGoalAnimation = true) => PoolManager.instance.Release(this, doGoalAnimation);
}

public struct Index
{
    // X Column
    // Y Row
    public int x, y;

    public Index(Index temp)
    {
        x = temp.x;
        y = temp.y;
    }
    public Index(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Index Default = new Index(-1, -1);

    public static bool operator ==(Index index1, Index index2) => (index1.x == index2.x && index1.y == index2.y);

    public static bool operator !=(Index index1, Index index2) => !(index1.x == index2.x && index1.y == index2.y);


    public override string ToString()
    {
        return x + "," + y;
    }

    public override bool Equals(object obj)
    {
        return obj is Index index &&
               x == index.x &&
               y == index.y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}

public enum CellElementState
{
    Inactive,
    Active,
    Sliding,
    Bouncing
}

public enum CellType
{
    Empty,
    CubeYellow,
    CubeRed,
    CubeBlue,
    CubeGreen,
    CubePurple,
    Duck,
    RocketHorizontal,
    RocketVertical,
    Balloon
}