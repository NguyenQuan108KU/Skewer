using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderEntityConveyorFake : OrderEntity
{
    private void Update()
    {
        CheckIsAccept();
    }
    public void CheckIsAccept()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewPos.z > 0 &&
            viewPos.x >= 0 && viewPos.x <= 1 &&
            viewPos.y >= 0 && viewPos.y <= 1)
        {
            isAccept = true;
        }
        else
        {
            isAccept = false;
        }
    }
}
