using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MovementSO")]
public class MovementSO : ScriptableObject
{
    public AnimationCurve bounceCurve;
    public AnimationCurve createRocketCurve;
    public AnimationCurve goalXCurve;
    public AnimationCurve goalYCurve;

    public float gridFallInitialSpeed = 0.02f;
    public float gridFallAcceleration = 0.002f;
    public float gridFallBounceMultiplier = 2f;
    public float skyFallInitialSpeed = 0.01f;
    public float skyFallAcceleration = 0.001f;
    public float skyFallBounceMultiplier = 1f;
    public float bounceOffset = .05f;
    public float rocketSpeed = 0.02f;
    public float creationAnimation = .3f;
    public float goalAnimationTime = .5f;

}
