

using UnityEngine;

public class CheckBoundMoving : MonoBehaviour
{


  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.name == "Slot")
    {
      GameplayController.Instance.GameOver(false);
    }
  }
}
