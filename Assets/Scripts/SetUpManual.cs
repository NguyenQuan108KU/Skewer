using System.Collections.Generic;
using UnityEngine;

public class SetUpManual : MonoBehaviour
{
    public List<PrimaryGrill> listPrimaryGrill = new List<PrimaryGrill>();
    private void OnEnable()
    {
        GameEvent.OnSetFirstGrill.AddListener(RefreshAll);
    }

    private void OnDisable()
    {
        GameEvent.OnSetFirstGrill.RemoveListener(RefreshAll);
    }
    private void Start()
    {
        RefreshOneGrill();
    }

    public void RefreshAll()
    {
        RefreshOneGrill();
    }

    public void RefreshOneGrill()
    {
        listPrimaryGrill.RemoveAll(grill => grill == null);
        foreach (var grill in listPrimaryGrill)
        {
            var colliders = grill.GetComponentsInChildren<BoxCollider2D>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
        }
        if (listPrimaryGrill.Count == 0) return;

        var firstGrillColliders =
            listPrimaryGrill[0].GetComponentsInChildren<BoxCollider2D>();

        foreach (var col in firstGrillColliders)
        {
            col.enabled = true;
        }
    }
}
