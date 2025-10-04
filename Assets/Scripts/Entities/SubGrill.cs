
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class SubGrill : GrillBase
{
  private PrimaryGrill primaryGrill;
  private LayerData layerData;
  [SerializeField] private SpriteRenderer visual;
  [SerializeField] private SortingGroup sortingGroup;

  public override EntityType entityType => EntityType.SubGrill;

  public void SetData(PrimaryGrill grill, LayerData layerData, int index)
  {
    this.layerData = layerData;
    primaryGrill = grill;
    // sortingGroup.sortingOrder = index + 2;
  }


  public void Show()
  {
    SetLayer(layerData);
  }

  public void MoveUpPrimary()
  {
    int itemIndex = 0;
    for (int i = 0; i < slots.Length; i++)
    {
      var item = slots[i].GetItem();
      if (item != null)
      {
        primaryGrill.AddFromSub(item, i, itemIndex);
        itemIndex++;
      }
    }

    visual.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(() => { Destroy(gameObject); });
  }
  public override Vector3 DestroyItem(int id)
  {

    return Vector3.zero;
  }

  public override bool CheckItemWithId(int id)
  {
    return true;
  }

  public override void ChangeItem(SlotBase slot, int newId)
  {

  }
}
