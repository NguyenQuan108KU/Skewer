using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderManagerEndless : OrderManager
{
    public int completeThreshold = 0;

    private List<OrderEntity> _waitingComplete = new List<OrderEntity>();
    public List<GameObject> listShipper;
    public override void GameLogicHandler_OnItemMoveSlot(Item item, SlotBase slot)
    {
        if (slot.GetGrill() is OrderEntity orderEntity)
        {
            if (_listOrders.Contains(orderEntity))
            {
                if (orderEntity.CheckComplete())
                {
                    if (!_waitingComplete.Contains(orderEntity))
                    {
                        _waitingComplete.Add(orderEntity);
                        completeThreshold++;
                    }
                    orderEntity.CloseLib();
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        orderEntity.tick.SetActive(true);
                    });
                    // Chưa đủ 2 thì chờ
                    if (completeThreshold < 2)
                        return;

                    HandleDoubleComplete();
                }
                else
                {
                    GameLogicHandler.Instance.TryCheckLoseGame();
                }
            }
        }
    }

    private void HandleDoubleComplete()
    {
        var completedOrders = new List<OrderEntity>(_waitingComplete);

        foreach (var order in completedOrders)
        {
            _listOrders.Remove(order);
        }

        var newOrders = new List<OrderEntity>();
        
        foreach (var order in completedOrders)
        {
            order.PlayOrderComplete(() =>
            {
                GameLogicHandler.Instance.CompleteCollectItem(order);
            });

            GameLogicHandler.Instance.CollectItem(order);

            // Tạo order mới nhưng chưa cho move
            int itemId, num;

            if (dataOrders != null && dataOrders.Count > 0)
            {
                itemId = dataOrders[0].itemId;
                num = dataOrders[0].num;
                dataOrders.RemoveAt(0);
            }
            else
            {
                (itemId, num) = OrderHelper.GetItemOrder();
            }
            var newOrder = CreateNextOrder(itemId, num);
            newOrder.SetOrderIndex(order.OrderIndex);
            newOrder.ready = true;
            // Đặt tất cả ở cùng vị trí start
            var startPos = LeftStartPos.position;
            startPos.z = 0;
            startPos.y = transform.position.y;
            newOrder.transform.position = startPos;

            newOrders.Add(newOrder);
        }
        // ✅ GÁN SHIPPER ĐẦU TIÊN CHO ORDER MỚI ĐẦU TIÊN
        if (newOrders.Count > 0 && listShipper != null && listShipper.Count > 0)
        {
            var firstOrder = newOrders[0];
            var firstShipper = listShipper[0];
            firstShipper.transform.SetParent(firstOrder.transform, false);
            firstShipper.transform.localPosition = new Vector3(-4.4f, 0.1f, 0);
            listShipper.RemoveAt(0);
        }

        // Sau khi tạo xong hết → move cùng lúc
        DOVirtual.DelayedCall(1f, () =>
        {
            foreach (var order in newOrders)
            {
                order.transform.DOLocalMove(
                    _listOrderLocalPositions[order.OrderIndex],
                    0.3f
                ).SetEase(Ease.OutSine);
            }

            AlignObjects();
            DOVirtual.DelayedCall(0.5f, () =>
            {
                foreach (var order in newOrders)
                {
                    GameLogicHandler.Instance.AppearNextOrder(order, true);
                }
            });
        });

        completeThreshold = 0;
        _waitingComplete.Clear();
    }
    public override void AlignObjects()
    {
        StartCoroutine(IAlignObjects());
    }
    public override IEnumerator IAlignObjects()
    {
        yield return new WaitForSeconds(delayAlignOrders);
        var start = -(_listOrders.Count - 1) * distance / 2;
        var sortedOrders = _listOrders.OrderBy(e => e.OrderIndex).ToList();
        var listLocalTargetPositions = new List<Vector3>();
        for (int i = 0; i < sortedOrders.Count; i++)
        {
            listLocalTargetPositions.Add(new Vector3(start + distance * i, 0, 0));

            var orderEntity = sortedOrders[i];
            orderEntity.transform.DOLocalMove(listLocalTargetPositions[i], 0.1f).SetEase(Ease.OutSine);
        }
    }
    public override void PlayAppearOrders()
    {
        DOVirtual.DelayedCall(.1f, () =>
        {
            for (int i = 0; i < _listOrders.Count; i++)
            {
                var order = _listOrders[i];
                order.transform.DOKill();

                // 👇 Offset sẵn theo index
                var startX = RightStartPos.localPosition.x + distance * i;
                var startPos = new Vector3(startX, 0, 0);

                order.transform.localPosition = startPos;
            }

            DOVirtual.DelayedCall(.05f, () =>
            {
                AppearOrdersSimultaneous();
            });
        });
            AssignShipperToFirstOrder();
    }
    private void AssignShipperToFirstOrder()
    {
        if (_listOrders.Count == 0) return;
        if (listShipper == null || listShipper.Count == 0) return;

        var firstOrder = _listOrders[0];
        var firstShipper = listShipper[0];

        firstShipper.transform.SetParent(firstOrder.transform, false);
        firstShipper.transform.localPosition = new Vector3(-4.4f, 0.1f, 0);
        listShipper.RemoveAt(0);
    }
    private void AppearOrdersSimultaneous()
    {
        foreach (var order in _listOrders)
        {
            order.transform.DOLocalMove(
                _listOrderLocalPositions[order.OrderIndex],
                1f
            ).SetEase(Ease.OutCubic);

            GameLogicHandler.Instance.AppearNextOrder(order, true);
        }

        SoundManager.Instance.PlaySound(SoundType.BoxAppear);
    }
}