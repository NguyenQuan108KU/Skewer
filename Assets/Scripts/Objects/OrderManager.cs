using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private OrderManagerSO orderManagerSO;
    [SerializeField] private OrderEntityConfigSO orderEntityConfigSO;
    [SerializeField] private float delayAlignOrders = 0.5f;

    [Header("Align")]
    [SerializeField] private float distance = 2.6f;
    [SerializeField] private Transform rightStartPos;
    [SerializeField] private Transform leftStartPos;
    private int _nextOrderIndex = 0;
    private List<Vector3> _listOrderLocalPositions = new List<Vector3>();
    public Transform LeftStartPos => leftStartPos;
    public Transform RightStartPos => rightStartPos;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {

    }
}
