
using System.Collections.Generic;
using UnityEngine;

public class GrillManager : MonoBehaviour
{
  private List<PrimaryGrill> listGrills = new List<PrimaryGrill>();
  public List<PrimaryGrill> ListGrills => listGrills;


  void Start()
  {
    // foreach (Transform child in transform)
    // {
    //   child.gameObject.SetActive(false);
    // }
  }

  public void Init()
  {

  }

  public void Clear()
  {
    foreach (var grill in listGrills)
    {
      Destroy(grill.gameObject);
    }
    listGrills.Clear();
  }

  public void AddGrill(PrimaryGrill grill)
  {
    listGrills.Add(grill);
  }

  public PrimaryGrill GetGrill(int id)
  {
    return listGrills.Find(e => e.id == id);
  }

  public List<PrimaryGrill> FindObstacleGrills(List<int> grillIds)
  {
    return listGrills.FindAll(e => grillIds.Contains(e.id));
  }

  public List<Item> GetItemsWithLayer(int layer = 1)
  {
    List<Item> items = new List<Item>();
    foreach (var primaryGrill in listGrills)
    {
      foreach (var slot in primaryGrill.GetSlots())
      {
        var item = slot.GetItem();
        if (item != null)
        {
          items.Add(item);
        }
      }
    }
    return items;
  }

  public bool CheckClearAllItems()
  {
    foreach (var grill in listGrills)
    {
      foreach (var slot in grill.GetSlots())
      {
        if (slot.GetItem() != null) return false;
      }
    }
    return true;
  }
}
