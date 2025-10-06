
using UnityEngine;


[CreateAssetMenu(fileName = "ItemVisualSO_SkewerJam", menuName = "MyGame/SkewerJam/ItemVisualSO_SkewerJam")]
public class ItemVisualSO_SkewerJam : ItemVisualSO
{
    public override void SetVisual(SpriteRenderer spriteRenderer, int id, bool isPrimary)
    {
        ItemDataSO itemData = LevelGenerator.Instance.GetItemData(id);
        spriteRenderer.sprite = itemData.sprite;
    }
}
