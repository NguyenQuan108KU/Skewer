

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColumnDown : MonoBehaviour
{
  public List<PrimaryGrill> primaryGrills;
  public float delay = 1f;
  public float offsetY = 2.5f;
  private bool isProcessing = false;


  void Start()
  {
    PrimaryGrill.OnAnyMainLayerEmpty += HandleAnyMainLayerEmpty;
  }

  public void SetData(List<PrimaryGrill> primaryGrills)
  {
    this.primaryGrills = primaryGrills;
    foreach (var primaryGrill in primaryGrills)
    {
      primaryGrill.transform.SetParent(transform);
    }
    offsetY = CalculateOffsetY(primaryGrills);
  }

  private float CalculateOffsetY(List<PrimaryGrill> primaryGrills)
  {
    float minDistance = float.MaxValue;
    for (int i = 0; i < primaryGrills.Count; i++)
    {
      for (int j = i + 1; j < primaryGrills.Count; j++)
      {
        float distance = Vector3.Distance(primaryGrills[i].transform.position, primaryGrills[j].transform.position);
        if (distance < minDistance)
        {
          minDistance = distance;
        }
      }
    }
    return minDistance;
  }
  private void HandleAnyMainLayerEmpty(object sender, System.EventArgs e)
  {
    PrimaryGrill primaryGrill = (PrimaryGrill)sender;
    if (primaryGrills.Contains(primaryGrill))
    {
      grillQueue.Enqueue(primaryGrill);
      if (!isProcessing)
        StartCoroutine(ProcessQueue());
    }
  }
  private Queue<PrimaryGrill> grillQueue = new Queue<PrimaryGrill>();
  private IEnumerator ProcessQueue()
  {
    isProcessing = true;
    bool isCheck = false;
    while (grillQueue.Count > 0)
    {
      PrimaryGrill grill = grillQueue.Dequeue();
      yield return StartCoroutine(IHandleFallDown(grill, isCheck == false));
      isCheck = true;
    }
    isProcessing = false;
  }



  private IEnumerator IHandleFallDown(PrimaryGrill grill, bool applyDelay)
  {
    if (applyDelay)
    {
      yield return new WaitForSeconds(delay);
    }
    if (primaryGrills.Contains(grill))
    {
      Vector3 targetPosition = grill.transform.position;
      int index = primaryGrills.IndexOf(grill);
      primaryGrills.RemoveAt(index);
      // grill.transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z + 2);
      grill.transform.DOScale(Vector3.zero, 0.3f).SetDelay(0.1f).SetEase(Ease.Linear).OnComplete(() =>
      {
        grill.gameObject.SetActive(false);

      });
      if (index < primaryGrills.Count)
      {
        DOVirtual.DelayedCall(0.2f, () =>
        {
          SoundManager.Instance.PlaySound(SoundType.DropGrill);
        });
      }
      List<PrimaryGrill> grillsToMove = new List<PrimaryGrill>();
      for (int i = index; i < primaryGrills.Count; i++)
        grillsToMove.Add(primaryGrills[i]);
      if (grillsToMove.Count > 0)
      {
        Sequence seq = AnimateFall(grillsToMove);
        yield return seq.WaitForCompletion();
      }
    }

  }
  private Sequence AnimateFall(List<PrimaryGrill> fallingGrills)
  {
    GameObject group = new GameObject("FallGroup");
    group.transform.SetParent(transform.parent);
    group.transform.position = fallingGrills[0].transform.position;
    foreach (var grill in fallingGrills)
      grill.transform.SetParent(group.transform, true);

    Vector3 startPos = group.transform.localPosition;
    Vector3 dropPos = startPos + Vector3.down * offsetY;

    var seq = DOTween.Sequence()
        .Append(group.transform.DOLocalMove(dropPos, 0.25f).SetEase(Ease.InCubic))
        .Append(group.transform.DOLocalMoveY(dropPos.y + 0.4f, 0.15f).SetEase(Ease.OutCubic))
        .Append(group.transform.DOLocalMoveY(dropPos.y, 0.15f).SetEase(Ease.InCubic))
        .Append(group.transform.DOLocalMoveY(dropPos.y + 0.15f, 0.1f).SetEase(Ease.OutCubic))
        .Append(group.transform.DOLocalMoveY(dropPos.y, 0.1f).SetEase(Ease.InCubic))
        .Append(group.transform.DOLocalMoveY(dropPos.y + 0.05f, 0.08f).SetEase(Ease.OutCubic))
        .Append(group.transform.DOLocalMoveY(dropPos.y, 0.08f).SetEase(Ease.InCubic));

    seq.OnComplete(() =>
    {
      foreach (var grill in fallingGrills)
        grill.transform.SetParent(this.transform, true);
      Destroy(group);
    });

    return seq;
  }
}
