using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTween : MonoBehaviour
{
    public float duration = 2f;

    void Start()
    {
        transform.DORotate(new Vector3(0, 0, -360), duration, RotateMode.FastBeyond360)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }
}
