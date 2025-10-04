

using System.Collections;
using DarkTonic.PoolBoss;
using DG.Tweening;
using UnityEngine;

public class Item : EntityBase
{
  public override EntityType entityType => EntityType.Item;
  [HideInInspector] public bool isPrimary;
  [SerializeField] private float offsetY = 0.5f;

  [SerializeField] private ItemVisual visual;
  [SerializeField] private BoxCollider2D boxCollider;
  private ItemData data;
  public ItemData Data => data;
  private SlotBase slot;
  private ItemPlaceHolder placeHolder;
  private Vector3 offset;

  public void SetItemData(ItemData data, SlotBase slot)
  {
    this.data = data;
    this.slot = slot;
    if (data == null)
    {
      visual.gameObject.SetActive(false);
      id = 0;
    }
    else
    {
      id = data.id;
      visual.gameObject.SetActive(true);
      visual.SetVisual(data.id);
    }
  }
  public virtual void OnComplete()
  {
  }

  public void SetPrimary(bool isPrimary)
  {
    this.isPrimary = isPrimary;
    boxCollider.enabled = isPrimary;
    visual.SetPrimary(isPrimary);
    if (isPrimary)
    {
      StartCoroutine(IESpawnSmokeEffect());
    }
  }
  private bool _isSelected;
  private bool _isDragging;
  private Vector3 _mouseDownPos;
  public void OnHandleMouseDown()
  {
    // if (!isPrimary || _isSelected || GameplayController.Instance.gameState != GameState.Playing) return;
    _mouseDownPos = Input.mousePosition;
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = transform.position.z;
    offset = transform.position - mousePos;
    _isSelected = true;
    OnSelected();
  }
  private Vector3 vel;
  public void OnHandleMouseDrag()
  {
    if (!_isSelected) return;
    if (!_isDragging)
    {
      if (Vector3.Distance(Input.mousePosition, _mouseDownPos) > 0.2f)
      {
        OnItemBeginDrag();
        _isDragging = true;
      }
      else
      {
        return;
      }
    }
    Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    target.z = 0;
    target += offset;
    transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, Time.deltaTime * 2);
  }


  public void OnHandleMouseUp()
  {
    if (!_isSelected) return;
    if (_isDragging) OnItemFinishDrag();
  }

  IEnumerator IESpawnSmokeEffect()
  {
    while (gameObject.activeInHierarchy)
    {
      float time = Random.Range(3f, 8f);
      yield return new WaitForSeconds(time);
      int rand = Random.Range(0, 3);
      if (rand == 0)
      {
        PoolBoss.SpawnInPool("Food _ Smoke", transform.position, Quaternion.identity);
      }
    }
  }
  private void OnSelected()
  {
    visual.OnSelected();
    // GameplayController.Instance.SelectItem(this);
    // SoundManager.Instance.PlaySound(SoundType.ItemPick);
  }

  private void OnItemBeginDrag()
  {
    slot.OnItemDrag();
    visual.OnBeginDrag();
    // placeHolder = GameplayController.Instance.itemPlaceHolder;
    placeHolder.transform.SetParent(slot.transform);
    placeHolder.transform.localPosition = Vector3.zero;
    placeHolder.SetSprite(visual.GetSprite());
    placeHolder.gameObject.SetActive(true);
  }
  private void OnItemFinishDrag()
  {
    _isDragging = false;
    if (placeHolder != null)
    {
      placeHolder.gameObject.SetActive(false);
      placeHolder = null;
    }

    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Grill"));

    foreach (Collider2D collider in colliders)
    {
      // PrimaryGrill grill = collider.GetComponent<PrimaryGrill>();

      // if (grill != null)
      // {
      //   SlotBase availableSlot = grill.GetNearestSlot(transform.position);
      //   if (availableSlot != null && availableSlot != slot)
      //   {
      //     GameplayController.Instance.SwitchSlot(availableSlot);
      //     return;
      //   }
      // }
    }
    BackToSlot();
  }
  private void BackToSlot()
  {
    slot.AddItem(this);
    // transform.DOLocalMove(Vector3.zero, GameDefine.itemMoveBackSpeed).SetSpeedBased(true).OnComplete(() =>
    // {
    //   OnIntoSlot();
    //   SoundManager.Instance.PlaySound(SoundType.ItemPut);
    // });
  }

  public void OnDeSelected()
  {
    _isSelected = false;
    visual.OnDeselected();
    visual.OnIntoSlot();
  }

  public SlotBase GetSlot()
  {
    return slot;
  }


  public void SwitchSlot(SlotBase slot)
  {
    if (this.slot != null)
    {
      this.slot.ItemOut();
    }

    this.slot = slot;
    slot.AddItem(this);
    // transform.DOLocalMove(Vector3.zero, GameDefine.itemMoveBackSpeed).SetSpeedBased(true).OnComplete(() =>
    // {
    //   GameplayController.Instance.SelectItem(null);
    //   OnDropToSlot(slot);
    // });
  }

  private void OnDropToSlot(SlotBase slot)
  {
    OnIntoSlot();
    if (this.slot.GetItem() != null)
    {
      //SOUND Items_Put
      // SoundManager.Instance.PlaySound(SoundType.ItemPut);
    }
  }

  public void OnIntoSlot()
  {
    visual.OnIntoSlot();
    slot.OnItemIntoSlot();
  }

  public void MoveToPrimary(SlotBase slot, int index)
  {
    SetPrimary(true);
    if (this.slot != null)
    {
      this.slot.ItemOut();
    }
    this.slot = slot;
    slot.AddItem(this);
    float delay = 0.1f * index;
    transform.DOScale(1, 0.25f).SetDelay(delay).SetEase(Ease.OutBack);
    transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.InSine).SetDelay(delay).OnComplete(OnIntoSlot);
    transform.DOLocalRotate(Vector3.zero, 0.25f).SetDelay(delay).SetEase(Ease.Linear);
  }
  public ItemVisual GetVisual()
  {
    return visual;
  }
}
