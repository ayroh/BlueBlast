#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    private string gridSizeString = "5";
    private static int gridSize = 5;

    private string movesString = "20";
    private static int moves = 20;

    private static Dictionary<CellType, Texture2D> cellTypeTextureDictionary = new Dictionary<CellType, Texture2D>();
    private static List<CellType> cellTypeList = new List<CellType>();
    private CellType activeCellType = CellType.Empty;

    private string cubeYellowGoalString;
    private string cubeRedGoalString;
    private string cubeBlueGoalString;
    private string cubeGreenGoalString;
    private string cubePurpleGoalString;
    private string balloonGoalString;
    private string duckGoalString;

    private int cubeYellowGoal;
    private int cubeRedGoal;
    private int cubeBlueGoal;
    private int cubeGreenGoal;
    private int cubePurpleGoal;
    private int balloonGoal;
    private int duckGoal;

    [MenuItem("Level/Level Editor")]
    public static void Init()
    {
        InitializeCellList();
        var wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("Level Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Size");
        GUILayout.Space(10);
        gridSizeString = EditorGUILayout.TextField("Grid Size: ", gridSizeString);
        GUILayout.Space(10);
        if (GUILayout.Button("Create Grid"))
        {
            CreateGrid();
        }
        LoadCellTextures();
        DrawCellTypeSelectionButtons();
        DrawGridButtons();
        DrawGoals();

        GUILayout.Label("Moves");
        GUILayout.Space(10);
        movesString = EditorGUILayout.TextField("Number of Moves: ", movesString);

        Repaint();
    }

    private void CreateGrid()
    {
        if (!Int32.TryParse(gridSizeString, out gridSize))
        {
            ShowError("Incorrect grid sizes are entered");
            return;
        }

        InitializeCellList();
    }

    private void DrawGridButtons()
    {
        if (cellTypeList.Count == 0)
            return;
        for (var i = 0;i < gridSize;i++)
        {
            GUILayout.BeginHorizontal();
            for (var j = 0;j < gridSize;j++)
            {
                var content = new GUIContent();
                if (cellTypeTextureDictionary.TryGetValue(cellTypeList[(gridSize - i - 1) + gridSize * j], out var value))
                {
                    content.image = value;
                }

                if (GUILayout.Button(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(40), GUILayout.Height(40)))
                {
                    if ((gridSize - i - 1) == 0 && activeCellType == CellType.Duck)
                    {
                        Debug.LogError("LevelEditor DrawGridButtons: Cant place duck to first row!");
                        continue;
                    }
                    cellTypeList[(gridSize - i - 1) + gridSize * j] = activeCellType;
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Clear Grid"))
        {
            InitializeCellList();
        }

        if (GUILayout.Button("Save Level Data"))
        {
            SaveLevelData();
        }
    }


    private void DrawCellTypeSelectionButtons()
    {
        GUILayout.Label("Cell Selection");
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("", GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(50), GUILayout.Height(50)))
        {
            activeCellType = CellType.Empty;
        }

        var content = new GUIContent();
        content.image = cellTypeTextureDictionary[CellType.CubeYellow];
        if (GUILayout.Button(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(50), GUILayout.Height(50)))
        {
            activeCellType = CellType.CubeYellow;
        }

        content.image = cellTypeTextureDictionary[CellType.CubeRed];
        if (GUILayout.Button(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(50), GUILayout.Height(50)))
        {
            activeCellType = CellType.CubeRed;
        }

        content.image = cellTypeTextureDictionary[CellType.CubeBlue];
        if (GUILayout.Button(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(50), GUILayout.Height(50)))
        {
            activeCellType = CellType.CubeBlue;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        content.image = cellTypeTextureDictionary[CellType.CubeGreen];
        if (GUILayout.Button(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(50), GUILayout.Height(50)))
        {
            activeCellType = CellType.CubeGreen;
        }

        content.image = cellTypeTextureDictionary[CellType.CubePurple];
        if (GUILayout.Button(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(50), GUILayout.Height(50)))
        {
            activeCellType = CellType.CubePurple;
        }

        content.image = cellTypeTextureDictionary[CellType.Balloon];
        if (GUILayout.Button(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(50), GUILayout.Height(50)))
        {
            activeCellType = CellType.Balloon;
        }

        content.image = cellTypeTextureDictionary[CellType.Duck];
        if (GUILayout.Button(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(50), GUILayout.Height(50)))
        {
            activeCellType = CellType.Duck;
        }

        GUILayout.EndHorizontal();
    }

    private void DrawGoals()
    {
        GUILayout.Space(10);
        GUILayout.Label("Goals");
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        var content = new GUIContent();
        content.image = cellTypeTextureDictionary[CellType.CubeYellow];
        GUILayout.BeginVertical();
        GUILayout.Label(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(30), GUILayout.Height(50));
        cubeYellowGoalString = EditorGUILayout.TextField("", cubeYellowGoalString, GUILayout.Width(30), GUILayout.Height(20));
        GUILayout.EndVertical();

        content.image = cellTypeTextureDictionary[CellType.CubeRed];
        GUILayout.BeginVertical();
        GUILayout.Label(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(30), GUILayout.Height(50));
        cubeRedGoalString = EditorGUILayout.TextField("", cubeRedGoalString, GUILayout.Width(30), GUILayout.Height(20));
        GUILayout.EndVertical();

        content.image = cellTypeTextureDictionary[CellType.CubeBlue];
        GUILayout.BeginVertical();
        GUILayout.Label(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(30), GUILayout.Height(50));
        cubeBlueGoalString = EditorGUILayout.TextField("", cubeBlueGoalString, GUILayout.Width(30), GUILayout.Height(20));
        GUILayout.EndVertical();


        content.image = cellTypeTextureDictionary[CellType.CubeGreen];
        GUILayout.BeginVertical();
        GUILayout.Label(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(30), GUILayout.Height(50));
        cubeGreenGoalString = EditorGUILayout.TextField("", cubeGreenGoalString, GUILayout.Width(30), GUILayout.Height(20));
        GUILayout.EndVertical();


        content.image = cellTypeTextureDictionary[CellType.CubePurple];
        GUILayout.BeginVertical();
        GUILayout.Label(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(30), GUILayout.Height(50));
        cubePurpleGoalString = EditorGUILayout.TextField("", cubePurpleGoalString, GUILayout.Width(30), GUILayout.Height(20));
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        content.image = cellTypeTextureDictionary[CellType.Balloon];
        GUILayout.BeginVertical();
        GUILayout.Label(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(30), GUILayout.Height(50));
        balloonGoalString = EditorGUILayout.TextField("", balloonGoalString, GUILayout.Width(30), GUILayout.Height(20));
        GUILayout.EndVertical();

        content.image = cellTypeTextureDictionary[CellType.Duck];
        GUILayout.BeginVertical();
        GUILayout.Label(content, GUILayout.MinWidth(5), GUILayout.MinHeight(40), GUILayout.Width(30), GUILayout.Height(50));
        duckGoalString = EditorGUILayout.TextField("", duckGoalString, GUILayout.Width(30), GUILayout.Height(20));
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private static void LoadCellTextures()
    {
        if (cellTypeTextureDictionary.Count != 0)
            return;
        cellTypeTextureDictionary.Add(CellType.CubeYellow, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/UI/cube_1.png", typeof(Texture2D)));
        cellTypeTextureDictionary.Add(CellType.CubeRed, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/UI/cube_2.png", typeof(Texture2D)));
        cellTypeTextureDictionary.Add(CellType.CubeBlue, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/UI/cube_3.png", typeof(Texture2D)));
        cellTypeTextureDictionary.Add(CellType.CubeGreen, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/UI/cube_4.png", typeof(Texture2D)));
        cellTypeTextureDictionary.Add(CellType.CubePurple, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/UI/cube_5.png", typeof(Texture2D)));
        cellTypeTextureDictionary.Add(CellType.Balloon, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/UI/balloon.png", typeof(Texture2D)));
        cellTypeTextureDictionary.Add(CellType.Duck, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/UI/duck.png", typeof(Texture2D)));
    }

    private static void InitializeCellList()
    {
        cellTypeList.Clear();
        for (var i = 0;i < gridSize;i++)
        {
            for (var j = 0;j < gridSize;j++)
            {
                cellTypeList.Add(CellType.Empty);
            }
        }
    }

    private void SaveLevelData()
    {
        var goalList = CreateGoals();
        if (goalList.Count == 0)
        {
            Debug.LogError("LevelEditor SaveLevelData: You didn't set any goals");
            return;
        }

        if (!SetMoves())
        {
            Debug.LogError("LevelEditor SaveLevelData: Number of moves is below one or incorrect entry");
            return;
        }


        var levelData = new LevelData
        {
            goals = goalList,
            cells = cellTypeList,
            level = "Test",
            gridSize = new Size(gridSize, gridSize),
            numberOfMoves = moves
        };
        LevelManager.SaveLevel(levelData);
    }

    private bool SetMoves()
    {
        if (Int32.TryParse(movesString, out moves))
        {
            if (moves > 0)
                return true;
        }
        return false;
    }

    private Dictionary<CellType, int> CreateGoals()
    {
        var goalList = new Dictionary<CellType, int>();
        if (Int32.TryParse(cubeYellowGoalString, out cubeYellowGoal))
        {
            if (cubeYellowGoal > 0)
                goalList.Add(CellType.CubeYellow, cubeYellowGoal);
        }

        if (Int32.TryParse(cubeRedGoalString, out cubeRedGoal))
        {
            if (cubeRedGoal > 0)
                goalList.Add(CellType.CubeRed, cubeRedGoal);
        }

        if (Int32.TryParse(cubeBlueGoalString, out cubeBlueGoal))
        {
            if (cubeBlueGoal > 0)
                goalList.Add(CellType.CubeBlue, cubeBlueGoal);
        }

        if (Int32.TryParse(cubeGreenGoalString, out cubeGreenGoal))
        {
            if (cubeGreenGoal > 0)
                goalList.Add(CellType.CubeGreen, cubeGreenGoal);
        }

        if (Int32.TryParse(cubePurpleGoalString, out cubePurpleGoal))
        {
            if (cubePurpleGoal > 0)
                goalList.Add(CellType.CubePurple, cubePurpleGoal);
        }

        if (Int32.TryParse(balloonGoalString, out balloonGoal))
        {
            if (balloonGoal > 0)
                goalList.Add(CellType.Balloon, balloonGoal);
        }

        if (Int32.TryParse(duckGoalString, out duckGoal))
        {
            if (duckGoal > 0)
                goalList.Add(CellType.Duck, duckGoal);
        }

        return goalList;
    }

    private void ShowError(string message)
    {
        Debug.LogWarning(message);
    }
}

#endif