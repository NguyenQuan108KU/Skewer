using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManagerBeltAndBox : OrderManager
{
    public override void Init()
    {
        int column = 4; // 4 cột
        int row = Mathf.CeilToInt((float)maxOrder / column);

        float spacingX = 2.6f;
        float spacingY = 2.2f;

        float startX = -(column - 1) * spacingX / 2f;
        float startY = (row - 1) * spacingY / 2f;

        for (int i = 0; i < maxOrder; i++)
        {
            int col = i % column;
            int r = i / column;

            float x = startX + col * spacingX;
            float y = startY - r * spacingY;

            _listOrderLocalPositions.Add(new Vector3(x, y, 0));
        }

        GameLogicHandler.Instance.OnItemMoveSlot += GameLogicHandler_OnItemMoveSlot;

        var preplaced = GetComponentsInChildren<OrderEntity>(true);
        if (preplaced != null && preplaced.Length > 0)
        {
            _listOrders.Clear();
            foreach (var oe in preplaced)
            {
                if (!_listOrders.Contains(oe))
                {
                    _listOrders.Add(oe);
                    // ensure visual state matches active/locked defaults
                    oe.Visual.SetNormalOrder(true);
                    oe.SetActive(true);
                }
            }
        }
    }
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
                    orderEntity.CloseLib();
                    GameLogicHandler.Instance.CompleteCollectItem(orderEntity);
                        if (checkNextOrder == false)
                        {
                            AlignObjects();
                        }
                    if (checkNextOrder)
                    {
                        //CreateNextOrder(orderIndex);
                    }
                    GameLogicHandler.Instance.CollectItem(orderEntity);
                }
                else
                {
                    GameLogicHandler.Instance.TryCheckLoseGame();
                }
            }
        }
    }
}
