using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager2Levels : OrderManager
{
    [SerializeField] public List<DataOrder> dataOrrderLevels2 = new List<DataOrder>();
    public override void GameLogicHandler_OnItemMoveSlot(Item item, SlotBase slot)
    {
        if (slot.GetGrill() is OrderEntity orderEntity)
        {
            if (_listOrders.Contains(orderEntity))
            {
                if (orderEntity.CheckComplete())
                {
                    var orderIndex = orderEntity.OrderIndex;
                    _listOrders.Remove(orderEntity);
                    var checkNextOrder = OrderHelper.CheckCreateNextOrder();
                    orderEntity.PlayComplete(() =>
                    {

                        GameLogicHandler.Instance.CompleteCollectItem(orderEntity);
                        if (checkNextOrder == false)
                        {
                            AlignObjects();
                        }
                    });
                    if (checkNextOrder)
                    {
                        CreateNextOrder(orderIndex);
                    }
                    GameLogicHandler.Instance.CollectItem2Levels(orderEntity);
                }
                else
                {
                    GameLogicHandler.Instance.TryCheckLoseGame();
                }
            }
        }
    }
}
