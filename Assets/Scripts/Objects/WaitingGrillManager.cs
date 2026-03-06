

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaitingGrillManager : MonoBehaviour
{
  [SerializeField] private Transform centerRefPoint;
  [SerializeField] private float distance = 1.2f;
  public List<WaitingGrill> listWaitingGrills = new List<WaitingGrill>();

  public List<WaitingGrill> ListWaitingGrills => listWaitingGrills;


  private void Start()
  {
    GameLogicHandler.Instance.OnItemMoveSlot += GameLogicHandler_OnItemMoveSlot;
  }

  public void SetData(int numberOfWaitingGrill)
  {

    for (int i = 0; i < numberOfWaitingGrill; i++)
    {
      var waitingGrill = Instantiate(PrefabManager.Instance.waitingGrill, transform).GetComponent<WaitingGrill>();
      waitingGrill.SetActive(true);
      listWaitingGrills.Add(waitingGrill);
    }
    GameplayUtils.AlignObjects(transform, distance);
  }

  private void GameLogicHandler_OnItemMoveSlot(Item item, SlotBase slot)
  {
    if (slot.GetGrill() is WaitingGrill waitingGrill)
    {
      GameLogicHandler.Instance.TryCheckLoseGame();
    }
  }
  public (WaitingGrill waitingGrill, SlotBase slot) GetDestinationSlot()
  {
    foreach (var waitingGrill in listWaitingGrills)
    {
      if (waitingGrill.IsActive == false) continue;
      var slot = waitingGrill.GetSlot(0);
      if (slot.GetItem() == null)
      {
        return (waitingGrill, slot);
      }
    }

    return (null, null);
  }

  public bool CheckClearAllItems()
  {
    foreach (var waitingGrill in listWaitingGrills)
    {
      if (waitingGrill.IsActive && waitingGrill.GetSlots().Where(e => e.GetItem() != null).Count() != 0)
      {
        return false;
      }
    }
    return true;
  }
    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        listWaitingGrills.Clear();
    }
}
