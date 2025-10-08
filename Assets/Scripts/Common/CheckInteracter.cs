

using UnityEngine;

public class CheckInteracter : MonoBehaviour
{


  // kiểm tra nếu 30s user không tương tác thì gọi game over
  [SerializeField] private float timeToCheck = 30f;
  private float timer;
  private bool isChecking = false;

  void Start()
  {
    timer = timeToCheck;
  }

  void Update()
  {
    if (Input.GetMouseButtonDown(0) && !isChecking)
    {
      ResetTimer();
    }
    if (!isChecking)
    {
      timer -= Time.deltaTime;
      if (timer <= 0)
      {
        isChecking = true;
        GameplayController.Instance.GameOver();
      }
    }
  }

  private void ResetTimer()
  {
    timer = timeToCheck;
    isChecking = false;
  }
}
