

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrimaryGrill : GrillBase
{
  public override EntityType entityType => EntityType.PrimaryGrill;
  [SerializeField] protected Transform container;
  [SerializeField] protected Transform subContainer;
  [SerializeField] protected GrillVisual grillVisual;
  [HideInInspector] public bool available;
  public GrillVisual GrillVisual => grillVisual;
  protected GrillData grillData;
  protected List<SubGrill> subGrills;
  protected bool isProcess = false;
  [HideInInspector] public bool isInGameplaySpace = true;
  protected Vector3 subOffset;
  public Action<PrimaryGrill> onMainLayerEmpty;
  public Action<PrimaryGrill> onMainLayerComplete;
  public Transform SubContainer => subContainer;

  public override void ChangeItem(SlotBase selectedSlot, int newId)
  {
    foreach (var slot in slots)
    {
      if (slot == selectedSlot)
      {
        Debug.Log($"ChangeItem: {slot.GetItem().id} {newId}");
        var item = slot.GetItem();
        var itemData = item.Data.Clone();
        itemData.id = newId;
        item.SetItemData(itemData, slot);
        return;
      }
    }

    if (subGrills != null)
    {
      foreach (var subGrill in subGrills)
      {
        subGrill.ChangeItem(selectedSlot, newId);
      }
    }
  }

  public virtual void AddFromSub(Item item, int slotIndex, int index)
  {

  }
  public override bool CheckItemWithId(int id)
  {
    return slots.Any(slot => slot.GetItem() != null && slot.GetItem().id == id);
  }

  public override Vector3 DestroyItem(int id)
  {
    Vector3 pos = transform.position;
    foreach (var slot in slots)
    {
      if (slot.GetItem() != null && slot.GetItem().id == id)
      {
        slot.GetItem().OnComplete();
        slot.ClearItem();
        pos = slot.transform.position;
        break;
      }
    }

    return pos;
  }
}
