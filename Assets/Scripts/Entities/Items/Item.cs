

using System.Collections;
using DarkTonic.PoolBoss;
using DG.Tweening;
using UnityEngine;

public class Item : EntityBase
{
  public override EntityType entityType => EntityType.Item;

  public ItemType itemType;
  [HideInInspector] public bool isPrimary;

  [SerializeField] private ItemVisual visual;
  [SerializeField] private BoxCollider2D boxCollider;

  [Header("Behavior SO")]
  [SerializeField] protected ItemBehaviorSO itemBehaviorSO;

  public ItemBehaviorSO ItemBehaviorSO => itemBehaviorSO;

  private ItemData data;
  public ItemData Data => data;
  private SlotBase slot;
  private ItemPlaceHolder placeHolder;
  public ItemVisual Visual => visual;
  public SlotBase Slot => slot;
  protected bool locked;
  public bool IsLocked => locked;
    public bool isCheck;
  public bool IsSelected { get => _isSelected; set => _isSelected = value; }
  protected bool isProcessing;
  Coroutine smokeRoutine;
  private void Awake()
  {
    GameLogicHandler.Instance.ItemManager.AddItem(this);
  }
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

  public void SetSlotBase(SlotBase slot)
  {
    this.slot = slot;
  }
  public virtual void OnComplete()
  {
  }

  public virtual void SetSlot(SlotBase slot)
  {
    this.slot = slot;
  }
  public virtual void SetLockState(bool lockState)
  {
    if (boxCollider)
      boxCollider.enabled = !lockState;
  }
  public void SetPrimary(bool isPrimary)
  {
    this.isPrimary = isPrimary;
    boxCollider.enabled = isPrimary;
    visual.SetPrimary(isPrimary);
    if (isPrimary)
    {
      smokeRoutine = StartCoroutine(IESpawnSmokeEffect());
    }
  }
  private bool _isSelected;
  private bool _isDragging;
  private Vector3 _mouseDownPos;
  public void OnHandleMouseDown()
  {
    // if (!isPrimary || _isSelected || GameplayController.Instance.gameState != GameState.Playing) return;
    // _mouseDownPos = Input.mousePosition;
    // Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // mousePos.z = transform.position.z;
    // offset = transform.position - mousePos;
    // _isSelected = true;
    // OnSelected();

    itemBehaviorSO.OnMouseDown(this);
  }
  private Vector3 vel;

  public virtual void SetSelected(bool isSelected)
  {
    _isSelected = isSelected;
  }
  public void OnHandleMouseUp()
  {
    itemBehaviorSO.OnMouseUp(this);
  }

  public void UnSelect()
  {
    itemBehaviorSO.OnMouseExitItem(this);
  }
  IEnumerator IESpawnSmokeEffect()
  {
    yield return new WaitForSeconds(2f);
    while (gameObject.activeInHierarchy)
    {
      if (_isOnConveyor)
      {
        yield break;
      }
      while (transform.position.y > 6)
      {
        yield return new WaitForSeconds(4f);
      }
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
  bool _isOnConveyor = false;
  public virtual void SetIsOnConveyor(bool state)
  {
    _isOnConveyor = state;
    visual.SetVisibleMaskState(state);
  }
  public virtual void SetSuggest(bool suggest)
  {
    if (visual == null) return;
    if (suggest) visual.Suggest();
    else
    {
      visual.EndSuggest();
    }
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
  public virtual void OnDropToSlot(SlotBase slot)
  {
    OnIntoSlot();

    if (this.slot.GetItem() != null)
      PlayDropSound();
    isProcessing = false;
  }
  protected virtual void PlayDropSound()
  {
    itemBehaviorSO.PlayDropSound(this);
  }
  public void SwitchSlot(SlotBase slot)
  {
        //StopCoroutine(smokeRoutine);
        itemBehaviorSO.SwitchSlot(this, slot);
  }

  public void OnIntoSlot()
  {
    transform.localPosition = Vector3.zero;
    visual.OnIntoSlot();
    slot.OnItemIntoSlot();
  }
  public virtual ItemData GetCurrentItemData()
  {
    ItemData itemData = new ItemData()
    {
      id = id,
      hidden = false,
      itemType = itemType,
    };
    return itemData;
  }
  public void MoveToPrimary(SlotBase slot, int index)
  {
    slot.AddItem(this);
    SetPrimary(true);
    if (this.slot != null)
    {
      this.slot.ItemOut();
    }
    this.slot = slot;
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
