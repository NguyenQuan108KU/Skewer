using System;
using UnityEngine;


public abstract class GrillBaseBehaviorSO : ScriptableObject
{
    [SerializeField] public EventSystemSO eventSystemSO;
    [SerializeField] public GrillVisualSO grillVisualSO;
    [SerializeField] public SlotBaseBehaviorSO slotBaseBehaviorSO;

    public abstract void OnSlotUpdated(SlotBase slot);

    public abstract void SetSubOffset(GrillBase grillBase, out Vector3 subOffset);
    public abstract ItemType ValidateItemType(ItemType itemType);
    public abstract int GetIceGrillStep();

    public abstract GameState GetGameState();

    public abstract void OnAddFromSub(PrimaryGrill primaryGrill, Item item, int slotIndex, int index);
}
