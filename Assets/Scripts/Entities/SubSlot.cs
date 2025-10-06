

using DG.Tweening;

public class SubSlot : SlotBase
{
  protected override void DOScaleItemIntro(Item item)
  {
    item.transform.DOScale(0.55f, 0.5f).From(0).SetEase(Ease.OutBack);
  }
}
