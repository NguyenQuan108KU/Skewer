

using DG.Tweening;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
  public Transform Hand;
  private Item suggestItem;
  public SuggestManager suggestManager;
  public GameObject textTut;
  protected virtual void Awake()
  {
    GameEvent.OnUserFirstTouch.AddListener(OnItemMoveSlot);
    DOVirtual.DelayedCall(1f, () =>
    {
      ShowHand();
    });
    }
    private void Start()
    {
        DOVirtual.DelayedCall(1.2f, () =>
        {
            textTut.SetActive(true);
            Hand.gameObject.SetActive(true);
        });
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(Hand.gameObject.activeSelf || textTut.gameObject.activeSelf)
            {
                textTut.SetActive(false);
                Hand.gameObject.SetActive(false);
            }
        }
    }
    public void ShowHand()
  {
    var item = GetSuggestItem();
    if (item != null)
    {
      textTut.SetActive(true);
      suggestItem = item;
      Hand.position = item.transform.position;
      Hand.gameObject.SetActive(true);
    }
  }
  protected virtual void OnItemMoveSlot()
  {
    ClearHand();
    if (suggestManager)
    {
      suggestManager.Init();
    }
    GameEvent.OnUserFirstTouch.RemoveListener(OnItemMoveSlot);
  }

  public void ClearHand()
  {
    Hand.gameObject.SetActive(false);
    textTut.SetActive(false);

    if (suggestItem != null)
    {
      suggestItem = null;
    }
  }
  public virtual Item GetSuggestItem()
  {
    var orderItemsDict = GameLogicHandler.Instance.OrderManager.GetOrderItems();
    var listItemsInGrillManager = GameLogicHandler.Instance.GrillManager.GetItemsWithLayerNotObstacle(1);

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
}
