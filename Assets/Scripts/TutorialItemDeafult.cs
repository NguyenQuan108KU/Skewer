using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialItemDeafult : TutorialManager
{
    [Header("Tutorial Override")]
    public bool useFixedTutorialItem = true;
    public Item fixedTutorialItem;
    private bool isTutorialDone = false;
    protected override void Awake()
    {
        base.Awake();
    }
    public override Item GetSuggestItem()
    {
        if (useFixedTutorialItem && !isTutorialDone && fixedTutorialItem != null)
        {
            return fixedTutorialItem;
        }
        return base.GetSuggestItem();
    }
    protected override void OnItemMoveSlot()
    {
        isTutorialDone = true;
        base.OnItemMoveSlot();
    }
}
