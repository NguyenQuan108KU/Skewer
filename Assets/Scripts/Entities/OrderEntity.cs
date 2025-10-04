

using UnityEngine;

public class OrderEntity : GrillBase
{
  [Header("Order Entity Visual")]
  [SerializeField] private Transform container;
  public override EntityType entityType => EntityType.PrimaryGrill;
  [SerializeField] private OrderEntityVisual orderEntityVisual;

  private int orderIndex;
  private bool active = false;
  private bool ready = false;

  private int itemIdTarget = 0;
  private int maxItems = 0;
  public int MaxItems => maxItems;

  public int ItemIdTarget => itemIdTarget;
  public bool IsActive => active;
  public int OrderIndex => orderIndex;
  public bool Ready => ready;

  public OrderEntityVisual Visual => orderEntityVisual;

  public override void ChangeItem(SlotBase slot, int newId)
  {

  }

  public override bool CheckItemWithId(int id)
  {
    return false;
  }
  public override Vector3 DestroyItem(int id)
  {
    return Vector3.zero;
  }
}
