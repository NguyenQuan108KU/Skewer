

using System;
using System.Linq;
using UnityEngine;

public class GameLogicHandler : SingletonBase<GameLogicHandler>
{
  [Header("Core")]
  [SerializeField] private GrillManager grillManager;
  [SerializeField] private OrderManager orderManager;
  [SerializeField] private WaitingGrillManager waitingGrillManager;
  [SerializeField] private ItemManager itemManager;


  public GrillManager GrillManager => grillManager;
  public OrderManager OrderManager => orderManager;
  public WaitingGrillManager WaitingGrillManager => waitingGrillManager;
  public ItemManager ItemManager => itemManager;

  public Item ItemSelected { get; set; }

  public event Action<Item, SlotBase> OnItemMoveSlot;
  public event Action<Item, bool> OnItemStartSwitch;
  public event Action<OrderEntity> OnAppearNextOrder;
  public event Action<int> OnAppearNextOrderItem;
  public event Action<int> OnCollectItem;
  public event Action<OrderEntity> OnStartCollectItem;
  public event Action<OrderEntity> OnCompleteCollectItem;

  private int pumpkin = 0;
  public int Pumpkin => pumpkin;
  public void AppearNextOrder(OrderEntity orderEntity, bool startLevel = false)
  {
    orderEntity.SetReady(true);
    if (startLevel == false)
    {
      OnAppearNextOrder?.Invoke(orderEntity);
      OnAppearNextOrderItem?.Invoke((int)orderEntity.ItemIdTarget);
    }

    int count = 0;
    foreach (var order in orderManager.ListOrders)
    {
      if (order.Ready == false || order.IsActive == false) continue;
      var targetItem = order.ItemIdTarget;
      foreach (var waitingGrill in waitingGrillManager.ListWaitingGrills)
      {
        var slot = waitingGrill.GetSlot(0);
        var item = slot.GetItem();
        var orderSlot = order.GetAvailableSlot();
        if (item != null && item.id == (int)targetItem && orderSlot != null)
        {
          ItemSelected = item;
          SwitchSlot(orderSlot);
          count++;
        }
      }
    }
  }

  private void SwitchSlot(SlotBase slot)
  {
    if (ItemSelected == null) return;
    ItemSelected.SetLockState(true);
    ItemSelected.SwitchSlot(slot);
    OnItemStartSwitch?.Invoke(ItemSelected, true);
  }

  public bool SelectItem(Item item)
  {
    ItemSelected = item;
    var (order, slot) = orderManager.GetDestinationSlot(item);
    if (slot != null)
    {
      SwitchSlot(slot);
      return true;
    }
    var (waitingGrill, waitingGrillSlot) = waitingGrillManager.GetDestinationSlot();
    if (waitingGrillSlot != null)
    {

      SwitchSlot(waitingGrillSlot);
      return true;
    }
    return false;
  }

  public void ItemMoveSlot(Item item, SlotBase slot)
  {
    OnItemMoveSlot?.Invoke(item, slot);
  }

  public void CollectItem(OrderEntity orderEntity)
  {
    OnCollectItem?.Invoke((int)orderEntity.ItemIdTarget);
    OnStartCollectItem?.Invoke(orderEntity);

    if (CheckWinGame())
    {
      // GameController.Instance.Win();
    }
  }

  public bool CheckWinGame()
  {
    if (grillManager.CheckClearAllItems() && waitingGrillManager.CheckClearAllItems()) return true;
    return false;
  }
  public void TryCheckLoseGame()
  {
    var stuckType = CheckLoseGame();
    if (stuckType != null)
    {
      // GameController.Instance.Stuck(stuckType.Value);
    }
  }

  public StuckType? CheckLoseGame()
  {
    var listWaitingGrill = waitingGrillManager.ListWaitingGrills;
    foreach (var waitingGrill in listWaitingGrill)
    {
      if (waitingGrill.GetSlots().Where(e => e.isEmpty() && waitingGrill.IsActive).Count() > 0) return null;
    }

    // nếu order còn có thể di chuyển item vào thì chưa thua
    var listTargetItemIds = orderManager.GetTargetItemIds();
    var listItemIdInLayer1 = GrillHelper.GetItemIdListWithLayer(1, true);
    foreach (var id in listTargetItemIds)
    {
      if (listItemIdInLayer1.Contains(id) == true) return null;
    }

    // else continue
    return StuckType.SkewerJam_OutOfSpace;
  }

  public void CompleteCollectItem(OrderEntity orderEntity)
  {
    OnCompleteCollectItem?.Invoke(orderEntity);
    pumpkin++;

    //Effect
  }
}
