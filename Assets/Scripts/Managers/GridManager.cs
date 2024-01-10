using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    readonly private Vector2 gridGap = new Vector2(.2f, .4f);
    readonly private Vector2 cellSize = Vector2.one;

    [SerializeField] private SpriteRenderer border;
    [SerializeField] private MovementSO movementSO;

    private const int widthOffset = 50;
    private const int heightOffset = 900;

    private Vector2 borderLeftBottom;
    private float borderScale;

    private Vector3 respawnPoint;
    private Vector3 spawnPoint;

    private List<List<CellElement>> cells = new List<List<CellElement>>();

    public bool CellAvailable(CellElement cellElement) => (cellElement != null && cellElement.state == CellElementState.Active);

    public void InitialiseLevel()
    {
        LevelManager.instance.LoadLevel();

        ResetGrid();


        ResizeBorder();
        CreateGridElements();

        GoalManager.instance.SetCurrentLevelGoals();
        GoalManager.instance.SetNumberOfMoves();

        borderLeftBottom = new Vector2(spawnPoint.x - (cellSize.x * borderScale) / 2, spawnPoint.y - (cellSize.y * borderScale) / 2);
        border.gameObject.SetActive(true);

        MenuManager.instance.SetInGameMenu(true);

        GameManager.instance.SetState(GameState.Started);
    }

    public void NextLevel()
    {

        InitialiseLevel();
    }

    private void ResetGrid()
    {
        for (int i = 0;i < cells.Count;++i)
        {
            for (int j = 0;j < cells[i].Count;++j)
            {
                if (cells[i][j] == null)
                    continue;

                cells[i][j].Release();
            }
            cells[i].Clear();
        }
        cells.Clear();

        for (int i = 0;i < LevelManager.instance.currentLevel.gridSize.x;++i)
            cells.Add(new List<CellElement>());

        for (int i = 0;i < LevelManager.instance.currentLevel.gridSize.x;++i)
        {
            for (int j = 0;j < LevelManager.instance.currentLevel.gridSize.x;++j)
            {
                cells[i].Add(null);
            }
        }
    }


    public void ReorganizeColumn(int column)
    {
        for (int i = 1;i < LevelManager.instance.currentLevel.gridSize.y;++i)
        {
            if (cells[column][i] == null || cells[column][i].state == CellElementState.Inactive)
                continue;

            int yPosition = -1;
            for (int j = 0;j < i;++j)
            {
                if (cells[column][j] == null)
                {
                    yPosition = j;
                    break;
                }
            }
            if (yPosition == -1)
                continue;
            Index cellIndex = new Index(cells[column][i].index);
            cells[column][i].SetIndex(new Index(column, yPosition));

            _ = cells[column][i].SetNewYPosition(GetYPosition(yPosition), false);

            cells[column][yPosition] = cells[column][i];
            SetList(cellIndex, null);
        }
    }



    public async UniTask CreateRocket(List<CellElement> popCells, Index index)
    {
        InputManager.instance.SetInputState(false);

        Vector3 indexPosition = GetIndexPosition(index);
        Sequence createRocketSequence = DOTween.Sequence();
        for (int i = 0;i < popCells.Count;++i)
        {
            createRocketSequence.Join(popCells[i].transform.DOMove(indexPosition, movementSO.creationAnimation).SetEase(movementSO.createRocketCurve));
        }

        await createRocketSequence.AsyncWaitForCompletion();

        cells[index.x][index.y].Pop(false);
        for (int i = 0;i < popCells.Count;++i)
        {
            popCells[i].Pop(false);
        }

        CellElement rocket = PoolManager.instance.Get(RandomRocket());
        rocket.transform.position = indexPosition;
        rocket.SetIndex(index);
        cells[index.x][index.y] = rocket;

        popCells.Clear();

        InputManager.instance.SetInputState(true);
    }

    public void Pop(Index index)
    {
        if (!IsIndexValid(index))
            return;

        CellElement cellElement = GetCellElement(index);
        if (CellAvailable(cellElement) && cellElement.IsDestroyable())
            cellElement.Pop();
    }

    //public async UniTask PopRocket(Rocket rocket)
    //{
    //    if (rocket.celltype == CellType.RocketHorizontal)
    //    {
    //        rocket.StartRocketPopAnimation();

    //        int validCount = 0;
    //        for (int i = 1;i < LevelManager.instance.currentLevel.gridSize.x;++i)
    //        {
    //            await UniTask.DelayFrame(RocketFrameCountBetweenCubes(), PlayerLoopTiming.Update, GameManager.instance.GetCancellationToken());
    //            validCount = 0;

    //            Index index = new Index(rocket.index.x + i, rocket.index.y);
    //            if (IsIndexValid(index))
    //            {
    //                validCount++;
    //                CellElement cellElement = GetCellElement(index);
    //                if (cellElement != null && cellElement.state == CellElementState.Active && cellElement.IsDestroyable())
    //                    cellElement.Pop();
    //            }

    //            index = new Index(rocket.index.x - i, rocket.index.y);
    //            if (IsIndexValid(index))
    //            {
    //                validCount++;
    //                CellElement cellElement = GetCellElement(index);
    //                if (cellElement != null && cellElement.state == CellElementState.Active && cellElement.IsDestroyable())
    //                    cellElement.Pop();
    //            }

    //            if (validCount == 0)
    //                break;
    //        }

    //        await UniTask.WaitUntil(() => rocket.animationEnded, PlayerLoopTiming.Update, GameManager.instance.GetCancellationToken());

    //        rocket.Release();

    //        for (int i = 0;i < LevelManager.instance.currentLevel.gridSize.x;++i)
    //        {
    //            ReorganizeColumn(i);
    //            FillColumn(i);
    //        }
    //    }
    //    else if (rocket.celltype == CellType.RocketVertical)
    //    {
    //        rocket.StartRocketPopAnimation();

    //        int validCount = 0;
    //        for (int i = 1;i < LevelManager.instance.currentLevel.gridSize.y;++i)
    //        {
    //            await UniTask.DelayFrame(RocketFrameCountBetweenCubes(), PlayerLoopTiming.Update, GameManager.instance.GetCancellationToken());
    //            validCount = 0;
    //            Index index = new Index(rocket.index.x, rocket.index.y + i);
    //            if (IsIndexValid(index))
    //            {
    //                validCount++;
    //                CellElement cellElement = GetCellElement(index);
    //                if (cellElement != null && cellElement.state == CellElementState.Active && cellElement.IsDestroyable())
    //                    cellElement.Pop();
    //            }

    //            index = new Index(rocket.index.x, rocket.index.y - i);
    //            if (IsIndexValid(index))
    //            {
    //                validCount++;
    //                CellElement cellElement = GetCellElement(index);
    //                if (cellElement != null && cellElement.state == CellElementState.Active && cellElement.IsDestroyable())
    //                    cellElement.Pop();
    //            }

    //            if (validCount == 0)
    //                break;
    //        }

    //        await UniTask.WaitUntil(() => rocket.animationEnded, PlayerLoopTiming.Update, GameManager.instance.GetCancellationToken());

    //        rocket.Release();
    //        ReorganizeColumn(rocket.index.x);
    //        FillColumn(rocket.index.x);
    //    }
    //}



    public void FillColumn(int column)
    {
        int numberOfCountInColumn = 0;
        for (int i = 0;i < LevelManager.instance.currentLevel.gridSize.y;++i)
        {
            if (cells[column][i] != null)
                continue;
            CellElement cell = PoolManager.instance.Get(RandomCube());
            FitToCell(cell, new Index(column, i), numberOfCountInColumn);
            ++numberOfCountInColumn;
        }
    }


    public void FitToCell(CellElement cellElement, Index index, int numberOfCountInColumn)
    {
        cellElement.transform.position = respawnPoint + new Vector3(index.x * cellSize.x * borderScale, numberOfCountInColumn * cellSize.y * borderScale, 0f);
        cellElement.SetIndex(index);
        _ = cellElement.SetNewYPosition(spawnPoint.y + index.y * cellSize.y * borderScale, true);
        cells[index.x][index.y] = cellElement;
    }



    private void ResizeBorder()
    {

        float xSize = LevelManager.instance.currentLevel.gridSize.x * cellSize.x + gridGap.x;
        float ySize = LevelManager.instance.currentLevel.gridSize.y * cellSize.y + gridGap.y;
        border.size = new Vector2(xSize, ySize);

        Vector3 leftBottom = border.transform.position - new Vector3(border.size.x / 2, border.size.y / 2);
        Vector3 rightBottom = border.transform.position + new Vector3((border.size.x) / 2, -(border.size.y) / 2);
        Vector3 leftTop = border.transform.position + new Vector3(-border.size.x / 2, border.size.y / 2);

        float xPixelCount = (Camera.main.WorldToScreenPoint(rightBottom) - Camera.main.WorldToScreenPoint(leftBottom)).x;
        float yPixelCount = (Camera.main.WorldToScreenPoint(leftTop) - Camera.main.WorldToScreenPoint(leftBottom)).y;

        float widthScale = (Screen.width - widthOffset) / xPixelCount;
        float heightScale = (Screen.height - heightOffset) / yPixelCount;

        borderScale = Math.Min(widthScale, heightScale);
        border.transform.localScale = new Vector3(borderScale, borderScale, 1f);

        spawnPoint = border.transform.position - new Vector3(borderScale * (border.size.x - gridGap.x - cellSize.x) / 2, borderScale * (border.size.y - gridGap.y - cellSize.y) / 2);
        respawnPoint = border.transform.position + new Vector3(-borderScale * (border.size.x - gridGap.x - cellSize.x) / 2, (borderScale * (border.size.y - gridGap.y - cellSize.y) / 2) + borderScale * cellSize.y * 5);
    }

    private void CreateGridElements()
    {
        for (int i = 0;i < LevelManager.instance.currentLevel.gridSize.x;++i)
        {
            for (int j = 0;j < LevelManager.instance.currentLevel.gridSize.y;++j)
            {
                CellElement cell = PoolManager.instance.Get(LevelManager.instance.ConvertLevel(new Index(i, j)));
                if (cell == null)
                {
                    cell = PoolManager.instance.Get(RandomCube());
                }
                cell.transform.position = GetIndexPosition(new Index(i, j));
                cell.SetIndex(new Index(i, j));
                cells[i][j] = cell;
            }
        }
    }

    public Index GetGridIndex(Vector2 mousePos)
    {
        Vector2 gridPos = new Vector2(mousePos.x - borderLeftBottom.x, mousePos.y - borderLeftBottom.y);
        if (!IsPositionValid(gridPos))
            return Index.Default;

        return new Index((int)(gridPos.x / borderScale), (int)(gridPos.y / borderScale));
    }


    public CellElement GetGridElement(Index index)
    {
        if (!IsIndexValid(index))
            return null;

        return GetCellElement(index);
    }

    public int RocketFrameCountBetweenCubes() => (int)(CellGap() / movementSO.rocketSpeed);

    public void SetList(Index index, CellElement cellElement) => cells[index.x][index.y] = cellElement;

    public bool IsIndexActive(Index index) => cells[index.x][index.y].state == CellElementState.Active;

    public float CellGap() => cellSize.x * borderScale * 2;

    public float GetYPosition(int row) => spawnPoint.y + (row * cellSize.y * borderScale);

    private bool IsPositionValid(Vector2 gridPos) => (gridPos.x < 0 || gridPos.y < 0 || gridPos.x > LevelManager.instance.currentLevel.gridSize.x * borderScale || gridPos.y > LevelManager.instance.currentLevel.gridSize.y * borderScale) ? false : true;

    private bool IsIndexValid(Index index) => (index.x < 0 || index.y < 0 || index.x >= LevelManager.instance.currentLevel.gridSize.x || index.y >= LevelManager.instance.currentLevel.gridSize.y) ? false : true;

    private CellElement GetCellElement(Index index) => cells[index.x][index.y];

    private Vector3 GetIndexPosition(Index index) => spawnPoint + new Vector3(index.x * cellSize.x * borderScale, index.y * cellSize.y * borderScale, 0f);




    public CellType RandomRocket() => (UnityEngine.Random.Range(0, 2) == 0) ? CellType.RocketHorizontal : CellType.RocketVertical;

    public CellType RandomCube() =>  LevelManager.instance.currentLevel.dropables[UnityEngine.Random.Range(0, LevelManager.instance.currentLevel.dropables.Count)];//cubeTypes[UnityEngine.Random.Range(0, cubeTypes.Length)];


}
