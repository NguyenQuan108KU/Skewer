

using System.Collections;
using UnityEngine;

public class SuggestManager : MonoBehaviour
{
  [SerializeField] private float waitSuggestTime = 10f;
  private Item suggestItem;



  public void Init()
  {
    StartCoroutine(StartWaitSuggests());
    GameLogicHandler.Instance.OnItemMoveSlot += OnItemMoveSlot;
  }

  private void OnItemMoveSlot(Item item, SlotBase slot)
  {
    ClearSuggestItem();
  }

  public IEnumerator StartWaitSuggests()
  {
    yield return new WaitForSeconds(waitSuggestTime);
    yield return new WaitUntil(() => GameplayController.Instance.gameState == GameState.Playing && suggestItem == null);
    var item = GetSuggestItem();
    if (item != null)
    {
      suggestItem = item;
      item.SetSuggest(true);
    }
  }
  public Item GetSuggestItem()
  {
    var orderItemsDict = GameLogicHandler.Instance.OrderManager.GetOrderItems();
    var listItemsInGrillManager = GameLogicHandler.Instance.GrillManager.GetItemsWithLayer(1);

    // foreach (var (itemId, (maxItems, num)) in orderItemsDict)
    foreach (var orderItem in orderItemsDict)
    {
      var item = listItemsInGrillManager.Find(e => e.id == orderItem.targetItem);
      if (item != null)
      {
        return item;
      }
    }
    return null;
  }

  public void ClearSuggestItem()
  {
    Clear();
    StartCoroutine(StartWaitSuggests());
  }

  public void Clear()
  {
    StopAllCoroutines();
    if (suggestItem != null)
    {
      suggestItem.SetSuggest(false);
      suggestItem = null;
    }
  }
}
