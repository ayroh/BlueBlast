using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public MovementSO movementSO;
    [HideInInspector] public PlayerData playerData { get; private set; }
    public static GameState gameState { get; private set; }


    private List<CellElement> popCells = new List<CellElement>();
    private List<Balloon> popBalloons = new List<Balloon>();
    private List<int> columnsToFill = new List<int>();

    private CancellationTokenSource source = new CancellationTokenSource();
    private CancellationToken token;
    public CancellationToken GetCancellationToken() => token;

    private const string playerDataPath = "/Resources/Player/";

    private void Start()
    {
        token = source.Token;
        CheckPlayerDataPath();
    }

    int count = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("captured");
            ScreenCapture.CaptureScreenshot("C:\\Users\\Patates\\Desktop\\Capture" + (++count).ToString() + ".png");
        }
    }


    private void OnApplicationQuit()
    {
        source.Cancel();
    }

    public async void OnCellClickedAsync(Index index)
    {
        ClearLists();
        CellElement cellElement = GridManager.instance.GetGridElement(index);
        if (!GridManager.instance.CellAvailable(cellElement))
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
            case CellType.Sheep:
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
            if(GridManager.instance.CellAvailable(adjacentElements[i]))
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

    #region Player Data
    public void ReadPlayerData()
    {
        try
        {
            string playerDataString = File.ReadAllText(Application.persistentDataPath + playerDataPath + "Player.json");
            playerData = JsonConvert.DeserializeObject<PlayerData>(playerDataString);
        }
        catch (Exception)
        {
            //playerData = new PlayerData()
            //{
            //    name = "Temp",
            //    level = "1"
            //};
        }
    }

    public void IncrementPlayerDataLevel()
    {
        int currentLevel;
        if (Int32.TryParse(playerData.level, out currentLevel))
        {
            if(LevelManager.instance.numberOfLevels != currentLevel)
                playerData.level = (++currentLevel).ToString();
        }
    }

    public void SavePlayerData(PlayerData data)
    {
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.persistentDataPath + playerDataPath + "Player.json", json);
        //Debug.Log("PlayerSaved");
    }

    public void SetNewPlayer(PlayerData newPlayerData) => playerData = newPlayerData;

    private void CheckPlayerDataPath()
    {

        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/Resources");
        if (!dir.Exists)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Resources");
        }


        dir = new DirectoryInfo(Application.persistentDataPath + "/Resources/Player");
        if (!dir.Exists)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Resources/Player");
        }
    }



    #endregion

    private void ClearLists()
    {
        columnsToFill.Clear();
        popCells.Clear();
        popBalloons.Clear();
    }

    public void SetState(GameState newState)
    {
        gameState = newState;
    }

}


public class PlayerData
{
    public string name;
    public string level;
}

public enum GameState
{
    Menu,
    Started,
    LoadingNextLevel
}