using UnityEngine;

public abstract class ItemVisualSO : ScriptableObject
{
  public abstract void SetVisual(SpriteRenderer spriteRenderer, int id, bool isPrimary);
}
