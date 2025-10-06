

using System;
using UnityEngine;

public class OrderEntity : GrillBase
{
  [Header("Order Entity Visual")]
  [SerializeField] private Transform container;
  public override EntityType entityType => EntityType.PrimaryGrill;
  [SerializeField] private OrderEntityVisual orderEntityVisual;

  private int orderIndex;
  private bool active = false;
  private bool ready = false;

  private int itemIdTarget = 0;
  private int maxItems = 0;
  public int MaxItems => maxItems;

  public int ItemIdTarget => itemIdTarget;
  public bool IsActive => active;
  public int OrderIndex => orderIndex;
  public bool Ready => ready;

  public OrderEntityVisual Visual => orderEntityVisual;

  public override void ChangeItem(SlotBase slot, int newId)
  {

  }
  public override ShuffleLayerData GetShuffleLayerData()
  {
    return null;
  }
  public override bool CheckItemWithId(int id)
  {
    return false;
  }
  public override Vector3 DestroyItem(int id)
  {
    return Vector3.zero;
  }
  public void SetActive(bool active)
  {
    this.active = active;
    orderEntityVisual.SetActive(active);
  }
  public void SetData(int itemId, int num)
  {
    SetTargetItem(itemId, num);
  }

  public void SetOrderIndex(int index)
  {
    orderIndex = index;
  }
  public void SetReady(bool ready)
  {
    this.ready = ready;
  }
  public SlotBase GetAvailableSlot()
  {
    for (int i = 0; i < maxItems; i++)
    {
      var slot = slots[i];
      if (slot.GetItem() == null)
      {
        return slot;
      }
    }
    return null;
  }

  public void SetTargetItem(int itemId, int num)
  {
    itemIdTarget = itemId;
    maxItems = num;
    ShowTargetItem();
  }

  private void ShowTargetItem()
  {

    for (int i = 0; i < slots.Length; i++)
    {
      if (i >= maxItems)
      {
        var slot = slots[i];
        slot.ClearItem();
        continue;
      }
      else
      {
        var itemData = new ItemData()
        {
          itemType = ItemType.Normal,
          id = (int)itemIdTarget,
        };

        var item = Instantiate(PrefabManager.Instance.itemFaded).GetComponent<Item>();
        item.SetPrimary(entityType == EntityType.PrimaryGrill);
        item.SetItemData(itemData, slots[i]);
        slots[i].ClearItem();
        slots[i].SetItem(item);
        slots[i].SetItem(null);
      }
    }
  }
  public bool CheckComplete()
  {
    var slots = GetSlots();
    for (int i = 0; i < maxItems; i++)
    {
      var slot = slots[i];
      if (slot.GetItem() == null || slot.GetItem().IsSelected == true)
      {
        return false;
      }
    }
    return true; // đã bay tới vị trí slot
  }

  public void PlayComplete(Action onComplete)
  {
    foreach (var slot in slots)
    {
      slot.GetItem()?.OnComplete();
    }
    orderEntityVisual.PlayComplete(onComplete);
  }
}
