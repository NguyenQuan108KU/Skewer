

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
  public static event EventHandler OnOrderDone;
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

    completeEffect?.Stop();
  }

  public void PlayComplete(Action onComplete)
  {
    StartCoroutine(IComplete(onComplete));
  }
  public void PlayCompleteLeft(Action onComplete)
  {
    StartCoroutine(ICompleteLeft(onComplete));
  }
    
  private IEnumerator IComplete(Action onComplete)
  {
        imageLid.gameObject.SetActive(true);
        imageLid.transform.localPosition = Vector3.up * 8;
        imageLid.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(downCurve);
        yield return new WaitForSeconds(0.3f);
        completeEffect.Play();
        // SOUND
        SoundManager.Instance.PlaySound(SoundType.ItemMerge);
        onComplete?.Invoke();
        OnOrderDone?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(0.4f);
        var orderEntity = transform.parent.GetComponent<OrderEntity>();
        orderEntity.MoveOut();
    }
  private IEnumerator ICompleteLeft(Action onComplete)
  {
        imageLid.gameObject.SetActive(true);
        imageLid.transform.localPosition = Vector3.up * 8;
        imageLid.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(downCurve);
        yield return new WaitForSeconds(0.3f);
        completeEffect.Play();
        // SOUND
        SoundManager.Instance.PlaySound(SoundType.ItemMerge);
        onComplete?.Invoke();
        OnOrderDone?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(0.4f);
        var orderEntity = transform.parent.GetComponent<OrderEntity>();
        orderEntity.MoveOutLeft();
    }
    //=====================Endless====================
    public void PlayEndlessClose()
    {
        StartCoroutine(IEndlessClose());
    }
    public void PlayEndlessMove(Action onComplete)
    {
        StartCoroutine(IEndlessMove(onComplete));
    }
    private IEnumerator IEndlessClose()
    {
        imageLid.gameObject.SetActive(true);
        imageLid.transform.localPosition = Vector3.up * 8;
        imageLid.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(downCurve);
        yield return new WaitForSeconds(0.3f);
        completeEffect.Play();
        // SOUND
        SoundManager.Instance.PlaySound(SoundType.ItemMerge);
        //onComplete?.Invoke();
        OnOrderDone?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(0.4f);
        var orderEntity = transform.parent.GetComponent<OrderEntity>();
    }
    private IEnumerator IEndlessMove(Action onComplete)
    {
        SoundManager.Instance.PlaySound(SoundType.ItemMerge);
        onComplete?.Invoke();
        OnOrderDone?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(0.4f);
        var orderEntity = transform.parent.GetComponent<OrderEntity>();
        orderEntity.MoveOutRight();
    }



    public virtual void OpenGrill(bool isNormal = true, bool doEffect = true)
  {
    var lid = isNormal ? imageLid : bonusLid;
    var ortherLid = isNormal ? bonusLid : imageLid;
    ortherLid.transform.DOKill();
    ortherLid.gameObject.SetActive(false);

    lid.transform.DOKill();

    if (doEffect)
    {
      lid.transform.localScale = Vector3.one;
      lid.gameObject.SetActive(true);
      lid.transform.DOLocalMoveY(2.5f, GameDefine.grillLidAnim).From(0.035f).SetEase(Ease.OutQuad);
      // lid.transform.DOScale(0.95f, GameDefine.grillLidAnim);
      lid.DOFade(0, GameDefine.grillLidAnim).SetEase(Ease.InQuad).OnComplete(() => { lid.gameObject.SetActive(false); });
    }
    else
    {
      lid.gameObject.SetActive(false);
    }
  }

}
