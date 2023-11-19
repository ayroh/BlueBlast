using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GoalManager : Singleton<GoalManager>
{
    [Header("Parents")]
    [SerializeField] private Transform goalTargetParent;
    [SerializeField] private Transform goalElementsParent;

    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private MovementSO movementSO;
    [SerializeField] private TextMeshProUGUI numberOfMovesText;

    private int _currentNumberOfMoves;
    public int currentNumberOfMoves
    {
        get { return _currentNumberOfMoves; }
        set { if (value >= 0) _currentNumberOfMoves = value; }
    }

    private List<Goal> goals = new List<Goal>();


    public void SetCurrentLevelGoals()
    {
        DeleteCurrentGoals();

        Dictionary<CellType, int> currentLevelGoals = LevelManager.instance.currentLevel.goals;
        if (currentLevelGoals == null || currentLevelGoals.Count == 0)
        {
            Debug.LogError("GoalManager SetCurrentLevelGoals: There aren't any goals for current level");
            return;
        }

        foreach (CellType key in currentLevelGoals.Keys)
        {
            AddGoal(key, currentLevelGoals[key]);
        }
    }

    public void AddGoal(CellType cellType, int count)
    {
        if (count < 1 || !IsGoalable(cellType))
        {
            Debug.LogError("GoalManager AddGoal: count is below one or Celltype is wrong");
            return;
        }
        if (goals.Count > 3)
        {
            Debug.LogError("GoalManager AddGoal: Cant add more than 4 goals");
            return;
        }


        Goal goal = PoolManager.instance.GetGoal(cellType);
        goal.transform.SetParent(goalTargetParent, false);
        goal.SetCount(count);
        goals.Add(goal);
    }

    public async void SendToGoal(CellElement cellElement, bool doAnimation)
    {
        if (!IsGoalable(cellElement.celltype))
            return;

        Goal targetGoal = goals.Find((goal) => goal.GetCellType() == cellElement.celltype);
        if (targetGoal == null)
            return;
        if (targetGoal.count > 0)
        {
            if (doAnimation)
            {
                Goal goal = PoolManager.instance.GetGoal(cellElement.celltype);
                goal.transform.SetParent(goalElementsParent, false);
                Vector3 pos = cellElement.transform.position;
                pos.z = 0f;
                goal.transform.position = pos;

                Sequence sequence = DOTween.Sequence();
                sequence.Join(goal.transform.DOMoveY(targetGoal.transform.position.y, NoiseTime(movementSO.goalAnimationTime)).SetEase(movementSO.goalYCurve));
                sequence.Join(goal.transform.DOMoveX(targetGoal.transform.position.x, NoiseTime(movementSO.goalAnimationTime)).SetEase(movementSO.goalXCurve));
                await sequence.AsyncWaitForCompletion();
                goal.Release();
            }
            SoundManager.instance.Play(Sound.CubeCollect);
            ParticleManager.instance.PlayStar(targetGoal.transform.position);
            if (targetGoal.DecrementCount() <= 0)
                CheckEnd();
        }
    }


    public void CheckEnd()
    {
        Goal endedGoals = goals.Find(goal => goal.count > 0);
        if (endedGoals != null && currentNumberOfMoves > 0)
            return;
        if (endedGoals == null)
            Debug.Log("SUCCESS!!!!");
        else
            Debug.Log("FAILURE!!!!!");

        InputManager.instance.SetInputState(false);
    }


    public int DecrementNumberOfMoves()
    {
        --currentNumberOfMoves;
        numberOfMovesText.text = currentNumberOfMoves.ToString();
        return currentNumberOfMoves;
    }

    public void SetNumberOfMoves()
    {
        currentNumberOfMoves = LevelManager.instance.currentLevel.numberOfMoves;
        numberOfMovesText.text = currentNumberOfMoves.ToString();
    }


    private void DeleteCurrentGoals()
    {
        for (int i = 0;i < goals.Count;i++)
        {
            PoolManager.instance.ReleaseGoal(goals[i]);
        }
        goals.Clear();
    }

    private float NoiseTime(float time, int percentage = 15)
    {
        float noise = (time * percentage) / 100;
        return time + Random.Range(-noise, noise);
    }

    private bool IsGoalable(CellType cellType) => !(cellType == CellType.Empty || cellType == CellType.RocketHorizontal || cellType == CellType.RocketVertical);

}
