using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Duck : CellElement
{
    public override async UniTask SetNewYPosition(float yPosition, bool fromSky)
    {
        await base.SetNewYPosition(yPosition, fromSky);

        if (state == CellElementState.Sliding)
        {
            return;
        }

        if (index.y == 0)
        {
            await UniTask.Delay(200);
            Pop();
            GridManager.instance.ReorganizeColumn(index.x);
            GridManager.instance.FillColumn(index.x);
        }
    }


    public override void Pop(bool doGoalAnimation = true)
    {
        base.Pop(doGoalAnimation);

        SoundManager.instance.Play(Sound.DuckPop);
    }
}

