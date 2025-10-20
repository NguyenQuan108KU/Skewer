using System;

using UnityEngine;

public abstract class ItemBehaviorSO : ScriptableObject
{
    [SerializeField] public ItemVisualSO itemVisualSO;
    [SerializeField] public EventSystemSO eventSystemSO;

    public abstract void OnMouseDown(Item item);

    public abstract void OnMouseUp(Item item);

    public abstract void OnMouseExitItem(Item item);

    public abstract void SwitchSlot(Item item, SlotBase slot);


    public abstract void OnSelected(Item item);

    public abstract void PlayDropSound(Item item);
}
