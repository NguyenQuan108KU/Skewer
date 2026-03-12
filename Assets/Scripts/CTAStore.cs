using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTAStore : SingletonBase<CTAStore>
{
    [Header("CTA Store Settings")]
    //[LunaPlaygroundField("Target", 0, "Target")]
    public int target;
    public int total;
    public bool isEndGame;
    private void OnEnable()
    {
        GameEvent.OnPlayComplete.AddListener(CompleteTarget);
    }

    private void OnDisable()
    {
        GameEvent.OnPlayComplete.RemoveListener(CompleteTarget);
    }
    private void Update()
    {
        if (isEndGame && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Go to store");
            PlayableAPI.GoToStore();
        }
    }
    public void CompleteTarget(int point)
    {
        total += point;
        if(total >= target && !isEndGame)
        {
            isEndGame = true;
            PlayableAPI.GameEnded();
            Debug.Log("CTA Store: Complete Target, go to store");
        }
    }
}
