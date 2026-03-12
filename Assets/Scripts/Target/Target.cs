


using DG.Tweening;
using TMPro;
using UnityEngine;

public class Target : MonoBehaviour
{
  //[LunaPlaygroundField(fieldSection: "Target Settings")]
  public bool HaveTarget = true;
  public TMP_Text targetText;
  public int currentTarget;
  //[LunaPlaygroundField(fieldSection: "Target Settings")]
  public int maxTarget = 10;
  public Transform waitingSlot;
  public bool isUITarget = false;
  private void Start()
  {
    if (!HaveTarget)
    {
      gameObject.SetActive(false);
      if (!isUITarget)
        waitingSlot.transform.DOLocalMoveX(0, 0);
      return;
    }
    targetText.text = $"{currentTarget}/{maxTarget}";
    GameLogicHandler.Instance.OnCollectItem += CollectItem;
  }

  private void CollectItem(int id)
  {
    currentTarget++;
    targetText.text = $"{currentTarget}/{maxTarget}";
    if (currentTarget >= maxTarget)
    {
      GameplayController.Instance.GameOver(true);
    }
  }
}
