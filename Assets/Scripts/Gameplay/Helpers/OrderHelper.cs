

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class OrderHelper
{


  private static int rescueGap = 0;
  private static int currentNumberRescues = 0;

  private static int stepGap = 0;
  public static int maxStep2Gap = 3;

  public static bool forward = true;
  public static bool CheckCreateNextOrder()
  {
    var orderManager = GameLogicHandler.Instance.OrderManager;


    var dictAllItemIds = ItemHelper.GetItemIdDictInGameplay(-1);
    var orderItemsDict = orderManager.GetOrderItemsDict();

    // số item còn lại <= số item còn lại tạo order
    foreach (var id in dictAllItemIds.Keys)
    {
      if (orderItemsDict.ContainsKey(id) == true)
      {
        // Nếu item target + item còn lại cùng loại nó > maxItems thì cần tạo thêm order
        if (dictAllItemIds[id] + orderItemsDict[id].num > orderItemsDict[id].maxItems) return true;
      }
      else
      {
        // Nếu item còn lại không có trong target thì cần tạo order
        return true;
      }
    }
    return false;
  }
  private static (int itemId, int num, int step) GetItemOrderBasic(int minStep, GameplayInfo gameplayInfo)
  {
    var dictItems = gameplayInfo.dictNeededSlots;

    if (forward == true)
    {
      for (int numItems = 3; numItems >= 1; numItems--)
      {
        // chuyển về dict<numStep, List<itemId>>
        var dictSteps = new Dictionary<int, List<int>>();
        foreach (var item in dictItems)
        {
          if (item.Value.ContainsKey(numItems))
          {
            var step = item.Value[numItems];
            dictSteps.TryAdd(step, new List<int>());
            dictSteps[step].Add(item.Key);
          }
        }

        foreach (var curStep in dictSteps.Keys)
        {
          if (curStep >= minStep)
          {
            var filteredItems = dictSteps[curStep];
            if (filteredItems.Count == 0) continue;

            var randomItemId = filteredItems[UnityEngine.Random.Range(0, filteredItems.Count)];
            numItems = numItems > 3 ? 3 : numItems;
            return (randomItemId, numItems, curStep);
          }
        }
      }
    }
    else
    {
      var dictNeededSlots = gameplayInfo.dictNeededSlots;
      for (int step = minStep; step < 6; step++)
      {
        var randomItemIds = dictNeededSlots.Keys.Where(e => dictNeededSlots[e].ContainsValue(minStep)).ToList();
        if (randomItemIds.Count > 0)
        {
          var randomItemId = randomItemIds[UnityEngine.Random.Range(0, randomItemIds.Count)];
          var nums = dictNeededSlots[randomItemId].Keys.Where(e => dictNeededSlots[randomItemId][e] == minStep).ToList();
          var maxNum = nums.Max();
          return (randomItemId, maxNum, minStep);
        }
      }

    }


    return (0, 0, 0);
  }

  public static (int itemId, int num) GetItemOrder()
  {
    var levelData = LevelGenerator.Instance.LevelData;
    var rescueCondition = levelData.rescueCondition;
    var logicOrderConfigs = levelData.logicOrderConfigs;


    if (CheckUseRescue(rescueCondition))
    {
      rescueGap = rescueCondition.maxRescueGap;
      currentNumberRescues += 1;
      return GetItemOrderToRescue();
    }
    var grillManager = GameLogicHandler.Instance.GrillManager;
    var orderManager = GameLogicHandler.Instance.OrderManager;
    // debug json dictNeededSlots
    var logicOrderConfig = GetLogicOrderConfig(logicOrderConfigs);
    var gameplayInfo = GetGameplayInfoForOrder();
    var (itemId, num, step) = GetItemOrderBasic(logicOrderConfig.minNumberSteps, gameplayInfo);
    if (itemId != 0)
    {
      if (rescueGap > 0) rescueGap--;
      if ((step >= 2 && stepGap <= 0))
      {
        stepGap = maxStep2Gap;
        return (itemId, num);
      }

      if (step < 2)
      {
        stepGap -= 1;
        stepGap = Mathf.Min(stepGap, maxStep2Gap);
        return (itemId, num);
      }
    }
    stepGap -= 1;
    stepGap = Mathf.Min(stepGap, maxStep2Gap);
    var (itemId2, num2, step2) = ForceGetItemOrderBasic(gameplayInfo);
    if (step2 >= 2) stepGap = maxStep2Gap;
    return (itemId2, num2);
  }

  private static Dictionary<int, Dictionary<int, int>> GetDictNeededSlots()
  {

    var dp = new Dictionary<int, Dictionary<int, int>>();

    var waitingGrillManager = GameLogicHandler.Instance.WaitingGrillManager;
    foreach (var waitingGrill in waitingGrillManager.ListWaitingGrills)
    {
      var item = waitingGrill.GetSlots()[0].GetItem();
      if (item != null)
      {
        if (dp.ContainsKey(item.id) == false)
        {
          dp[item.id] = new Dictionary<int, int>() { { 1, 0 } };
        }
        else
        {
          var numInDp = dp[item.id].Count;
          if (numInDp < 3)
          {
            dp[item.id][numInDp + 1] = 0;
          }
        }
      }
    }

    var orderManager = GameLogicHandler.Instance.OrderManager;
    var orderItemsDict = orderManager.GetOrderItemsDict();
    var neededItemsForCurrentOrder = orderItemsDict.ToDictionary(e => e.Key, e => e.Value.maxItems - e.Value.num);

    // tính số slot trống cần tối đa để lấy ra item
    var grillManager = GameLogicHandler.Instance.GrillManager;
    var dictCountSlotByGrill = new Dictionary<int, int>(); // số slot mỗi grill
    var dictNumItemsInUpperLayer = new Dictionary<int, List<Item>>(); // số lượng item ở layer trên đó
    foreach (var primaryGrill in grillManager.ListGrills)
    {
      var slots = primaryGrill.GetSlots();
      if (slots == null) continue;

      var listCurrentItems = slots.Select(e => e.GetItem()).Where(e => e != null).ToList();
      var dictItems = listCurrentItems.GroupBy(e => e.id).ToDictionary(e => e.Key, e => e.Count());

      foreach (var itemId in dictItems.Keys)
      {
        var numItems = dictItems[itemId];
        // cần chọn item tối ưu cho order
        if (neededItemsForCurrentOrder.ContainsKey(itemId))
        {
          var neededItems = neededItemsForCurrentOrder[itemId];
          neededItemsForCurrentOrder[itemId] -= numItems;
          if (neededItemsForCurrentOrder[itemId] <= 0)
          {
            neededItemsForCurrentOrder.Remove(itemId);
          }

          numItems = numItems - neededItems;
        }
        if (numItems <= 0) continue;

        var numInDp = dp.ContainsKey(itemId) ? dp[itemId].Count : 0;
        dp.TryAdd(itemId, new Dictionary<int, int>());
        for (int j = 1; j <= numItems; j++)
        {
          if (numInDp + j > 3) break;
          dp[itemId].TryAdd(numInDp + j, 0);
        }
      }
      dictNumItemsInUpperLayer[primaryGrill.id] = listCurrentItems;
      dictCountSlotByGrill[primaryGrill.id] = listCurrentItems.Count;
    }

    var dictMinItemForOrder = new Dictionary<int, List<(List<Item>, int)>>(); // đếm các item (num, neededSlot)
    foreach (var primaryGrill in grillManager.ListGrills)
    {
      var subGrills = primaryGrill.GetSubGrills();
      if (subGrills == null || subGrills.Count == 0) continue;
      var slots = subGrills[0].GetSlots();
      if (slots == null) continue;

      var listCurrentItems = slots.Select(e => e.GetItem()).Where(e => e != null).ToList();
      var dictItems = listCurrentItems.GroupBy(e => e.id).ToDictionary(e => e.Key, e => e.Count());
      foreach (var itemId in dictItems.Keys)
      {
        if (neededItemsForCurrentOrder.ContainsKey(itemId))
        {
          dictMinItemForOrder.TryAdd(itemId, new List<(List<Item>, int)>());

          var items = listCurrentItems.Where(e => e.id == itemId).ToList();
          var neededSlot = dictCountSlotByGrill.GetValueOrDefault(primaryGrill.id, 0);
          dictMinItemForOrder[itemId].Add((items, neededSlot));
        }
      }
    }

    var listIgnoreItems = new List<Item>();
    foreach (var itemId in neededItemsForCurrentOrder.Keys)
    {
      if (dictMinItemForOrder.ContainsKey(itemId))
      {
        var neededItems = neededItemsForCurrentOrder[itemId];
        listIgnoreItems.AddRange(ChooseItemToIgnore(dictMinItemForOrder[itemId], neededItems));
      }
    }

    // duyệt lần 2 để tính toán
    foreach (var primaryGrill in grillManager.ListGrills)
    {
      var subGrills = primaryGrill.GetSubGrills();
      if (subGrills == null || subGrills.Count == 0) continue;
      var slots = subGrills[0].GetSlots();
      if (slots == null) continue;

      var listCurrentItems = slots.Select(e => e.GetItem()).Where(e => e != null).ToList();
      var remainItems = listCurrentItems.Where(e => listIgnoreItems.Contains(e) == false).ToList();
      var dictItems = remainItems.GroupBy(e => e.id).ToDictionary(e => e.Key, e => e.Count());

      foreach (var itemId in dictItems.Keys)
      {
        var numItems = dictItems[itemId];
        var currentNeededSlot = dictCountSlotByGrill.GetValueOrDefault(primaryGrill.id, 0);

        // nếu ở layer bên trên có item này thì trừ đi số lượng itme ở layer trên đó
        var numItemsInUpperLayer = dictNumItemsInUpperLayer.GetValueOrDefault(primaryGrill.id, new List<Item>()).Where(e => e.id == itemId).Count();
        currentNeededSlot = currentNeededSlot - numItemsInUpperLayer;

        if (dp.ContainsKey(itemId) == false)
        {
          dp[itemId] = new Dictionary<int, int>();
          for (int j = 1; j <= Mathf.Min(numItems, 3); j++)
          {
            dp[itemId].TryAdd(j, currentNeededSlot);
          }
        }
        else
        {
          var numInDp = dp[itemId].Count;
          for (int j = numInDp; j >= 1; j--)
          {
            var currentValue = dp[itemId][j] + currentNeededSlot;
            for (int k = 1; k <= numItems; k++)
            {
              if (j + k > 3) continue;
              dp[itemId].TryAdd(j + k, currentValue);
              dp[itemId][j + k] = Mathf.Min(dp[itemId][j + k], currentValue);
            }
          }
        }
      }
    }

    return dp;
  }
  private static List<Item> ChooseItemToIgnore(List<(List<Item>, int)> list, int neededItems)
  {
    // dp[i] = (số item đạt được, chi phí, danh sách item đã chọn)
    var dp = new (int maxItems, int minCost, List<Item> chosen)[neededItems + 1];

    for (int i = 0; i <= neededItems; i++)
    {
      dp[i].maxItems = 0;
      dp[i].minCost = int.MaxValue;
      dp[i].chosen = new List<Item>();
    }

    dp[0].minCost = 0;

    foreach (var (items, cost) in list)
    {
      int itemCount = items.Count;

      // duyệt ngược để không overwrite trạng thái cũ
      for (int cap = neededItems; cap >= 0; cap--)
      {
        if (dp[cap].minCost == int.MaxValue) continue;

        int newCap = cap + itemCount;
        if (newCap > neededItems) newCap = neededItems;

        int newItems = dp[cap].maxItems + itemCount;
        int newCost = dp[cap].minCost + cost;

        bool shouldUpdate = false;

        if (newItems > dp[newCap].maxItems) shouldUpdate = true;
        else if (newItems == dp[newCap].maxItems && newCost < dp[newCap].minCost) shouldUpdate = true;

        if (shouldUpdate)
        {
          dp[newCap].maxItems = newItems;
          dp[newCap].minCost = newCost;
          dp[newCap].chosen = dp[cap].chosen.Concat(items).ToList();
        }
      }
    }

    // tìm trạng thái tốt nhất
    var best = dp[0];
    for (int i = 1; i <= neededItems; i++)
    {
      if (dp[i].maxItems > best.maxItems ||
          (dp[i].maxItems == best.maxItems && dp[i].minCost < best.minCost))
      {
        best = dp[i];
      }
    }

    return best.chosen;
  }
  private static GameplayInfo GetGameplayInfoForOrder()
  {
    return new GameplayInfo() { dictNeededSlots = GetDictNeededSlots() };
  }
  private static bool CheckUseRescue(RescueCondition rescueCondition)
  {
    if (rescueGap > 0) return false;
    if (currentNumberRescues >= rescueCondition.maxNumberRescues) return false;

    var waitingGrillManager = GameLogicHandler.Instance.WaitingGrillManager;
    var maxWaitingGrill = waitingGrillManager.ListWaitingGrills.Where(e => e.IsActive).ToList().Count;
    var threshold = maxWaitingGrill - rescueCondition.remainingWaitingGrill;
    return waitingGrillManager.ListWaitingGrills.Where(e => e.GetSlots()[0].GetItem() != null).Count() >= threshold;
  }

  private static (int itemId, int num, int step) ForceGetItemOrderBasic(GameplayInfo gameplayInfo)
  {
    // lấy step nhỏ nhất
    var dictNeededSlots = gameplayInfo.dictNeededSlots;
    var minStep = dictNeededSlots.Values.Min(e => e.Values.Min());

    var randomItemIds = dictNeededSlots.Keys.Where(e => dictNeededSlots[e].ContainsValue(minStep)).ToList();
    var randomItemId = randomItemIds[UnityEngine.Random.Range(0, randomItemIds.Count)];
    var nums = dictNeededSlots[randomItemId].Keys.Where(e => dictNeededSlots[randomItemId][e] == minStep).ToList();
    var maxNum = nums.Max();
    return (randomItemId, maxNum, minStep);
  }


  private static (int itemId, int num) GetItemOrderToRescue()
  {
    // ---Tạo order để giải cứu---
    // Item được lấy từ hàng chờ --> Làm giảm số lượng khay trống nhiều nhất
    var waitingGrillManager = GameLogicHandler.Instance.WaitingGrillManager;

    var itemsInWaiting = waitingGrillManager.ListWaitingGrills.Select(e => e.GetSlots()[0].GetItem());
    var items = itemsInWaiting.Where(e => e != null).ToList();
    Debug.Log(items.Count);
    var dictItems = items.GroupBy(e => e.id).ToDictionary(e => e.Key, e => e.Count());
    var s = string.Join(", ", dictItems.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    var itemId = dictItems.OrderByDescending(e => e.Value).First().Key;
    var num = dictItems[itemId] > 3 ? 3 : dictItems[itemId];
    return ((int)itemId, num);
  }
  public class GameplayInfo
  {
    public Dictionary<int, Dictionary<int, int>> dictNeededSlots;
  }

  private static LogicOrderConfig GetLogicOrderConfig(List<LogicOrderConfig> logicOrderConfigs)
  {
    var itemManager = GameLogicHandler.Instance.ItemManager;
    var currentRegion = 1 - (float)itemManager.CurrentItems / itemManager.TotalItems;
    return logicOrderConfigs.FirstOrDefault(config => config.region >= currentRegion);
  }
}
