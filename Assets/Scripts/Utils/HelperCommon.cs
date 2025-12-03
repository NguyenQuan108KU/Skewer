


using System;

public static class HelperCommon
{
  public static GrillType ValidateGrillType(this GrillType grillType)
  {
    if (Enum.IsDefined(typeof(GrillType), grillType)) return grillType;
    switch ((int)grillType)
    {
      case 11:
        return GrillType.Lock;
      case 12:
        return GrillType.LockAds;
      case 13:
        return GrillType.LockAndKey;
      case 14:
        return GrillType.LockAndKey2;
      case 15:
        return GrillType.Ice;
      case 16:
        return GrillType.Lid;
      case 19:
        return GrillType.Shutter;
      default:
        return GrillType.Normal;
    }
  }
}
