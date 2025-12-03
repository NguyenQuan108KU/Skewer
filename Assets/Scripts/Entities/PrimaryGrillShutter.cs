

using UnityEngine;

public class PrimaryGrillShutter : PrimaryGrill
{
  [SerializeField] private GrillVisualShutter visualShutter;
  private const int MoveToChangeState = 4;

  private bool isClosed = false;

  private int moveCount = 0;
  private bool useBooster = false;

  public override void SetData(GrillData grillData)
  {
    base.SetData(grillData);
    isClosed = grillData.isLock;
    visualShutter.SetUpShutter(isClosed);
    SetLockItems(isClosed);

    moveCount = isClosed ? 0 : 0;
    useBooster = false;
  }

  public void OnEnable()
  {
    grillBaseBehaviorSO.eventSystemSO.RegisterEvents_OnDropItem(OnItemDropped);
  }

  public void OnDisable()
  {
    grillBaseBehaviorSO.eventSystemSO.UnregisterEvents_OnDropItem(OnItemDropped);
  }
  private void OnItemDropped(Item item, bool fromWaitingGrill)
  {
    if (completed == true) return;
    if (fromWaitingGrill == true) return;

    useBooster = true;

    moveCount++;
    if (moveCount >= MoveToChangeState)
    {
      isClosed = !isClosed;
      SetLockItems(isClosed);
      visualShutter.SetUpShutter(isClosed);
      moveCount = 0;

    }

    if (GameplayController.Instance.gameState == GameState.Playing)
    {
      useBooster = false;
    }
  }

  public override bool CanShuffle()
  {
    return false;
  }
}
