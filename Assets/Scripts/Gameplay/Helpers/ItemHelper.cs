using System.Collections.Generic;
using System.Linq;

public static class ItemHelper
{
    public static Dictionary<int, int> GetItemIdDictInGameplay(int layer)
    {
        var grillManager = GameLogicHandler.Instance.GrillManager;
        var waitingGrillManager = GameLogicHandler.Instance.WaitingGrillManager;

        var listItemIds = GrillHelper.GetItemIdListWithLayer(layer);
        foreach (var waitingGrill in waitingGrillManager.ListWaitingGrills)
        {
            var slot = waitingGrill.GetSlots()[0];
            var item = slot.GetItem();
            if (item != null)
            {
                listItemIds.Add(item.id);
            }
        }

        return listItemIds.GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count());
    }
}
