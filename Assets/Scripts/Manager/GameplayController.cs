

using UnityEngine;

public class GameplayController : SingletonBase<GameplayController>
{
  public LayerMask layerMaskItem;

  private Item itemTmp = null;
  private void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      OnHandleMouseDown(Input.mousePosition);
    }
    if (Input.GetMouseButtonUp(0))
    {
      OnHandleMouseUp();
    }
  }

  private void OnHandleMouseDown(Vector2 mousePos)
  {
    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(mousePos);
    var col = Physics2D.OverlapPoint(touchPosition, layerMaskItem);
    if (col)
    {
      col.transform.TryGetComponent<Item>(out itemTmp);
      if (itemTmp != null)
      {
        itemTmp.OnHandleMouseDown();
      }
    }
  }

  private void OnHandleMouseUp()
  {
    if (itemTmp == null) return;
    itemTmp.OnHandleMouseUp();
    itemTmp = null;
  }
}
