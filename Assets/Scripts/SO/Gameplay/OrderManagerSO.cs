
using UnityEngine;


[CreateAssetMenu(fileName = "OrderManagerSO", menuName = "MyGame/SkewerJam/Config/OrderManagerSO")]
public class OrderManagerSO : ScriptableObject
{
    public int MaxOrder;
    public int DefaultNumberOfReadyOrder;
}
