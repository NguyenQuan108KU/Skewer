
using UnityEngine;
using DG.Tweening;

public class ScaleElement : MonoBehaviour
{
    [SerializeField]
    private float animationDuration = 0.5f;
    [SerializeField]
    private float delay = 0f;
    [SerializeField]
    private Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);
    void Start()
    {
        StartAnimation();
    }
    public void StartAnimation()
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            transform.DOKill();
            transform.DOScale(targetScale, animationDuration).SetEase(Ease.InOutCirc).SetLoops(-1, LoopType.Yoyo);
        });
    }
}
