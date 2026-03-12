using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderEaster : OrderManager
{
    public List<GameObject> characters = new List<GameObject>();

    public override void PlayAppearOrders()
    {
        base.PlayAppearOrders();

        for (int i = 0; i < ListOrders.Count; i++)
        {
            AttachCharacterToOrder(i);
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
                    _listOrders.Remove(orderEntity);

                    var checkNextOrder = OrderHelper.CheckCreateNextOrder();

                    orderEntity.PlayCompleteLeft(() =>
                    {
                        GameLogicHandler.Instance.CompleteCollectItem(orderEntity);
                    });
                    for (int i = 0; i < _listOrders.Count; i++)
                    {
                        _listOrders[i].SetOrderIndex(i);
                    }
                    if (checkNextOrder)
                    {
                        CreateNextOrder(_listOrders.Count);
                        AttachCharacterToOrder(_listOrders.Count - 1);
                    }
                    AlignObjects();

                    GameLogicHandler.Instance.CollectItem(orderEntity);
                }
                else
                {
                    GameLogicHandler.Instance.TryCheckLoseGame();
                }
            }
        }
    }
    public void AttachCharacterToOrder(int index)
    {
        if (characters.Count == 0 || index >= ListOrders.Count)
            return;

        GameObject charObj = characters[0];   // luôn lấy character đầu

        characters.RemoveAt(0);               // xóa khỏi list

        charObj.transform.SetParent(ListOrders[index].transform);
        charObj.transform.localPosition = new Vector3(0, -4f, 0);
        charObj.transform.localRotation = Quaternion.identity;

        charObj.SetActive(true);
    }
}