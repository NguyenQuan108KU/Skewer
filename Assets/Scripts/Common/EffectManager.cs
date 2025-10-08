using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EffectManager : MonoBehaviour
{

    public Transform completeTextPrefab;

    [SerializeField] private List<TextPraiseEffectData> textPraiseEffectDataList;

    public AudioSource sourceSpeak;
    [LunaPlaygroundField("Sound Praise", 3, "Sounds")]
    public bool HaveSound = true;
    private Queue<int> recentIndexes = new Queue<int>();
    private const int MaxRecent = 1;
    void Start()
    {
        OrderEntityVisual.OnOrderDone += OnSortVase;
    }

    private void OnSortVase(object sender, EventArgs e)
    {
        PlayEffectCompleteText((sender as OrderEntityVisual).transform.position);
    }

    private int count = 0;
    private void PlayEffectCompleteText(Vector3 startPosition)
    {
        if (count++ % 2 != 0) return;
        var completeTextSpawned = Instantiate(completeTextPrefab).GetComponent<CompleteText>();
        int idx = GetRandomIndexAvoidingRepeats();
        completeTextSpawned.SetTextSprite(textPraiseEffectDataList[idx].sprite);
        completeTextSpawned.transform.position = startPosition - Vector3.forward * 0.1f;
        completeTextSpawned.transform.localScale = transform.localScale;
        if (HaveSound)
        {
            sourceSpeak.PlayOneShot(textPraiseEffectDataList[idx].sound);
        }
        DOVirtual.DelayedCall(1, () =>
               {
                   Destroy(completeTextSpawned.gameObject);
               });
    }

    private int GetRandomIndexAvoidingRepeats()
    {
        int attempt = 0;
        int idx;
        do
        {
            idx = UnityEngine.Random.Range(0, textPraiseEffectDataList.Count);
            attempt++;
            if (textPraiseEffectDataList.Count <= MaxRecent + 1 || attempt > 10)
                break;
        } while (recentIndexes.Contains(idx));

        recentIndexes.Enqueue(idx);
        if (recentIndexes.Count > MaxRecent)
            recentIndexes.Dequeue();

        return idx;
    }
    private void OnDestroy()
    {
        // Vase.OnVaseDone -= OnSortVase;
    }
}


[Serializable]
public class TextPraiseEffectData
{
    public Sprite sprite;
    public AudioClip sound;
}
