

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModeMovingManager : MonoBehaviour
{

  [SerializeField] private Vector3 speedMoving = Vector3.zero;
  private void Start()
  {
    GameEvent.OnLoadedLevel.AddListener(Init);
    GameEvent.OnUserFirstTouch.AddListener(StartMoving);
    GameplayController.OnFinishGame += () =>
    {
      isMoving = false;
    };
  }
  public void Init()
  {
    List<PrimaryGrill> primaryGrills = GameLogicHandler.Instance.GrillManager.ListGrills;
    List<List<PrimaryGrill>> groups = new List<List<PrimaryGrill>>();
    foreach (var primaryGrill in primaryGrills)
    {
      if (groups.Count == 0)
      {
        groups.Add(new List<PrimaryGrill> { primaryGrill });
        continue;
      }
      bool isAdded = false;
      foreach (var group in groups)
      {
        if (group[0].transform.position.x == primaryGrill.transform.position.x)
        {
          group.Add(primaryGrill);
          isAdded = true;
          break;
        }
      }
      if (!isAdded)
      {
        groups.Add(new List<PrimaryGrill> { primaryGrill });
      }
    }
    int index = 0;
    foreach (var group in groups)
    {
      List<PrimaryGrill> primaryGrillsGroup = group.OrderBy(p => p.transform.position.y).ToList();
      ColumnDown columnDown = new GameObject("ColumnDown" + index).AddComponent<ColumnDown>();
      columnDown.transform.SetParent(transform);
      columnDown.transform.localPosition = Vector3.zero;
      columnDown.SetData(primaryGrillsGroup);
      columnDown.delay = 0;
      index++;
    }
  }
  private bool isMoving = false;
  private void Update()
  {
    if (isMoving)
    {
      transform.Translate(speedMoving * Time.deltaTime);
    }
  }
  private void StartMoving()
  {
    isMoving = true;
  }
}
