using System;
using UnityEngine;


[CreateAssetMenu(fileName = "EventSystemSO_SkewerJam", menuName = "MyGame/SkewerJam/EventSystemSO_SkewerJam")]
public class EventSystemSO_SkewerJam : EventSystemSO
{
    public override void RegisterEvents_OnCollectItem(Action<int> onCollectItem)
    {
        GameLogicHandler.Instance.OnCollectItem += onCollectItem;
    }

    public override void UnregisterEvents_OnCollectItem(Action<int> onCollectItem)
    {
        GameLogicHandler.Instance.OnCollectItem -= onCollectItem;
    }

    public override void RegisterEvents_OnDropItem(Action<Item, bool> onDropItem)
    {
        GameLogicHandler.Instance.OnItemStartSwitch += onDropItem;
    }

    public override void UnregisterEvents_OnDropItem(Action<Item, bool> onDropItem)
    {
        GameLogicHandler.Instance.OnItemStartSwitch -= onDropItem;
    }
}
