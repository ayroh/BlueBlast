using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : CellElement
{

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public override void Pop(bool doGoalAnimation = true)
    {
        if (doGoalAnimation)
        {
            ParticleManager.instance.Play(this);
            SoundManager.instance.Play(Sound.CubeExplode);
        }
        base.Pop(doGoalAnimation);
    }


}
