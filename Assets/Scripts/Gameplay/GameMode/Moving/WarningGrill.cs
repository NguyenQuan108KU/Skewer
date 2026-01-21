

using System.Collections.Generic;
using UnityEngine;

public class WarningGrill : SingletonBase<WarningGrill>
{
  [SerializeField] private AudioSource audioSource;
  private List<PrimaryGrill> listWarningGrills = new List<PrimaryGrill>();

  private void OnEnable()
  {
    GameplayController.OnFinishGame += OnFinishGame;
  }
  private void OnFinishGame()
  {
    isEndGame = true;
  }

  public void AddWarningGrill(PrimaryGrill primaryGrill)
  {
    listWarningGrills.Add(primaryGrill);
  }
  public void RemoveWarningGrill(PrimaryGrill primaryGrill)
  {
    if (listWarningGrills.Contains(primaryGrill))
      listWarningGrills.Remove(primaryGrill);
  }
  // check liên tục nếu có 1 grill warning thì play sound loop 1s 1 lần, nếu không có grill warning thì stop sound
  private float timePlaySound = 1;
  private bool isEndGame = false;
  private void Update()
  {
    if (isEndGame) return;
    if (listWarningGrills.Count > 0)
    {
      timePlaySound += Time.deltaTime;
      if (timePlaySound >= 1)
      {
        timePlaySound = 0;
        audioSource.Play();
      }
    }
  }
}
