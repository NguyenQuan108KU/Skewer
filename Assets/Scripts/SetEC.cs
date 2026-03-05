using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEC : MonoBehaviour
{
    [LunaPlaygroundField(fieldSection: "EC Setting")]
    public bool isECLoseTimer;

    public GameObject ecLoseTimer;
    public GameObject ecLoseTray;

    private void Start()
    {
        if (isECLoseTimer)
        {
            GameplayController.Instance.panelLoseTime = ecLoseTimer;
        }
        else
        {
            GameplayController.Instance.panelLoseTime = ecLoseTray;
        }
    }
}
