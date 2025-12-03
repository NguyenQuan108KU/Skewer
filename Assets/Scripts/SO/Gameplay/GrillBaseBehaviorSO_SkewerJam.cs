using UnityEngine;


[CreateAssetMenu(fileName = "GrillBaseBehaviorSO_SkewerJam", menuName = "MyGame/SkewerJam/GrillBaseBehaviorSO_SkewerJam")]
public class GrillBaseBehaviorSO_SkewerJam : GrillBaseBehaviorSO
{
    public override void OnSlotUpdated(SlotBase slot)
    {
    }

    public override void SetSubOffset(GrillBase grillBase, out Vector3 subOffset)
    {
        subOffset = Vector3.zero;
    }

    public override ItemType ValidateItemType(ItemType itemType)
    {
        if (itemType == ItemType.Ice) return ItemType.Normal;
        return itemType;
    }

    public override int GetIceGrillStep()
    {
        return 6;
    }

    public override GameState GetGameState()
    {
        return GameState.Loading;
    }

    public override void OnAddFromSub(PrimaryGrill primaryGrill, Item item, int slotIndex, int index)
    {
        item.MoveToPrimary(primaryGrill.GetSlot(slotIndex), index);
    }
}
