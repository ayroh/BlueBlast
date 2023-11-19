using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public LevelData currentLevel { get; private set; }


    public void LoadLevel(string level)
    {
        string levelData;
        try
        {
            levelData = File.ReadAllText(Application.dataPath + "/Resources/level" + level + ".json");
            currentLevel = JsonConvert.DeserializeObject<LevelData>(levelData);
        }
        catch (Exception)
        {
            Debug.LogError("Level couldn't read, Random level coming!");

            currentLevel = new LevelData();

            List<CellType> cells = new List<CellType>();
            for (int i = 0;i < 81;++i) cells.Add(CellType.Empty);

            currentLevel.cells = cells;

            Dictionary<CellType, int> dict = new Dictionary<CellType, int>();
            dict.Add(CellType.CubePurple, 50);
            dict.Add(CellType.CubeRed, 50);

            currentLevel.goals = dict;
            currentLevel.level = "Backup";
            currentLevel.gridSize = new Size(9, 9);
            currentLevel.numberOfMoves = 80;
        }
    }

    public static void SaveLevel(LevelData data)
    {
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.dataPath + "/Resources/level" + data.level + ".json", json);
        Debug.Log("LevelSaved");
    }

    public CellType ConvertLevel(int x, int y)
    {
        if (currentLevel == null || x < 0 || y < 0 || x >= currentLevel.gridSize.x || y >= currentLevel.gridSize.y)
        {
            Debug.LogError("Cannot convert level");
            return CellType.Empty;
        }
        return currentLevel.cells[x * currentLevel.gridSize.x + y];
    }

}

public class LevelData
{
    public string level;
    public Size gridSize;
    public List<CellType> cells;
    public Dictionary<CellType, int> goals;
    public int numberOfMoves;
}

public struct Size
{
    public int x, y;

    public Size(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
