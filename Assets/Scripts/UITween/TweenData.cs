using System;
using UnityEngine;


[Serializable]
public class TweenData
{
    public Transform target;

    public TweenConfigSO configSO;

    public bool custom;

    public TweenConfig config;

    public Action OnCompleted;

    public void SetupData()
    {
        SetDataFromConfigSo();
    }

    private void OnConfigSOValueChanged()
    {
        custom = configSO == null;
        SetDataFromConfigSo();
    }

    private void SetDataFromConfigSo()
    {
        if (!custom && configSO != null) config = configSO.config.Clone();
    }

}

public enum UITweenType
{
    None,
    Scale,
    Fade,
    Move,
    LocalMove,
    RectLocalMove,
    FadeGroup,
    Active
}
