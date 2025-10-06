


// create itemSO scriptable object
using UnityEngine;
using System;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "ItemSO", menuName = "CustomSO/ItemSO")]
public class ItemSO : ScriptableObject
{
  public List<ItemDataSO> itemDataList;

  public ItemDataSO GetItemData(int id)
  {
    foreach (var itemData in itemDataList)
    {
      if (itemData.id == id)
      {
        return itemData;
      }
    }
    return null;
  }

}


[Serializable]
public class ItemDataSO
{
  public int id;
  public Sprite sprite;
}
