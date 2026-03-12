using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    //[LunaPlaygroundField(fieldSection: "2 Level")]
    public bool isLevel2;
    public GameObject buttonStore;
    public List<DataOrder> dataOrders = new List<DataOrder>();

    public GameObject iconEasy;
    public GameObject iconHard;
    public GameObject background;

    public float dropTime = 0.6f;
    public float flyTime = 0.6f;

    public TextAsset levelDataFileHard;
    private void OnEnable()
    {
        GameEvent.StartLevel2.AddListener(GameHard);
    }

    private void OnDisable()
    {
        GameEvent.StartLevel2.RemoveListener(GameHard);
    }

    void Start()
    {
        GameEasy();
    }

    public void GameEasy()
    {
        Camera cam = Camera.main;

        Vector3 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        Vector3 topOut = cam.ViewportToWorldPoint(new Vector3(0.5f, 2f, 10f));

        iconEasy.transform.position = topOut;

        Sequence seq = DOTween.Sequence();
        seq.Append(iconEasy.transform.DOMove(center, dropTime).SetEase(Ease.InQuad));
        seq.AppendCallback(() =>
        {
            SoundManager.Instance.PlaySound(SoundType.Easy);
        });
        seq.Append(iconEasy.transform.DOJump(center, 1.3f, 1, 0.4f));
        seq.AppendInterval(1f);
        seq.Append(iconEasy.transform.DOMove(topOut, flyTime).SetEase(Ease.OutQuad));
        seq.AppendCallback(() =>
        {
            background.SetActive(false);
        });
    }

    public void GameHard()
    {
        background.SetActive(true);
        LoadLevel();
        RectTransform rect = iconHard.GetComponent<RectTransform>();

        // đặt giữa màn hình
        rect.anchoredPosition = Vector2.zero;

        // bắt đầu nhỏ
        iconHard.transform.localScale = Vector3.zero;
        iconHard.SetActive(true);

        Sequence seq = DOTween.Sequence();

        // phóng to
        seq.Append(iconHard.transform.DOScale(12f, 1.2f));
        seq.AppendCallback(() =>
        {
            SoundManager.Instance.PlaySound(SoundType.Hard);
        });
        // nhún nhún
        seq.Append(iconHard.transform.DOScale(11f, 0.5f).SetEase(Ease.OutBack));
        seq.Append(iconHard.transform.DOScale(12f, 0.7f).SetEase(Ease.OutBack));
        seq.Append(iconHard.transform.DOScale(11f, 0.5f).SetEase(Ease.OutBack));

        // dừng
        //seq.AppendInterval(0.5f);

        // thu nhỏ
        seq.Append(iconHard.transform.DOScale(0f, 1f).SetEase(Ease.InBack));

        // tắt
        seq.AppendCallback(() =>
        {
            iconHard.SetActive(false);
            background.SetActive(false);
        });
    }
    public void LoadLevel()
    {
        OrderManager.Instance.dataOrders = dataOrders;
        LevelGenerator.Instance.levelDataFile = levelDataFileHard;  
        LevelGenerator.Instance.GenerateLevel();
        if(isLevel2)
            buttonStore.SetActive(true);
    }
}