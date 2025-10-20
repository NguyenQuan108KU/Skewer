


using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PrimaryGrillIce : PrimaryGrill
{
  [SerializeField] private int iceState = 3;
  [SerializeField] private GrillVisualIce visualIce;
  private int currentState = 0;
  private int numStep = 0;
  private bool success = false;
  public static List<PrimaryGrillIce> primaryGrillIces = new List<PrimaryGrillIce>();
  private bool started;
  public int priority;
  public int CurrentState => currentState;
  public int NumStep => numStep;

  public override void SetData(GrillData grillData)
  {
    currentState = iceState;
    lockState = 1;
    numStep = 0;
    started = false;
    success = false;
    priority = grillData.priority;
    base.SetData(grillData);
    visualIce.SetIceState(currentState);
    SetLockItems(true);
    primaryGrillIces.Add(this);
  }

  public static void CheckAndStartProgress()
  {
    // if (primaryGrillIces is not { Count: > 0 }) return;
    if (primaryGrillIces == null || primaryGrillIces.Count == 0) return;
    // primaryGrillIces.Sort((a, b) => a.priority - b.priority);
    primaryGrillIces[0].StartProgress();
  }

  public void StartProgress()
  {
    if (started) return;
    started = true;
    grillBaseBehaviorSO.eventSystemSO.RegisterEvents_OnCollectItem(OnCollectItem);
    grillBaseBehaviorSO.eventSystemSO.RegisterEvents_OnDropItem(OnItemDropped);
  }

  public void NextProgress()
  {
    grillBaseBehaviorSO.eventSystemSO.UnregisterEvents_OnCollectItem(OnCollectItem);
    grillBaseBehaviorSO.eventSystemSO.UnregisterEvents_OnDropItem(OnItemDropped);
    primaryGrillIces.Remove(this);
    if (primaryGrillIces.Count > 0)
    {
      primaryGrillIces[0].StartProgress();
    }
  }
  private void OnCollectItem(int itemId)
  {
    if (!IsLock) return;
    success = true;
    DownState();
    numStep = 0;
  }

  private void OnItemDropped(Item item, bool changed)
  {
    // if (!changed || !isLock) return;
    if (!changed || lockState != 1) return;
    numStep++;
    success = false;
    if (numStep >= GetIceGrillStep() && currentState < iceState)
    {
      DOVirtual.DelayedCall(0.35f, () =>
      {
        if (!success) UpState();
      }, this);
      numStep = 0;
    }
  }
  private int GetIceGrillStep()
  {
    return grillBaseBehaviorSO.GetIceGrillStep();
  }
  public void UpState()
  {
    if (currentState >= iceState) return;
    this.currentState++;
    visualIce.SetIceState(currentState);
  }

  public void DownState()
  {
    if (currentState <= 0) return;
    this.currentState--;
    visualIce.SetIceState(currentState);

    if (currentState == 0)
    {
      UnlockIce();
    }
  }
  private void UnlockIce()
  {
    NextProgress();
    SetLockItems(false);
  }

  public override void Unlock()
  {
    StartCoroutine(PlayUnlockIce());

  }

  private IEnumerator PlayUnlockIce()
  {
    for (int i = currentState; i > 0; i--)
    {
      DownState();
      yield return new WaitForSeconds(0.3f);
    }
    base.Unlock();
    SetSlotCollider(true);
  }
}
