using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelManager : Singleton<LevelManager>
{
    public LevelData currentLevel { get; private set; }

    private const string levelPath = "/Resources/Levels";

    public int numberOfLevels;
[System.Serializable]
public class Word
{
    public string word, description;
    public Word(string tempWord, string tempDescription)
    {
        word = tempWord;
        description = tempDescription;
    }
}
    private void Start()
    {
        CountNumberOfLevels();
    }

    public void LoadLevel(string level = default)
    {
        if(level == default)
            level = GameManager.instance.playerData.level;

        string levelData;
        try
        {
            levelData = Resources.Load<TextAsset>("Levels/level" + level).text;
            currentLevel = JsonConvert.DeserializeObject<LevelData>(levelData);
        }
        catch (Exception)
        {
            Debug.LogError("Level couldn't read, Random level coming!");

            currentLevel = new LevelData();

            List<CellType> cells = new List<CellType>();
            for (int i = 0;i < 81;++i) cells.Add(CellType.Empty);

            currentLevel.cells = cells;

            Dictionary<CellType, int> dict = new Dictionary<CellType, int>()
            {
                {CellType.CubePurple, 50 },
                {CellType.CubeRed, 50 }
            };

            currentLevel.goals = dict;
            currentLevel.level = "Backup";
            currentLevel.gridSize = new Size(9, 9);
            currentLevel.numberOfMoves = 80;
            currentLevel.dropables = new List<CellType> { CellType.CubeBlue, CellType.CubeRed, CellType.CubeYellow, CellType.CubePurple, CellType.CubeGreen };
        }
    }

    
    public static void SaveLevel(LevelData data)
    {
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.persistentDataPath + levelPath + "/level" + data.level + ".json", json);
        Debug.Log("LevelSaved");
    }

    private void CountNumberOfLevels()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + levelPath);
        numberOfLevels = dir.GetFiles("*.meta").Length;
    }

    public CellType ConvertLevel(Index index)
    {
        if (currentLevel == null || index.x < 0 || index.y < 0 || index.x >= currentLevel.gridSize.x || index.y >= currentLevel.gridSize.y)
        {
            Debug.LogError("Cannot convert level");
            return CellType.Empty;
        }
        return currentLevel.cells[index.x * currentLevel.gridSize.x + index.y];
    }

}



public class LevelData
{
    public string level;
    public Size gridSize;
    public List<CellType> cells;
    public Dictionary<CellType, int> goals;
    public int numberOfMoves;
    public List<CellType> dropables;
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
