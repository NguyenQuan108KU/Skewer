


using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class LevelData
{
  public List<GrillData> grillData;
  public List<OrderData> orderData;
  public LevelDifficulty difficulty;
  public List<ConveyorData> conveyorData;
  public virtual LevelData Clone()
  {
    LevelData levelData = new LevelData();
    levelData.difficulty = this.difficulty;

    levelData.grillData = new List<GrillData>(grillData);
    if (conveyorData != null)
      levelData.conveyorData = new List<ConveyorData>(conveyorData);
    return levelData;
  }
}
[Serializable]
public class LayerData
{
  public ItemData[] itemData;

  public LayerData(int numberSlot)
  {
    itemData = new ItemData[numberSlot];
  }

  public bool ValidateData()
  {
    for (int i = 0; i < itemData.Length; i++)
    {
      if (itemData[i] != null && itemData[i].id > 0) return true;
    }

    return false;
  }
}

[Serializable]
public class ItemData
{
  public int id;
  public bool hidden;
  public ItemType itemType;

  public virtual ItemData Clone()
  {
    return new ItemData()
    {
      id = id,
      hidden = hidden,
      itemType = itemType,
    };
  }
}


public enum ItemType : byte
{
  Normal = 0,
  Hidden,
  Special,
  Bomb,
  Ice,
  Key,
  Key2,
  KeyArea
}
public enum GrillType
{
  Normal = 0,

  //Single = 1,
  //Drop = 2,
  //Drop7 = 3,
  Lock = 4,
  LockAds = 5,
  LockAndKey = 6,
  Ice = 7,
  Lid = 8,
  Vending = 9,
  LockAndKey2 = 10,

  // Drop7Lock,
  // Drop7LockAds,
  // Drop7LockAndKey,
  // Drop7LockAndKey2,
  // Drop7Ice,
  // Drop7Lid,
  // Drop5,
  // Drop6,
  SingleMin = 11,
  Spicy = 12,
  Broken = 13,
  Simple = 14,
  Shutter = 19,
}

[Serializable]
public class OrderData
{
  public int time;
  public int[] ids;
  public int appearCondition;
  public int[] order;
  public OrderItemData[] orderItem;
}
[Serializable]
public class GrillData
{
  public byte id;
  public int priority;
  public Vector3Data position;
  public List<LayerData> layer;
  public GrillType grillType;
  public bool isLock;
  public int slotCount;

  public int SlotCount
  {
    get
    {
      if (slotCount > 0) return slotCount;
      switch ((byte)grillType)
      {
        case 0:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 10:
          slotCount = 3;
          return 3;
        case 1:
          grillType = GrillType.Normal;
          slotCount = 1;
          return 1;
        case 9:
        case 11:
          slotCount = 1;
          return 1;
        case 3:
          slotCount = 7;
          grillType = GrillType.Normal;
          return 7;
        default:
          slotCount = 3;
          return 3;
      }
    }
  }

  public bool ValidateData()
  {
    if (layer == null || layer.Count == 0) return false;
    for (int i = layer.Count - 1; i >= 0; i--)
    {
      if (!layer[i].ValidateData())
      {
        layer.RemoveAt(i);
      }
    }

    return layer.Count > 0;
  }
}

[Serializable]
public class OrderItemData
{
  public int id;
  public int layer;
  public bool spicy;
}

[Serializable]
public class Vector3Data
{
  public float x;
  public float y;
  public float z;

  public Vector3Data(Vector3 v)
  {
    x = v.x;
    y = v.y;
    z = v.z;
  }

  public Vector3 ToVector3()
  {
    return new Vector3(x, y, z);
  }
}
[Serializable]
public class ConveyorData
{
  public int id;
  public ConveyorType conveyorType;
  public MoveType moveType = MoveType.Horizontal;
  public float speed;
  public Vector3Data position;
  public List<int> grillIds = new List<int>();
  public float space = 0.75f;
}
[Serializable]
public enum ConveyorType
{
  None = 0,
  Horizontal = 1,
  Vertical = 2,
  HorizontalMin = 3,
  HorizontalSimple = 4
}

[Serializable]
public enum MoveType
{
  None = 0,
  Horizontal = 1,
  Vertical = 2,
  Drop = 3
}
