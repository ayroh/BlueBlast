using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPhaseTrigger : MonoBehaviour
{
    public void NextLevelTrigger()
    {
        GridManager.instance.NextLevel();
    }

}
