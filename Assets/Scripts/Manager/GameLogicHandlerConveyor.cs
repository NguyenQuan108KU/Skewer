using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicHandlerConveyor : GameLogicHandler
{
    public GameObject xImage;
    public override void AppearNextOrder(OrderEntity orderEntity, bool startLevel = false)
    {
        orderEntity.SetReady(true);

        if (startLevel == false)
        {
            TriggerAppearNextOrder(orderEntity);
            TriggerAppearNextOrderItem(orderEntity.ItemIdTarget);
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
                    ItemSelected = item;
                    SwitchSlot(orderSlot, true);
                    count++;
            }
        }

        TryCheckLoseGame();
    }
    public override void SwitchSlot(SlotBase slot, bool fromWaitingGrill)
    {
        if (!slot.grill.isAccept)
        {
            ItemSelected?.UnSelect();
            SoundManager.Instance.PlaySound(SoundType.False);

            if (xImage != null)
            {
                GameObject x = Instantiate(xImage, ItemSelected.transform);
                x.transform.localPosition = new Vector3(0.1f, 0.125f, 0);
                x.transform.localScale = Vector3.zero;

                SpriteRenderer sr = x.GetComponent<SpriteRenderer>();

                // hiệu ứng pop
                x.transform.DOScale(0.18f, 0.25f).SetEase(Ease.OutBack);

                // bay lên nhẹ
                //x.transform.DOLocalMoveY(0.5f, 0.6f).SetRelative();

                // fade
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 0;
                    sr.color = c;

                    sr.DOFade(1f, 0.15f)
                      .OnComplete(() =>
                      {
                          sr.DOFade(0f, 0.4f)
                            .SetDelay(0.6f)
                            .OnComplete(() => Destroy(x));
                      });
                }
            }

            return;
        }

        base.SwitchSlot(slot, fromWaitingGrill);
    }
}
