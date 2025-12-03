


using DG.Tweening;
using UnityEngine;

public class SlotBase : MonoBehaviour
{
  [SerializeField] protected GrillBase grill;
  [SerializeField] protected Transform container;
  public int id { get; private set; }
  [SerializeField] protected Item item;
  [HideInInspector] public bool isOnConveyor;
  public virtual void ClearItem()
  {
    if (this.item != null)
    {
      Destroy(this.item.gameObject);
      this.item = null;
    }
  }

  public void SetItemManually(Item item)
  {
    this.item = item;
  }
  public virtual void SetItem(Item item)
  {
    this.item = item;
    if (item != null)
    {
      item.transform.SetParent(container);
      item.transform.localPosition = Vector3.zero;
      if (item != null)
      {
        DOScaleItemIntro(item);
      }
    }
  }
  public void ItemOut()
  {
    this.item = null;
    grill.OnSlotUpdated(this);
  }
  protected virtual void DOScaleItemIntro(Item item)
  {
    item.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBack);
  }
  public void OnItemDrag()
  {
    item.transform.SetParent(null);
  }
  public bool isEmpty()
  {
    return item == null;
  }
  public void AddItem(Item item)
  {
    this.item = item;
    item.transform.SetParent(container);
  }

  public void SetId(int id)
  {
    this.id = id;
  }

  public void OnItemIntoSlot()
  {
    if (grill != null)
      grill.OnSlotUpdated(this);
  }
  public Item GetItem()
  {
    return item;
  }
  public GrillBase GetGrill()
  {
    return grill;
  }

  public void OnItemSelected()
  {
  }

  public void SetConveyor(bool isOn)
  {
    this.isOnConveyor = isOn;
    if (item != null)
    {
      item.SetIsOnConveyor(isOn);
    }
  }
}
