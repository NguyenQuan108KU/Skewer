

using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;


public class LevelGenerator : SingletonBase<LevelGenerator>
{
  public TextAsset levelDataFile;
  private LevelData_SkewerJam _levelData = null;

  public LevelData_SkewerJam LevelData => _levelData;
  public GrillManager grillManager;


  public void GenerateLevel()
  {
    _levelData = JsonConvert.DeserializeObject<LevelData_SkewerJam>(levelDataFile.text);
  }


  private void CreateGrill(List<GrillData> listGrillData)
  {
    foreach (var grillData in listGrillData)
    {
      if (grillData.isLock) grillData.grillType = GrillType.Lock;
      var grillType = grillData.grillType.ValidateGrillType();
      var slotCount = grillData.SlotCount;

      var (validateGrillType, validateSlotCount) = ValidateGrillType(grillType, slotCount);
      if (validateGrillType == null) continue;
    }
  }

  private (GrillType? validateGrillType, int validateSlotCount) ValidateGrillType(GrillType grillType, int slotCount)
  {
    switch (grillType)
    {
      case GrillType.LockAds:
        return (null, 0);
      case GrillType.Vending:
        return (GrillType.Normal, 1);
    }

    return (grillType, slotCount);
  }
}
