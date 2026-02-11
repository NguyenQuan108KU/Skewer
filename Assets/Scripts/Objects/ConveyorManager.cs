

using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
  public List<ConveyorController> conveyors = new List<ConveyorController>();

  public void Init()
  {

  }

  public void Clear()
  {
    foreach (var conveyor in conveyors)
    {
      Destroy(conveyor.gameObject);
    }
    conveyors.Clear();
  }


  public void AddConveyor(ConveyorController conveyor)
  {
    conveyors.Add(conveyor);
  }


  public List<ConveyorData> GetConveyorData()
  {
    return LevelGenerator.Instance.LevelData.conveyorData;

  }
}
