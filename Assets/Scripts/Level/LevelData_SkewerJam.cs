

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData_SkewerJam : LevelData
{
  public int numberOfWaitingGrill;
  public RescueCondition rescueCondition;

  public List<LogicOrderConfig> logicOrderConfigs;
  public LevelData_SkewerJam CloneSkewerJam()
  {
    var levelData = base.Clone();

    var levelDataSkewerJam = new LevelData_SkewerJam()
    {
      difficulty = levelData.difficulty,

      grillData = levelData.grillData,
      orderData = levelData.orderData,
      conveyorData = levelData.conveyorData,
      numberOfWaitingGrill = this.numberOfWaitingGrill,
      rescueCondition = this.rescueCondition,
      logicOrderConfigs = this.logicOrderConfigs,
    };
    return levelDataSkewerJam;
  }
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
