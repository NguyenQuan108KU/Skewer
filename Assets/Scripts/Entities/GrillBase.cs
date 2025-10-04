

using UnityEngine;

public abstract class GrillBase : EntityBase
{
  public GrillType grillType;
  [SerializeField] protected SlotBase[] slots;
  protected bool showed = false;

  public virtual void SetLayer(LayerData layerData)
  {
    for (int i = 0; i < layerData.itemData.Length; i++)
    {
      if (layerData.itemData[i] != null && layerData.itemData[i].id > 0)
      {
        Item item = Instantiate(PrefabManager.Instance.itemPrefab, slots[i].transform).GetComponent<Item>();
        item.SetPrimary(entityType == EntityType.PrimaryGrill);
        item.SetItemData(layerData.itemData[i], slots[i]);
        slots[i].SetItem(item);
      }
      else
      {
        slots[i].ClearItem();
      }
    }
    showed = true;
  }

  // protected virtual Item CreateItem()
  // {

  // }
  public virtual void OnSlotUpdated(SlotBase slot)
  {
  }

  public SlotBase[] GetSlots()
  {
    return slots;
  }


  public virtual void Setup()
  {
  }

  public virtual void OnCreateObj(params object[] args)
  {
  }
  public abstract void ChangeItem(SlotBase slot, int newId);
  public abstract bool CheckItemWithId(int id);
  public abstract Vector3 DestroyItem(int id);
  public virtual void OnReturnObj()
  {
    for (int i = 0; i < slots.Length; i++)
    {
      slots[i].ClearItem();
    }

    showed = false;
  }
}
