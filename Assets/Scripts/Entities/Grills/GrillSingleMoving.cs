


using UnityEngine;

public class GrillSingleMoving : PrimaryGrill
{

  private bool isWarning = false;
  [SerializeField] private float warningY = 3.7f;
  private void Update()
  {
    if (!isWarning)
    {
      if (transform.position.y > warningY)
      {
        OnWarningGrill();
        isWarning = true;
      }
    }
    else
    {
      if (transform.position.y < warningY)
      {
        StopWarningGrill();
      }
    }
  }
  protected override void UpdateSubGrills()
  {
    base.UpdateSubGrills();
    grillVisual.StopWarning();
    WarningGrill.Instance.RemoveWarningGrill(this);

  }
  private void OnWarningGrill()
  {
    grillVisual.PlayWarning();
    WarningGrill.Instance.AddWarningGrill(this);
  }

  private void StopWarningGrill()
  {
    grillVisual.StopWarning();
    isWarning = false;
    WarningGrill.Instance.RemoveWarningGrill(this);
  }
}
