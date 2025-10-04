using UnityEngine;


public abstract class EntityBase : MonoBehaviour
{
  public abstract EntityType entityType { get; }
  public int id;
}
