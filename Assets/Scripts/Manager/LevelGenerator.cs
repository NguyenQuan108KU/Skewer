

using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using DG.Tweening;


public class LevelGenerator : SingletonBase<LevelGenerator>
{
  public TextAsset levelDataFile;
  private LevelData_SkewerJam _levelData = null;

  public LevelData_SkewerJam LevelData => _levelData;
  public GrillManager grillManager;
  public List<ItemDataSO> itemDataList;
  [SerializeField] private ItemSO itemSO;
  private void Start()
  {
    Init();
    GenerateLevel();
  }

  public void Init()
  {

  }
  public void GenerateLevel()
  {
    _levelData = JsonConvert.DeserializeObject<LevelData_SkewerJam>(levelDataFile.text);
    _levelData = ValidateLevelData(_levelData);

    GameLogicHandler.Instance.ItemManager.Init();
    CreateGrill(_levelData.grillData);

    var waitingGrillManager = GameLogicHandler.Instance.WaitingGrillManager;
    CreateWaitingGrill(_levelData.numberOfWaitingGrill, waitingGrillManager);

    var orderManager = GameLogicHandler.Instance.OrderManager;
    CreateOrder(_levelData.orderData, orderManager);

    DOVirtual.DelayedCall(1, () =>
    {
      GameplayController.Instance.isReady = true;
    });
  }


  private void CreateGrill(List<GrillData> listGrillData)
  {
    foreach (var grillData in listGrillData)
    {
      if (grillData.isLock) grillData.grillType = GrillType.Lock;
      var grillType = grillData.grillType.ValidateGrillType();
      var slotCount = grillData.SlotCount;

      var (validateGrillType, validateSlotCount) = ValidateGrillType(grillType, slotCount);
      if (validateGrillType == null) continue;

      PrimaryGrill grill = Instantiate(PrefabManager.Instance.GetPrimaryPrefab(grillType, validateSlotCount), grillManager.transform).GetComponent<PrimaryGrill>();
      grill.SetData(grillData);
      GameLogicHandler.Instance.GrillManager.AddGrill(grill);
    }
    CheckObstacles();
  }

  private void CreateWaitingGrill(int numberOfWaitingGrill, WaitingGrillManager waitingSlotManager)
  {
    waitingSlotManager.SetData(numberOfWaitingGrill);
  }
  private void CreateOrder(List<OrderData> listOrderData, OrderManager orderManager)
  {
    orderManager.SetData(listOrderData);
  }

  private void CheckObstacles()
  {
    PrimaryGrillIce.CheckAndStartProgress();
  }
  private (GrillType? validateGrillType, int validateSlotCount) ValidateGrillType(GrillType grillType, int slotCount)
  {
    switch (grillType)
    {
      case GrillType.LockAds:
        return (null, 0);
      case GrillType.Vending:
        return (GrillType.Normal, 1);
    }

    return (grillType, slotCount);
  }

  private LevelData_SkewerJam ValidateLevelData(LevelData_SkewerJam levelData)
  {
    var levelDataSkewerJam = levelData.CloneSkewerJam();
    levelDataSkewerJam.numberOfWaitingGrill = 5;

    // 
    levelDataSkewerJam.rescueCondition = new RescueCondition();
    levelDataSkewerJam.rescueCondition.maxNumberRescues = 5;
    levelDataSkewerJam.rescueCondition.maxRescueGap = 2;
    levelDataSkewerJam.rescueCondition.remainingWaitingGrill = 2;

    LogicOrderConfig logicOrderConfigs = new LogicOrderConfig();
    logicOrderConfigs.region = 1;
    logicOrderConfigs.minNumberSteps = 0;
    levelDataSkewerJam.logicOrderConfigs = new List<LogicOrderConfig>();
    levelDataSkewerJam.logicOrderConfigs.Add(logicOrderConfigs);
    return levelDataSkewerJam;
  }
  public ItemDataSO GetItemData(int id)
  {
    if (itemSO == null)
    {
      return itemDataList.Find(e => e.id == id);
    }
    return itemSO.GetItemData(id);
  }
}
