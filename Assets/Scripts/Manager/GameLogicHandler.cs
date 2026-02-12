

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
  [SerializeField] private ConveyorManager conveyorManager;


  public GrillManager GrillManager => grillManager;
  public OrderManager OrderManager => orderManager;
  public WaitingGrillManager WaitingGrillManager => waitingGrillManager;
  public ItemManager ItemManager => itemManager;
  public ConveyorManager ConveyorManager => conveyorManager;

  public Item ItemSelected { get; set; }

  public event Action<Item, SlotBase> OnItemMoveSlot;
  public event Action<Item, bool> OnItemStartSwitchAndCheck;
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
          SwitchSlot(orderSlot, true);
          count++;
        }
      }
    }
    TryCheckLoseGame();
  }

  private void SwitchSlot(SlotBase slot, bool fromWaitingGrill)
  {
    if (ItemSelected == null) return;
    ItemSelected.SetLockState(true);
    ItemSelected.SwitchSlot(slot);
    OnItemStartSwitchAndCheck?.Invoke(ItemSelected, fromWaitingGrill);
    OnItemStartSwitch?.Invoke(ItemSelected, true);
  }

  public bool SelectItem(Item item)
  {
    if (CheckClickAndWarning(item) == true)
    {
      return false;
    }

    // item bay
    ItemSelected = item;
    bool isSwitchSuccess = false;

    var (order, slot) = orderManager.GetDestinationSlot(item);
    if (slot != null)
    {
      SwitchSlot(slot, false);
      isSwitchSuccess = true;
    }
    else
    {
      var (waitingGrill, waitingGrillSlot) = waitingGrillManager.GetDestinationSlot();
      if (waitingGrillSlot != null)
      {
        SwitchSlot(waitingGrillSlot, false);
        isSwitchSuccess = true;
      }
    }

    // sau khi item switch: Xem có cần warning không?
    if (isSwitchSuccess == true)
    {
      if (WaitingGrillHelper.CheckWarning() == true)
      {
        WaitingGrillHelper.Warning();
      }
    }

    return isSwitchSuccess;
  }
  private bool CheckClickAndWarning(Item item)
  {
    if (ItemHelper.CheckSelectedItemOnOrder(item) == false)
    {
      if (WaitingGrillHelper.IsWarning == true && WaitingGrillHelper.CheckWarningCount() == true)
      {
        if (WaitingGrillHelper.Warning() == true)
        {
          return true;
        }
      }

      WaitingGrillHelper.ResetWarning();
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
            GameplayController.Instance.GameOver(true);
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
            //GameController.Instance.Stuck(stuckType.Value);
            GameplayController.Instance.GameOver(false, stuckType);
    }
  }

  public StuckType? CheckLoseGame()
  {
    var listWaitingGrill = waitingGrillManager.ListWaitingGrills;
    foreach (var waitingGrill in listWaitingGrill)
    {
      if (waitingGrill.GetSlots().Where(e => e.isEmpty() && waitingGrill.IsActive).Count() > 0)
        return null;
    }

    // // nếu order còn có thể di chuyển item vào thì chưa thua
    // var listTargetItemIds = orderManager.GetTargetItemIds();
    // var listItemIdInLayer1 = GrillHelper.GetItemIdListWithLayer(1, true);
    // var dictItems = listItemIdInLayer1.GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count());
    // foreach (var id in listTargetItemIds)
    // {
    //   if (dictItems.ContainsKey(id) == true && dictItems[id] > 0) return null;
    // }
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
