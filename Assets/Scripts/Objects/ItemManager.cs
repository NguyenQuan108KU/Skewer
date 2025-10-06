

using UnityEngine;

public class ItemManager : MonoBehaviour
{
  private int totalItems = 0;
  private int currentItems = 0;

  public int TotalItems => totalItems;
  public int CurrentItems => currentItems;
  public void OnStartCollectItem(OrderEntity orderEntity)
  {
    currentItems -= orderEntity.MaxItems;
  }

  public void Init()
  {
    var levelData = LevelGenerator.Instance.LevelData;
    totalItems = 0;
    currentItems = 0;
    foreach (var grillData in levelData.grillData)
    {
      if (grillData.layer != null)
      {
        foreach (var layerData in grillData.layer)
        {
          if (layerData.itemData != null)
          {
            foreach (var itemData in layerData.itemData)
            {
              if (itemData != null && itemData.id > 0)
              {
                totalItems++;
              }
            }
          }
        }
      }
    }

    GameLogicHandler.Instance.OnStartCollectItem += OnStartCollectItem;
    currentItems = totalItems;
  }

  public void Clear()
  {
    GameLogicHandler.Instance.OnStartCollectItem -= OnStartCollectItem;
    totalItems = 0;
    currentItems = 0;
  }
}
