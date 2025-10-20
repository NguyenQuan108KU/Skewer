
using UnityEngine;

public class PrefabManager : SingletonBase<PrefabManager>
{
  public GameObject itemPrefab;
  [Header("Grill")]
  public GameObject primaryGrillNormal3;
  public GameObject primaryGrillNormal1;
  public GameObject primaryGrillIce;

  [Header("Sub grill")]
  public GameObject subGrillNormal;

  [Header("Order")]
  public GameObject orderEntity;
  public GameObject itemFaded;
  public GameObject waitingGrill;
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
    }

    return new GameObject();
  }
}
