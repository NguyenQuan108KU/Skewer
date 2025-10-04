


using DG.Tweening;
using UnityEngine;

public class SlotBase : MonoBehaviour
{
  [SerializeField] protected GrillBase grill;
  [SerializeField] protected Transform container;
  protected Item item;

  public virtual void ClearItem()
  {
    if (this.item != null)
    {
      Destroy(this.item.gameObject);
      this.item = null;
    }
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

  public void OnItemIntoSlot()
  {
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
}
