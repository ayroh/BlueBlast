using Cysharp.Threading.Tasks;
using UnityEngine;

public class Rocket : CellElement
{
    [SerializeField] private SpriteRenderer leftTopSprite;
    [SerializeField] private SpriteRenderer rightBottomSprite;
    [SerializeField] private float rocketInitialX = .125f;

    private int animationEndedCount;
    public bool animationEnded => animationEndedCount == 2;

    public void Init(CellType newCellType)
    {
        if (newCellType == CellType.RocketVertical)
            transform.eulerAngles = new Vector3(0f, 0f, 90f);
        else if (newCellType == CellType.RocketHorizontal)
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else
        {
            Debug.LogWarning("Rocket Init: Non-Rocket Celltype send.");
            return;
        }


        Vector3 pos = leftTopSprite.transform.localPosition;
        pos.x = -rocketInitialX;
        leftTopSprite.transform.localPosition = pos;

        pos = rightBottomSprite.transform.localPosition;
        pos.x = rocketInitialX;
        rightBottomSprite.transform.localPosition = pos;
    }

    public override void Pop(bool doGoalAnimation = true)
    {
        SetState(CellElementState.Inactive);
        StartRocketPopAnimation();
        //_ = GridManager.instance.PopRocket(this);
    }


    public override void SetIndex(Index newIndex)
    {
        base.SetIndex(newIndex);
        leftTopSprite.sortingOrder = index.y + 1;
        rightBottomSprite.sortingOrder = index.y + 1;
    }

    public async void StartRocketPopAnimation()
    {
        InputManager.instance.SetInputState(false);
        animationEndedCount = 0;
        leftTopSprite.sortingOrder = 15;
        rightBottomSprite.sortingOrder = 15;
        LeftRocketAnimation();
        RightRocketAnimation();

        await UniTask.WaitUntil(() => animationEnded, PlayerLoopTiming.Update, GameManager.instance.GetCancellationToken());

        Release();

        DoColumnOperations();
        InputManager.instance.SetInputState(true);
    }

    private void DoColumnOperations()
    {
        if(celltype == CellType.RocketHorizontal)
        {
            for(int i = 0;i < LevelManager.instance.currentLevel.gridSize.x;++i)
            {
                GridManager.instance.ReorganizeColumn(i);
                GridManager.instance.FillColumn(i);
            }
        }
        else if(celltype == CellType.RocketVertical)
        {
            GridManager.instance.ReorganizeColumn(index.x);
            GridManager.instance.FillColumn(index.x);
        }
    }

    private async void RightRocketAnimation()
    {
        Vector3 rightBottomPos = leftTopSprite.transform.localPosition;
        int currentFrame = 0;
        int endingFrame = GridManager.instance.RocketFrameCountBetweenCubes() * LevelManager.instance.currentLevel.gridSize.x;

        while (currentFrame < endingFrame)
        {
            rightBottomPos.x += GameManager.instance.movementSO.rocketSpeed;
            rightBottomSprite.transform.localPosition = rightBottomPos;

            if(currentFrame % GridManager.instance.RocketFrameCountBetweenCubes() == 0)
            {
                if (celltype == CellType.RocketHorizontal)
                {
                    GridManager.instance.Pop(new Index(index.x + (currentFrame / GridManager.instance.RocketFrameCountBetweenCubes()), index.y));
                }
                else if (celltype == CellType.RocketVertical)
                {
                    GridManager.instance.Pop(new Index(index.x, index.y + (currentFrame / GridManager.instance.RocketFrameCountBetweenCubes())));
                }
            }

            currentFrame++;
            await UniTask.NextFrame(GameManager.instance.GetCancellationToken());
        }
        animationEndedCount++;
    }

    private async void LeftRocketAnimation()
    {
        Vector3 leftTopPos = leftTopSprite.transform.localPosition;
        int currentFrame = 0;
        int endingFrame = GridManager.instance.RocketFrameCountBetweenCubes() * LevelManager.instance.currentLevel.gridSize.x;

        while (currentFrame < endingFrame)
        {
            leftTopPos.x -= GameManager.instance.movementSO.rocketSpeed;
            leftTopSprite.transform.localPosition = leftTopPos;

            if (currentFrame % GridManager.instance.RocketFrameCountBetweenCubes() == 0)
            {
                if (celltype == CellType.RocketHorizontal)
                {
                    GridManager.instance.Pop(new Index(index.x - (currentFrame / GridManager.instance.RocketFrameCountBetweenCubes()), index.y));
                }
                else if (celltype == CellType.RocketVertical)
                {
                    GridManager.instance.Pop(new Index(index.x, index.y - (currentFrame / GridManager.instance.RocketFrameCountBetweenCubes())));
                }
            }

            currentFrame++;
            await UniTask.NextFrame(GameManager.instance.GetCancellationToken());
        }
        animationEndedCount++;
    }

}
