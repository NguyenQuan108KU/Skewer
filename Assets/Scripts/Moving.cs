using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Moving : MonoBehaviour
{
    public List<GameObject> listShipper;
    public float moveDuration = 0.6f;
    public Vector3 firstTargetPosition; // set trong Inspector

    private void Start()
    {
        DOVirtual.DelayedCall(2.1f, MoveFirstShipper);
    }

    void MoveFirstShipper()
    {
        if (listShipper == null || listShipper.Count == 0)
            return;

        GameObject firstShipper = listShipper[0];

        firstShipper.transform.DOLocalMove(
            firstTargetPosition,
            moveDuration
        ).SetEase(Ease.OutCubic);
    }
}