

using DG.Tweening;
using UnityEngine;

public class OrderCharacter : OrderEntity
{
  [SerializeField] private SpriteRenderer characterSprite;
  private DataCharacter dataCharacter;

  private void Start()
  {
    DataCharacter dataCharacter = OrderManager.Instance.GetCurrentOrderVisual();
    this.dataCharacter = dataCharacter;
    characterSprite.sprite = dataCharacter.sprite;

  }

  public override void MoveOut()
  {
    var targetPos = transform.localPosition + Vector3.right * 30;
    transform.DOLocalMove(targetPos, 1.5f).SetEase(orderEntityVisual.upCurve)
    .OnComplete(() =>
    {
      Destroy(gameObject);
    });

  }
}
