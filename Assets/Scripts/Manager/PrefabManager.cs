
using UnityEngine;

public class PrefabManager : SingletonBase<PrefabManager>
{
  public GameObject itemPrefab;
  [Header("Grill")]
  public GameObject primaryGrillNormal3;
  public GameObject primaryGrillNormal1;
  public GameObject primaryGrillIce;
  public GameObject primaryGrillShutter;

  [Header("Sub grill")]
  public GameObject subGrillNormal;

  [Header("Order")]
  public GameObject orderEntity;
  public GameObject itemFaded;
  public GameObject waitingGrill;


  [Header("Conveyor")]
  public GameObject conveyorHorizontal;
  public GameObject conveyorVertical;
  public GameObject GetPrimaryPrefab(GrillType grillType, int validateSlotCount)
  {
    switch (grillType)
    {
      case GrillType.Normal:
        if (validateSlotCount == 1) return primaryGrillNormal1;
        else if (validateSlotCount == 3) return primaryGrillNormal3;
        break;

      case GrillType.Ice:
        return primaryGrillIce;
        break;
      case GrillType.Shutter:
        return primaryGrillShutter;
        break;
    }

    return new GameObject();
  }

  public GameObject GetConveyorPrefab(ConveyorType conveyorType)
  {
    switch (conveyorType)
    {
      case ConveyorType.Horizontal:
        return conveyorHorizontal;
      case ConveyorType.Vertical:
        return conveyorVertical;
    }
    return new GameObject();
  }
}
