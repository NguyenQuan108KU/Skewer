using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public static class UITween
{
    public static void Play(TweenData tweenData)
    {
        tweenData.SetupData();
        if (tweenData.config == null) return;
        switch (tweenData.config.tweenType)
        {
            case UITweenType.Scale:
                PlayTweenScale(tweenData);
                break;
            case UITweenType.Fade:
                PlayTweenFade(tweenData);
                break;
            case UITweenType.Move:
                PlayTweenMove(tweenData);
                break;
            case UITweenType.LocalMove:
                PlayTweenLocalMove(tweenData);
                break;
            case UITweenType.RectLocalMove:
                PlayTweenRectLocalMove(tweenData);
                break;
            case UITweenType.FadeGroup:
                PlayTweenFadeGroup(tweenData);
                break;
            case UITweenType.Active:
                PlayTweenActive(tweenData);
                break;
        }
    }

    private static void PlayTweenScale(TweenData tweenData)
    {
        var target = tweenData.target;
        target.localScale = Vector3.one * tweenData.config.from;
        target.DOScale(tweenData.config.to, tweenData.config.duration).SetEase(tweenData.config.curve)
            .SetDelay(tweenData.config.delay).OnComplete(() => { tweenData.OnCompleted?.Invoke(); });
    }

    private static void PlayTweenFade(TweenData tweenData)
    {
        var target = tweenData.target.GetComponent<Graphic>();
        var color = target.color;
        color.a = tweenData.config.from;
        target.color = color;
        target.DOFade(tweenData.config.to, tweenData.config.duration).SetEase(tweenData.config.curve)
             .SetDelay(tweenData.config.delay).OnComplete(() => { tweenData.OnCompleted?.Invoke(); });
    }

    private static void PlayTweenMove(TweenData tweenData)
    {
        var target = tweenData.target;
        target.position = tweenData.config.mFrom;
        target.DOMove(tweenData.config.mTo, tweenData.config.duration).SetEase(tweenData.config.curve)
             .SetDelay(tweenData.config.delay).OnComplete(() => { tweenData.OnCompleted?.Invoke(); });
    }

    private static void PlayTweenLocalMove(TweenData tweenData)
    {
        var target = tweenData.target;
        target.localPosition = tweenData.config.mFrom;
        target.DOLocalMove(tweenData.config.mTo, tweenData.config.duration).SetEase(tweenData.config.curve)
             .SetDelay(tweenData.config.delay).OnComplete(() => { tweenData.OnCompleted?.Invoke(); });
    }

    private static void PlayTweenRectLocalMove(TweenData tweenData)
    {
        var target = (RectTransform)tweenData.target;
        target.anchoredPosition = tweenData.config.mFrom;
        target.DOAnchorPos(tweenData.config.mTo, tweenData.config.duration).SetEase(tweenData.config.curve)
             .SetDelay(tweenData.config.delay).OnComplete(() => { tweenData.OnCompleted?.Invoke(); });
    }


    private static void PlayTweenFadeGroup(TweenData tweenData)
    {
        var target = tweenData.target.GetComponent<CanvasGroup>();
        target.alpha = tweenData.config.from;
        target.DOFade(tweenData.config.to, tweenData.config.duration).SetEase(tweenData.config.curve)
             .SetDelay(tweenData.config.delay).OnComplete(() => { tweenData.OnCompleted?.Invoke(); });
    }

    private static void PlayTweenActive(TweenData tweenData)
    {
        tweenData.target.gameObject.SetActive(false);
        DOVirtual.DelayedCall(tweenData.config.delay, () =>
         {
             tweenData.target.gameObject.SetActive(true);
         });
    }
}

