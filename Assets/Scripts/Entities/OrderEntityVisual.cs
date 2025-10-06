

using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class OrderEntityVisual : MonoBehaviour
{
  [SerializeField] private GameObject normalOrder;
  [SerializeField] private GameObject bonusOrder;
  [SerializeField] private SpriteRenderer bonusLid;

  [SerializeField] private SpriteRenderer imageLid;
  [SerializeField] private ParticleSystem completeEffect;
  public AnimationCurve downCurve = AnimationCurve.Linear(0, 0, 1, 1);
  public AnimationCurve upCurve = AnimationCurve.Linear(0, 0, 1, 1);

  public void SetNormalOrder(bool isNormal)
  {
    normalOrder.gameObject.SetActive(isNormal);
    bonusOrder.gameObject.SetActive(!isNormal);
  }

  public void SetActive(bool isActive)
  {
    imageLid.gameObject.SetActive(false);
    // activeSprite.gameObject.SetActive(isActive);
    // inactiveSprite.gameObject.SetActive(!isActive);

    // completeEffect?.Stop();
  }

  public void PlayComplete(Action onComplete)
  {

    // Effect
    StartCoroutine(IComplete(onComplete));
  }

  private IEnumerator IComplete(Action onComplete)
  {
    imageLid.gameObject.SetActive(true);
    imageLid.transform.localPosition = Vector3.up * 8;
    imageLid.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(downCurve);
    yield return new WaitForSeconds(0.3f);
    // completeEffect.Play();
    // SOUND
    onComplete?.Invoke();

    yield return new WaitForSeconds(0.4f);
    var orderEntity = transform.parent.GetComponent<OrderEntity>();
    var targetPos = orderEntity.transform.localPosition + Vector3.up * 8;
    orderEntity.transform.DOLocalMove(targetPos, 0.3f).SetEase(upCurve);
    yield return new WaitForSeconds(0.3f);
    Destroy(orderEntity.gameObject);
  }
}
