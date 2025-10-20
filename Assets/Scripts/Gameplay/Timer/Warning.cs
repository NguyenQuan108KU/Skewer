
using UnityEngine;

public class Warning : MonoBehaviour
{

  private void Start()
  {
    GameplayController.OnFinishGame += HideTimer;
  }
  private void HideTimer()
  {
    gameObject.SetActive(false);
  }
}
