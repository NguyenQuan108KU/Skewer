

using UnityEngine;

public class DisableSelfFinish : MonoBehaviour
{

  private void OnEnable()
  {
    GameplayController.OnGameObjectFinish += DestroySelf;
  }
  private void DestroySelf()
  {
    gameObject.SetActive(false);
  }

  private void OnDisable()
  {
    GameplayController.OnGameObjectFinish -= DestroySelf;
  }

}
