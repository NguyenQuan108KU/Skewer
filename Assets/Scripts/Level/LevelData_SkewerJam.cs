

using System;
using UnityEngine;

[Serializable]
public class LevelData_SkewerJam : LevelData
{
  public int numberOfWaitingGrill;
  public RescueCondition rescueCondition;

  public LogicOrderConfig[] logicOrderConfigs;
}


[Serializable]
public class LogicOrderConfig
{
  public float region;
  public int minNumberSteps;
}
[Serializable]
public class RescueCondition
{
  public int maxNumberRescues;
  public int maxRescueGap;
  public int remainingWaitingGrill;
}
