

using DG.Tweening;
using UnityEngine;

public class ItemVisual : MonoBehaviour
{
  [SerializeField] protected Item item;
  [SerializeField] private SpriteRenderer spriteRenderer;


  public void SetVisual(int id)
  {
    ItemDataSO itemData = LevelGenerator.Instance.GetItemData(id);
    if (itemData != null)
    {
      spriteRenderer.sprite = itemData.sprite;
    }
  }

  public void SetPrimary(bool isPrimary)
  {
    // spriteRenderer.material = isPrimary ? GameResourceReference.Instance.itemMaterials[0] : GameResourceReference.Instance.itemMaterials[2];
  }


  public void OnSelected()
  {
    DOTween.Kill("ItemScale");
    transform.DOScale(1.1f, 0.15f).SetId("ItemScale");
    // transform.localScale = Vector3.one * 1.1f;
    // spriteRenderer.material = GameResourceReference.Instance.itemMaterials[1];
    spriteRenderer.sortingOrder = 100;
  }

  public void OnBeginDrag()
  {
    spriteRenderer.sortingOrder = 100;
  }

  public void OnDeselected()
  {
    transform.DOScale(1, 0.1f);
    // spriteRenderer.material = GameResourceReference.Instance.itemMaterials[0];
  }
  public void OnIntoSlot()
  {
    spriteRenderer.sortingOrder = 1;
  }
  public Sprite GetSprite()
  {
    return spriteRenderer.sprite;
  }
  public void SetSortingOrder(int sortingOrder)
  {
    // sortingGroup.sortingOrder = sortingOrder;
  }
}
