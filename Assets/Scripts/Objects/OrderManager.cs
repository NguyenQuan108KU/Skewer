using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class OrderManager : SingletonBase<OrderManager>
{
    // Start is called before the first frame update
    // [SerializeField] private OrderManagerSO orderManagerSO;
    //[LunaPlaygroundField(fieldSection: "Order Settings")]
    [SerializeField] public int maxOrder = 4;
    //[LunaPlaygroundField(fieldSection: "Order Settings")]

    [SerializeField] private int defaultNumberOfReadyOrder = 4;
    [SerializeField] private OrderEntityConfigSO orderEntityConfigSO;
    [SerializeField] public float delayAlignOrders = 0.5f;

    [Header("Align")]
    [SerializeField] public float distance = 2.6f;
    [SerializeField] private Transform rightStartPos;
    [SerializeField] private Transform leftStartPos;
    public int _nextOrderIndex = 0;
    public List<Vector3> _listOrderLocalPositions = new List<Vector3>();
    public Transform LeftStartPos => leftStartPos;
    public Transform RightStartPos => rightStartPos;

    public List<OrderEntity> _listOrders = new List<OrderEntity>();
    public List<OrderEntity> ListOrders => _listOrders;
    [SerializeField] public List<DataOrder> dataOrders = new List<DataOrder>();
    public List<DataCharacter> dataCharacters = new List<DataCharacter>();
    public virtual void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Init()
    {
        var startPos = -(maxOrder - 1) * distance / 2;
        for (int i = 0; i < maxOrder; i++)
        {
            var orderPos = new Vector3(startPos + distance * i, 0, 0);
            _listOrderLocalPositions.Add(orderPos);
        }

        GameLogicHandler.Instance.OnItemMoveSlot += GameLogicHandler_OnItemMoveSlot;

        // Auto-register any OrderEntity that was manually placed as child of this manager in the scene.
        // This allows manual scene setup (LevelGenerator disabled) to work without extra inspector toggles.
        var preplaced = GetComponentsInChildren<OrderEntity>(true);
        if (preplaced != null && preplaced.Length > 0)
        {
            _listOrders.Clear();
            foreach (var oe in preplaced)
            {
                if (!_listOrders.Contains(oe))
                {
                    _listOrders.Add(oe);
                    // ensure visual state matches active/locked defaults
                    oe.Visual.SetNormalOrder(true);
                    oe.SetActive(true);
                }
            }
        }
    }

    public (OrderEntity order, SlotBase slot) GetDestinationSlot(Item item)
    {
        foreach (var order in _listOrders)
        {
            if (order.ItemIdTarget == item.id && order.Ready)
            {
                var orderSlot = order.GetAvailableSlot();
                if (orderSlot != null)
                {
                    return (order, orderSlot);
                }
            }
        }
        return (null, null);
    }
    public virtual void GameLogicHandler_OnItemMoveSlot(Item item, SlotBase slot)
    {
        if (slot.GetGrill() is OrderEntity orderEntity)
        {
            if (_listOrders.Contains(orderEntity))
            {
                if (orderEntity.CheckComplete())
                {
                    var orderIndex = orderEntity.OrderIndex;
                    _listOrders.Remove(orderEntity);
                    var checkNextOrder = OrderHelper.CheckCreateNextOrder();
                    orderEntity.PlayComplete(() =>
                    {

                        GameLogicHandler.Instance.CompleteCollectItem(orderEntity);
                        if (checkNextOrder == false)
                        {
                            //AlignObjects();
                        }
                    });
                    if (checkNextOrder)
                    {
                       // CreateNextOrder(orderIndex);
                    }
                    GameLogicHandler.Instance.CollectItem(orderEntity);
                }
                else
                {
                    GameLogicHandler.Instance.TryCheckLoseGame();
                }
            }
        }
    }

    public void SetData(List<OrderData> listOrderData)
    {
        _nextOrderIndex = 0;
        for (int i = 0; i < defaultNumberOfReadyOrder; i++)
        {
            if (dataOrders.Count > 0)
            {
                var (itemId, num) = (dataOrders[0].itemId, dataOrders[0].num);
                dataOrders.RemoveAt(0);
                var orderEntity = CreateNextOrder(itemId, num);
                orderEntity.SetOrderIndex(i);
            }
            else
            {
                var (itemId, num) = OrderHelper.GetItemOrder();
                var orderEntity = CreateNextOrder(itemId, num);
                orderEntity.SetOrderIndex(i);
            }
        }
        for (int i = defaultNumberOfReadyOrder; i < maxOrder; i++)
        {
            var orderEntity = CreateNextLockedOrder();
            orderEntity.SetOrderIndex(i);
        }

        //PlayAppearOrders();
    }

    public OrderEntity CreateNextOrder(int id, int num)
    {
        var orderEntity = Instantiate(PrefabManager.Instance.orderEntity, transform).GetComponent<OrderEntity>();
        orderEntity.SetActive(true);
        orderEntity.Visual.SetNormalOrder(true);
        orderEntity.SetData(id, num);
        _listOrders.Add(orderEntity);
        _nextOrderIndex++;
        return orderEntity;
    }
    public void CreateNextOrder(int orderIndex)
    {
        var (itemId, num) = (0, 0);
        if (dataOrders.Count > 0)
        {
            (itemId, num) = (dataOrders[0].itemId, dataOrders[0].num);
            dataOrders.RemoveAt(0);
        }
        else (itemId, num) = OrderHelper.GetItemOrder();
        var nextOrder = CreateNextOrder(itemId, num);
        var orderPos = leftStartPos.position;
        orderPos.z = 0;
        orderPos.y = transform.position.y;
        nextOrder.transform.position = orderPos;
        nextOrder.SetOrderIndex(orderIndex);

        DOVirtual.DelayedCall(orderEntityConfigSO.delayAppearNextOrder, () =>
        {
            SoundManager.Instance.PlaySound(SoundType.BoxAppear);

            nextOrder.transform.DOLocalMove(_listOrderLocalPositions[nextOrder.OrderIndex], 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
           {
               GameLogicHandler.Instance.AppearNextOrder(nextOrder);
           });
        });
    }

    public virtual void AlignObjects()
    {
        StartCoroutine(IAlignObjects());
        // DOVirtual.DelayedCall(delayAlignOrders, () =>
        // {
        //     var start = -(_listOrders.Count - 1) * distance / 2;
        //     var sortedOrders = _listOrders.OrderBy(e => e.OrderIndex).ToList();
        //     var listLocalTargetPositions = new List<Vector3>();
        //     for (int i = 0; i < sortedOrders.Count; i++)
        //     {
        //         listLocalTargetPositions.Add(new Vector3(start + distance * i, 0, 0));

        //         var orderEntity = sortedOrders[i];
        //         orderEntity.transform.DOLocalMove(listLocalTargetPositions[i], 0.1f).SetEase(Ease.OutSine);

        //     }
        // });
    }

    public virtual IEnumerator IAlignObjects()
    {
        yield return new WaitForSeconds(delayAlignOrders);
        var start = -(_listOrders.Count - 1) * distance / 2;
        var sortedOrders = _listOrders.OrderBy(e => e.OrderIndex).ToList();
        var listLocalTargetPositions = new List<Vector3>();
        for (int i = 0; i < sortedOrders.Count; i++)
        {
            listLocalTargetPositions.Add(new Vector3(start + distance * i, 0, 0));

            var orderEntity = sortedOrders[i];
            orderEntity.transform.DOLocalMove(listLocalTargetPositions[i], 0.1f).SetEase(Ease.OutSine);
            yield return new WaitForSeconds(0.075f);
        }
    }
    public OrderEntity CreateNextLockedOrder()
    {
        var orderEntity = Instantiate(PrefabManager.Instance.orderEntity, transform).GetComponent<OrderEntity>();
        orderEntity.SetActive(false);
        orderEntity.Visual.SetNormalOrder(false);
        _listOrders.Add(orderEntity);
        return orderEntity;
    }

    public virtual void PlayAppearOrders()
    {
        DOVirtual.DelayedCall(.1f, () =>
        {
            for (int i = 0; i < _listOrders.Count; i++)
            {
                var order = _listOrders[i];
                order.transform.DOKill();

                var orderPos = rightStartPos.position;
                orderPos.z = 0;
                orderPos.y = transform.position.y;
                order.transform.position = orderPos;
            }
            DOVirtual.DelayedCall(.05f, () =>
            {
                StartCoroutine(IAppearOrders());
            });
        });
    }

    private IEnumerator IAppearOrders()
    {
        for (int i = 0; i < _listOrders.Count; i++)
        {
            var order = _listOrders[i];
            SoundManager.Instance.PlaySound(SoundType.BoxAppear);
            order.transform.DOLocalMove(_listOrderLocalPositions[order.OrderIndex], 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                GameLogicHandler.Instance.AppearNextOrder(order, true);
            });
            yield return new WaitForSeconds(.1f);
        }

    }
    public Dictionary<int, (int maxItems, int num)> GetOrderItemsDict()
    {
        var dict = new Dictionary<int, (int maxItems, int num)>();
        foreach (var order in _listOrders)
        {
            if (order.IsActive == false) continue;

            var targetItem = order.ItemIdTarget;
            if (dict.ContainsKey(targetItem) == false)
            {
                dict[targetItem] = (0, 0);
            }

            var newMaxItems = dict[targetItem].maxItems + order.MaxItems;
            dict[targetItem] = (newMaxItems, dict[targetItem].num);

            foreach (var slot in order.GetSlots())
            {
                if (slot.GetItem() != null)
                {
                    dict[targetItem] = (newMaxItems, dict[targetItem].num + 1);
                }
            }
        }
        return dict;
    }

    public List<DataSuggest> GetOrderItems()
    {
        var dict = new Dictionary<int, (int maxItems, int num)>();
        var listData = new List<DataSuggest>();
        foreach (var order in _listOrders)
        {
            if (order.IsActive == false) continue;

            var targetItem = order.ItemIdTarget;
            var orderData = listData.Find(t => t.targetItem == targetItem);
            if (listData.Find(t => t.targetItem == targetItem) == null)
            {
                var data = new DataSuggest();
                data.maxItems = 0;
                data.num = 0;
                data.targetItem = targetItem;
                orderData = data;
                listData.Add(data);
            }

            var newMaxItems = orderData.maxItems + order.MaxItems;
            orderData.maxItems = newMaxItems;
            foreach (var slot in order.GetSlots())
            {
                if (slot.GetItem() != null)
                {
                    orderData.num += 1;
                }
            }
        }
        return listData;
    }
    public List<int> GetTargetItemIds()
    {
        var list = new List<int>();
        foreach (var order in _listOrders)
        {
            if (order.IsActive == false) continue;
            if (order.ItemIdTarget == 0) continue;
            list.Add(order.ItemIdTarget);
        }
        return list.Distinct().ToList();
    }
    private int currentOrderVisual = 0;
    public DataCharacter GetCurrentOrderVisual()
    {
        if (currentOrderVisual == 0)
        {
            currentOrderVisual = dataCharacters.Count;
        }
        currentOrderVisual--;
        return dataCharacters[currentOrderVisual];
    }
}

[System.Serializable]
public class DataSuggest
{
    public int maxItems;
    public int num;
    public int targetItem;
}


[System.Serializable]
public class DataOrder
{
    public int itemId;
    public int num;
}


[Serializable]
public class DataCharacter
{
    public int id;
    public Sprite sprite;
    public Character character;
}

public enum Character
{
    Man,
    Woman,
}
