

using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GrillVisual : MonoBehaviour
{
  [SerializeField] protected SpriteRenderer stove;
  [SerializeField] protected SpriteRenderer lid;
  [SerializeField] protected SortingGroup sortingGroup;
  [SerializeField] protected string lidType;
  [SerializeField] protected string stoveType;
  protected float defaultLidPos = 0.225f;

  protected virtual void Awake()
  {
    defaultLidPos = lid.transform.localPosition.y;
    if (sortingGroup != null) sortingGroup.enabled = false;
  }

  public virtual void UpdateSubGrill()
  {
  }
  public virtual void SetDefaultGrill(GrillData grillData)
  {
    SetVisual();
    lid.gameObject.SetActive(true);
    lid.transform.localPosition = new Vector3(0, defaultLidPos, 0);
    lid.transform.localScale = Vector3.one;
    lid.SetAlpha(1);

    UnHighlightGrill();
  }
  protected virtual void SetVisual()
  {
    var grillBase = GetComponentInParent<GrillBase>();
    // var grillVisualSO = grillBase.GrillBaseBehaviorSO.grillVisualSO;
    // grillVisualSO.SetGrillBaseVisual(this, stove, lid, stoveType, lidType);
  }

  public void UnHighlightGrill()
  {
    // stove.material = GameResourceReference.Instance.itemMaterials[0];
    // if (sortingGroup != null)
    // {
    //   sortingGroup.enabled = false;
    //   sortingGroup.sortingLayerName = "Object";
    // }
  }

  public virtual void OpenGrill(bool doEffect = true)
  {
    lid.transform.DOKill();

    if (doEffect)
    {
      lid.transform.localScale = Vector3.one;
      lid.gameObject.SetActive(true);
      lid.transform.DOLocalMoveY(2.5f, GameDefine.grillLidAnim).From(defaultLidPos).SetEase(Ease.OutQuad);
      lid.transform.DOScale(0.95f, GameDefine.grillLidAnim);
      lid.DOFade(0, GameDefine.grillLidAnim).SetEase(Ease.InQuad).OnComplete(() => { lid.gameObject.SetActive(false); });
    }
    else
    {
      lid.gameObject.SetActive(false);
    }
  }
}
