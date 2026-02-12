using System.Collections.Generic;
using UnityEngine;

public class SimpleConveyor : MonoBehaviour
{
    public List<Transform> objects;
    public Transform limit;
    public float speed = 2f;
    public float spacing = 1.2f;

    void Update()
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if (objects[i] == null)
            {
                objects.RemoveAt(i);
                continue;
            }
            objects[i].localPosition += Vector3.up * speed * Time.deltaTime;

            if (objects[i].localPosition.y >= limit.localPosition.y)
            {
                MoveToEnd(objects[i]);
            }
        }
    }


    void MoveToEnd(Transform obj)
    {
        float lowestY = objects[0].localPosition.y;
        foreach (var t in objects)
        {
            if (t.localPosition.y < lowestY)
                lowestY = t.localPosition.y;
        }
        Vector3 newPos = obj.localPosition;
        newPos.y = lowestY - spacing;
        obj.localPosition = newPos;
    }
}
