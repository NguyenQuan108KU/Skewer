


using System.Collections.Generic;
using UnityEngine;

public class CountLayer : MonoBehaviour
{
  public List<GameObject> progressElements;

  int countLayer = 0;
  private void Start()
  {
    countLayer = progressElements.Count;
  }
  public void SetSubLayer()
  {
    countLayer--;
    for (int i = countLayer; i < progressElements.Count; i++)
    {
      if (i >= progressElements.Count || i < 0) break;
      progressElements[i].SetActive(false);
    }
  }
}
