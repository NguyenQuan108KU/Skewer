using UnityEngine;
using DG.Tweening;


[CreateAssetMenu(fileName = "ItemBehaviorSO_SkewerJam", menuName = "MyGame/SkewerJam/ItemBehaviorSO_SkewerJam")]
public class ItemBehaviorSO_SkewerJam : ItemBehaviorSO
{
    [Space(10)]
    [Header("Animation")]
    [SerializeField] private Vector3 scaleDown = new Vector3(0.8f, 1.1f, 1f);
    [SerializeField] private float scaleDuration = 0.05f;
    [SerializeField] private bool useSpeed = false;
    [SerializeField] private float durationMove = 0.4f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private AnimationCurve curveX = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve curveY = AnimationCurve.Linear(0, 0, 1, 1);

    private Item selectedItem;

    public override void OnSelected(Item item)
    {
        item.Visual.OnSelected();
        item.Slot.OnItemSelected();
    }

    public override void PlayDropSound(Item item)
    {
        // if (item.Slot.GetGrill().grillType == GrillType.Broken) return;
        // MySonatFramework.audioService.PlaySound(AudioId.Items_Pick_SMode_HLW_Grill_sort);
    }

    public override void OnMouseDown(Item item)
    {
        if (!item.isPrimary || item.IsSelected || item.IsLocked || item.Slot.GetGrill().IsLock) return;

        selectedItem = item;
        item.DOKill();
        item.transform.DOScale(scaleDown, scaleDuration);
    }

    public override void OnMouseExit(Item item)
    {
        if (selectedItem == item)
        {
            selectedItem = null;
        }
        item.DOKill();
        item.transform.DOScale(Vector3.one, scaleDuration);
    }

    public override void OnMouseUp(Item item)
    {
        // if (GameController.Instance.GameState != GameState.Playing) return;
        if (selectedItem == item)
        {
            var switchSuccess = GameLogicHandler.Instance.SelectItem(item);
            if (switchSuccess == false)
            {
                item.transform.DOKill();
                item.transform.DOScale(Vector3.one, scaleDuration);
            }
        }
        else
        {
            if (selectedItem != null)
            {
                selectedItem.transform.DOKill();
                selectedItem.transform.DOScale(Vector3.one, scaleDuration);
            }
        }
        selectedItem = null;

    }

    public override void SwitchSlot(Item item, SlotBase slot)
    {
        item.transform.DOKill();
        if (item.Slot != null)
        {
            item.Slot.ItemOut();
        }

        item.SetSlot(slot);
        item.SetSelected(true);
        slot.AddItem(item);
        item.Visual.OnDeselected();

        item.Visual.SetSortingOrder(1);

        var seq = DOTween.Sequence();
        if (useSpeed)
        {
            seq.Join(item.transform.DOLocalMoveX(0, speed).SetSpeedBased(useSpeed).SetEase(curveX));
            seq.Join(item.transform.DOLocalMoveY(0, speed).SetSpeedBased(useSpeed).SetEase(curveY));
        }
        else
        {
            seq.Join(item.transform.DOLocalMoveX(0, durationMove).SetEase(curveX));
            seq.Join(item.transform.DOLocalMoveY(0, durationMove).SetEase(curveY));
        }
        seq.Append(item.transform.DOScale(scaleDown, scaleDuration));
        seq.Append(item.transform.DOScale(Vector3.one, scaleDuration));
        seq.OnComplete(() =>
        {
            item.SetSelected(false);
            item.OnDropToSlot(slot);
            item.Visual.SetSortingOrder(0);
            GameLogicHandler.Instance.ItemMoveSlot(item, slot);
        });
    }
}
