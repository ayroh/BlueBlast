using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public MovementSO movementSO;

    private List<CellElement> popCells = new List<CellElement>();
    private List<Balloon> popBalloons = new List<Balloon>();
    private List<int> columnsToFill = new List<int>();

    private CancellationTokenSource source = new CancellationTokenSource();
    private CancellationToken token;
    public CancellationToken GetCancellationToken() => token;

    private void Start()
    {
        token = source.Token;
        GridManager.instance.StartGame();
    }


    private void OnApplicationQuit()
    {
        source.Cancel();
    }

    public async void OnCellClickedAsync(Index index)
    {
        ClearLists();
        CellElement cellElement = GridManager.instance.GetGridElement(index);
        if (cellElement == null || cellElement.state != CellElementState.Active)
            return;

        switch (cellElement.celltype)
        {
            case CellType.CubeYellow:
            case CellType.CubeRed:
            case CellType.CubeBlue:
            case CellType.CubeGreen:
            case CellType.CubePurple:

                popCells.Add(cellElement);
                ClickCell(cellElement);

                if (popCells.Count < 2)
                {
                    ClearLists();
                    return;
                }

                SoundManager.instance.Play(Sound.CubeExplode);

                if (popCells.Count >= 5)
                {
                    popCells.Remove(cellElement);
                    for (int i = 0;i < popCells.Count;++i)
                    {
                        if (!columnsToFill.Contains(popCells[i].index.x))
                            columnsToFill.Add(popCells[i].index.x);
                    }

                    await GridManager.instance.CreateRocket(popCells, index);
                }
                else
                {
                    for (int i = 0;i < popCells.Count;++i)
                    {
                        popCells[i].Pop();
                        if (!columnsToFill.Contains(popCells[i].index.x))
                            columnsToFill.Add(popCells[i].index.x);
                    }
                }

                for (int i = 0;i < popBalloons.Count;++i)
                {
                    popBalloons[i].Pop();
                    if (!columnsToFill.Contains(popBalloons[i].index.x))
                        columnsToFill.Add(popBalloons[i].index.x);
                }

                for (int i = 0;i < columnsToFill.Count;++i)
                {
                    GridManager.instance.ReorganizeColumn(columnsToFill[i]);
                }

                for (int i = 0;i < columnsToFill.Count;++i)
                {
                    GridManager.instance.FillColumn(columnsToFill[i]);
                }
                break;
            case CellType.Duck:
                return;
            case CellType.RocketHorizontal:
            case CellType.RocketVertical:
                cellElement.Pop();
                break;
            case CellType.Balloon:
                return;
            case CellType.Empty:
                return;
        }
        if (GoalManager.instance.DecrementNumberOfMoves() == 0)
            GoalManager.instance.CheckEnd();
    }


    private void ClickCell(CellElement cellElement)
    {
        List<CellElement> adjacentElements = new List<CellElement> {
            GridManager.instance.GetGridElement(new Index(cellElement.index.x, cellElement.index.y + 1)),
            GridManager.instance.GetGridElement(new Index(cellElement.index.x, cellElement.index.y - 1)),
            GridManager.instance.GetGridElement(new Index(cellElement.index.x - 1, cellElement.index.y)),
            GridManager.instance.GetGridElement(new Index(cellElement.index.x + 1, cellElement.index.y))
        };

        for (int i = 0;i < adjacentElements.Count;++i)
        {
            if (adjacentElements[i] != null && adjacentElements[i].state == CellElementState.Active)
            {
                if (adjacentElements[i].celltype == CellType.Balloon && !popBalloons.Contains(adjacentElements[i] as Balloon))
                {
                    popBalloons.Add(adjacentElements[i] as Balloon);
                }
                else if (cellElement.celltype == adjacentElements[i].celltype && !popCells.Contains(adjacentElements[i]))
                {
                    popCells.Add(adjacentElements[i]);
                    ClickCell(adjacentElements[i]);
                }
            }
        }
    }


    private void ClearLists()
    {
        columnsToFill.Clear();
        popCells.Clear();
        popBalloons.Clear();
    }

}
