using UnityEngine;
using System.Collections;

public enum MotionDirection
{
    none,
    up,
    down,
    left,
    right,
}

/// <summary>
/// MotionData stores direction and duration of a motion
/// </summary>
public struct MotionData
{
    public MotionDirection direction;
    public float duration;

    public MotionData(MotionDirection initialDirection = MotionDirection.none, float initialDuration = 0.0f)
    {
        direction = initialDirection;
        duration = initialDuration;
    }
}
