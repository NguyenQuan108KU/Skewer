

using UnityEngine;

public class WaitingGrill : GrillBase
{
  [SerializeField] private WaitingGrillVisual waitingGrillVisual;
  public WaitingGrillVisual Visual => waitingGrillVisual;
  private bool isActive = false;
  public bool IsActive => isActive;

  #region Implementations
  public override EntityType entityType => EntityType.PrimaryGrill;
  public override bool CheckItemWithId(int id)
  {
    return false;
  }

  public override Vector3 DestroyItem(int id)
  {
    return Vector3.zero;
  }
  public override void ChangeItem(global::SlotBase slot, int newId)
  {

  }

  public void SetActive(bool isActive)
  {
    this.isActive = isActive;
    waitingGrillVisual.SetActive(isActive);
  }
  public override ShuffleLayerData GetShuffleLayerData()
  {
    return null;
  }
  #endregion
}
