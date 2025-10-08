

using UnityEngine;

public class ResponsiveUI : ResponsiveBase
{
  public Vector3 positionVertical;
  public Vector3 positionHorizontal;
  public Vector3 positionTablet;

  private RectTransform rectTransform;

  public override void Start()
  {
    rectTransform = GetComponent<RectTransform>();
    base.Start();
  }
  public override void OnHorizontal()
  {
    if (rectTransform != null)
    {
      rectTransform.anchoredPosition = positionHorizontal;
    }
  }

  public override void OnVertical()
  {
    if (rectTransform != null)
    {
      rectTransform.anchoredPosition = positionVertical;
    }
  }

  public override void OnVerticalTall()
  {
    if (rectTransform != null)
    {
      rectTransform.anchoredPosition = positionVertical; // Assuming same position as vertical for tall
    }
  }
  public override void OnTablet()
  {
    if (rectTransform != null)
    {
      rectTransform.anchoredPosition = positionTablet;
    }
  }
}
