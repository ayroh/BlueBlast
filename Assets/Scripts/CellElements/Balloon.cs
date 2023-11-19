using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : CellElement
{


    public override void Pop(bool doGoalAnimation = true)
    {
        base.Pop(doGoalAnimation);
        SoundManager.instance.Play(Sound.BalloonPop);
    }

}
