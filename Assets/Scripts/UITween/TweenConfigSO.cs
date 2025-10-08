using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Sonat/UI Tween/Tween Config")]
public class TweenConfigSO : ScriptableObject
{
    public TweenConfig config;
}

[Serializable]
public class TweenConfig
{
    public UITweenType tweenType;

    public float from;

    public float to = 1;
    public Vector3 mFrom;

    public Vector3 mTo;

    public float duration;
    public float delay;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    public TweenConfig Clone()
    {
        return new TweenConfig
        {
            tweenType = tweenType,
            from = from,
            to = to,
            duration = duration,
            delay = delay,
            mTo = mTo,
            mFrom = mFrom,
            curve = curve
        };
    }
}
