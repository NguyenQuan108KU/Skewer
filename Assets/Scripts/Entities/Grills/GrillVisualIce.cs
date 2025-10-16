

using UnityEngine;

public class GrillVisualIce : GrillVisual
{
  [SerializeField] private SpriteRenderer[] iceStates;
  [SerializeField] private ParticleSystem[] iceEffects;
  private int currentIceState = 0;

  protected override void SetVisual()
  {
    base.SetVisual();
    currentIceState = 0;
  }
}
