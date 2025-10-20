

using UnityEngine;

public class ConveyorController : EntityBase
{
  public override EntityType entityType => EntityType.Conveyor;
  [SerializeField] protected MoveType moveType;

}
