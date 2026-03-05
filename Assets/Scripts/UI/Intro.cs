using DG.Tweening;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public GameObject icon;
    public GameObject background;

    public float dropTime = 0.6f;
    public float flyTime = 0.6f;

    void Start()
    {
        Camera cam = Camera.main;

        Vector3 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        Vector3 topOut = cam.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 10f));

        icon.transform.position = topOut;

        Sequence seq = DOTween.Sequence();

        // rơi xuống
        seq.Append(icon.transform.DOMove(center, dropTime)
            .SetEase(Ease.InQuad));

        // nảy
        seq.Append(icon.transform.DOJump(
            center,
            2f,
            1,
            0.4f
        ));

        // dừng
        seq.AppendInterval(1.2f);

        // bay lên
        seq.Append(icon.transform.DOMove(topOut, flyTime)
            .SetEase(Ease.OutQuad));

        // background mờ dần
        seq.AppendCallback(() =>
        {
            background.SetActive(false);
        });
    }
}