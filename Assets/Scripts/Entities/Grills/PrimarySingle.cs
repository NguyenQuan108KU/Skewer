

using UnityEngine;

public class PrimarySingle : PrimaryGrill
{

  [SerializeField] private CountLayer countLayer;
  public override void SetData(GrillData grillData)
  {
    base.SetData(grillData);
  }

  protected override void UpdateSubGrills()
  {
    countLayer.SetSubLayer();
    base.UpdateSubGrills();
  }
}
