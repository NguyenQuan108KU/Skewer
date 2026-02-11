

using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PrimaryGrill : GrillBase
{
  public static event EventHandler OnAnyMainLayerEmpty;
  public override EntityType entityType => EntityType.PrimaryGrill;
  [SerializeField] protected Transform container;
  [SerializeField] protected Transform subContainer;
  [SerializeField] protected GrillVisual grillVisual;
  [HideInInspector] public bool available;
  public GrillVisual GrillVisual => grillVisual;
  protected GrillData grillData;
  protected List<SubGrill> subGrills;
  protected bool isProcess = false;
  [HideInInspector] public bool isInGameplaySpace = true;
  protected Vector3 subOffset;
  public Action<PrimaryGrill> onMainLayerEmpty;
  public Action<PrimaryGrill> onMainLayerComplete;
  public Transform SubContainer => subContainer;
    public bool isClose;
    public GameObject lib;
    public float duration = 0.4f;

    public int SlotCount => slots.Length;
  public bool isManualSetup = false;


  protected bool completed = false;
  private void Awake()
  {
    if (isManualSetup)
    {
      GameLogicHandler.Instance.GrillManager.AddGrill(this);
      foreach (var slot in slots)
      {
        if (!slot.isEmpty())
        {
          slot.GetItem().SetPrimary(entityType == EntityType.PrimaryGrill);
          slot.GetItem().SetSlotBase(slot);
        }
      }
    }
  }
  public virtual void SetData(GrillData grillData)
  {
    grillVisual.SetDefaultGrill(grillData);
    this.grillData = grillData;
    id = grillData.id;
    transform.localPosition = grillData.position.ToVector3();
    SetSlotId();

    if (this.grillData.layer != null && this.grillData.layer.Count > 0)
    {
      SetLayer(grillData.layer[0]);
      if (grillData.layer.Count > 1)
      {
        subGrills = new List<SubGrill>();
        SetSubGrills();
      }

    }
    OpenGrill();
  }
  public virtual void OpenGrill()
  {
    DOVirtual.DelayedCall(1f, () => grillVisual.OpenGrill(true), this);
  }
  protected virtual void SetSubGrills()
  {
    int layer = 0;
    for (int i = this.grillData.layer.Count - 1; i > 0; i--)
    {
      var subGrill = CreateSubGrill(layer);
      subGrill.SetData(this, this.grillData.layer[i], layer);
      if (i == 1)
      {
        subGrill.Show();
      }
      subGrills.Insert(0, subGrill);
      layer++;
    }
  }
  public virtual List<SubGrill> GetSubGrills()
  {
    return subGrills;
  }
  public virtual void SetSlotId()
  {
    for (int i = 0; i < slots.Length; i++)
    {
      slots[i].SetId(this.id * 1000 + i);
    }
  }

  protected virtual SubGrill CreateSubGrill(int layer)
  {
    Vector3 pos = subContainer.position + (grillData.layer.Count - 1) * Vector3.down * 0.035f + Vector3.up * layer * 0.035f + Vector3.back * layer * 0.03f + subOffset;
    string subGrillName = "SubGrillNormal";
    SubGrill subGrill = Instantiate(PrefabManager.Instance.subGrillNormal, subContainer).GetComponent<SubGrill>();
    subGrill.name = subGrillName;
    subGrill.transform.position = pos;

    return subGrill;
  }

  public virtual List<ShuffleLayerData> GetSubsShuffleLayerData()
  {
    List<ShuffleLayerData> data = new List<ShuffleLayerData>();
    if (subGrills != null)
    {
      foreach (var subGrill in subGrills)
      {
        data.Add(subGrill.GetShuffleLayerData());
      }
    }

    return data;
  }
  public override ShuffleLayerData GetShuffleLayerData()
  {
    return new ShuffleLayerData()
    {
      canNotShuffle = !CanShuffle(),
      grill = this,
      layerData = GetCurrentData()
    };
  }
  public virtual bool CanShuffle()
  {
    if (IsLock) return false;
    foreach (var slot in slots)
    {
      var item = slot.GetItem();
      if (item != null)
      {
        if (item.itemType == ItemType.Ice || item.itemType == ItemType.Bomb || item.itemType == ItemType.Key || item.itemType == ItemType.Key2) return false;
      }
    }

    return true;
  }
  public virtual LayerData GetCurrentData()
  {
    LayerData _layerData = new LayerData(slots.Length);
    for (int i = 0; i < slots.Length; i++)
    {
      var item = slots[i].GetItem();
      if (item != null)
      {
        _layerData.itemData[i] = item.GetCurrentItemData();
      }
      else
      {
        _layerData.itemData[i] = new ItemData() { id = 0 };
      }
    }

    return _layerData;
  }
  public override void ChangeItem(SlotBase selectedSlot, int newId)
  {
    foreach (var slot in slots)
    {
      if (slot == selectedSlot)
      {
        var item = slot.GetItem();
        var itemData = item.Data.Clone();
        itemData.id = newId;
        item.SetItemData(itemData, slot);
        return;
      }
    }

    if (subGrills != null)
    {
      foreach (var subGrill in subGrills)
      {
        subGrill.ChangeItem(selectedSlot, newId);
      }
    }
  }
  protected virtual void UpdateSubGrills()
  {
    isProcess = false;
    grillVisual.UpdateSubGrill();
    if (subGrills == null || subGrills.Count == 0)
    {
      OnAnyMainLayerEmpty?.Invoke(this, EventArgs.Empty);
      return;
    }
    subGrills[0].MoveUpPrimary();
    subGrills.RemoveAt(0);
    if (subGrills.Count > 0)
    {
      subGrills[0].Show();
    }

    // if (IsLock)
    //   SetLockItems(IsLock);
  }

  public virtual void CheckEmpty()
  {
    foreach (var slot in slots)
    {
      if (!slot.isEmpty()) return;
    }
        if (isClose)
        {
            OnClosedLib();
        }
    UpdateSubGrills();
    onMainLayerEmpty?.Invoke(this);

  }
    public void OnClosedLib()
    {
        lib.SetActive(true);
        SpriteRenderer sr = lib.GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;
        lib.transform.localScale = Vector3.zero;

        Vector3 startPos = lib.transform.localPosition;
        startPos.y = 0.8f;
        lib.transform.localPosition = startPos;

        Sequence seq = DOTween.Sequence();

        seq.Append(
    lib.transform.DOScale(Vector3.one, duration)
        .SetEase(Ease.OutBack)
);
        seq.Append(
            lib.transform.DOLocalMoveY(0.2f, duration)
                .SetEase(Ease.OutCubic)
        );
        seq.AppendCallback(() =>
        {
            SoundManager.Instance.PlaySound(SoundType.BoxClosed);
        });
    }
    public override void OnSlotUpdated(SlotBase slot)
  {
    base.OnSlotUpdated(slot);
    if (slot.isEmpty())
    {
      CheckEmpty();
    }

    CheckComplete();


    //SonatUtils.DelayCall(0.1f, () => { GameplayController.instance.CheckOutOfMove(); }, this);
    grillBaseBehaviorSO.OnSlotUpdated(slot);
  }

  public virtual void CheckComplete()
  {
    if (SlotCount == 1) return;
    int id = 0;
    foreach (var slot in slots)
    {
      if (slot.isEmpty()) return;
      if (id == 0) id = slot.GetItem().id;
      else if (id != slot.GetItem().id) return;
    }

  }
  public virtual void OnResetConveyorCircle()
  {
  }
  public virtual bool HasSubGrills()
  {
    return subGrills != null && subGrills.Count > 0;
  }
  public override void SetMaskVisible(bool state)
  {
    base.SetMaskVisible(state);
    grillVisual.SetMaskVisible(state);
    if (subGrills != null)
    {
      foreach (var subGrill in subGrills)
      {
        subGrill.SetMaskVisible(state);
      }
    }
  }


  public virtual void AddFromSub(Item item, int slotIndex, int index)
  {
    grillBaseBehaviorSO.OnAddFromSub(this, item, slotIndex, index);
  }
  public override bool CheckItemWithId(int id)
  {
    return slots.Any(slot => slot.GetItem() != null && slot.GetItem().id == id);
  }

  public override Vector3 DestroyItem(int id)
  {
    Vector3 pos = transform.position;
    foreach (var slot in slots)
    {
      if (slot.GetItem() != null && slot.GetItem().id == id)
      {
        slot.GetItem().OnComplete();
        slot.ClearItem();
        pos = slot.transform.position;
        break;
      }
    }

    return pos;
  }
  protected void SetSlotCollider(bool state)
  {
    foreach (var slot in slots)
    {
      BoxCollider2D collider2D = slot.GetComponent<BoxCollider2D>();
      collider2D.enabled = state;
    }
  }
  public virtual void SetLockItems(bool lockState)
  {
    this.lockState = lockState ? (byte)1 : (byte)0;
    foreach (var slot in slots)
    {
      slot.GetItem()?.SetLockState(lockState);
    }
  }

  public virtual void UnlockState()
  {
    if (!IsLock) return;
    lockState--;
    if (lockState <= 0)
    {
      Unlock();
    }
  }
  public virtual void Unlock()
  {
    SetLockItems(false);
    OpenGrill();
  }
}
