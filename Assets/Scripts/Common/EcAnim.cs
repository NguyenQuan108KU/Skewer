using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public enum EntranceType
{
  None,
  ZoomIn,
  ZoomOut,
  Left,
  Right,
  Top,
  Bottom,
}

[System.Serializable]
public class EcNodeConfig
{
  public RectTransform node;
  public float duration = 1f;
  public float delay = 0f;
  public EntranceType entranceType = EntranceType.ZoomIn;
}

public class EcAnim : MonoBehaviour
{

  public List<EcNodeConfig> nodes = new List<EcNodeConfig>();

  private void Awake()
  {
    foreach (var config in nodes)
    {
      if (config.node == null) continue;


      Vector2 anchor = config.node.anchoredPosition;
      Vector3 originalScale = config.node.transform.localScale;

      SetupInitialState(config);

      AnimateNode(config, anchor, originalScale);
    }
  }

  private void SetupInitialState(EcNodeConfig config)
  {

    Vector2 offset = Vector3.zero;

    switch (config.entranceType)
    {
      case EntranceType.ZoomIn:
        config.node.transform.localScale = new Vector2(0f, 0f);
        break;
      case EntranceType.ZoomOut:
        config.node.transform.localScale = new Vector2(1.5f, 1.5f);
        break;
      case EntranceType.Left:
        offset = new Vector2(-500, 0);
        break;
      case EntranceType.Right:
        offset = new Vector2(500, 0);
        break;
      case EntranceType.Top:
        offset = new Vector2(0, 500);
        break;
      case EntranceType.Bottom:
        offset = new Vector2(0, -500);
        break;
      case EntranceType.None:
        config.node.gameObject.SetActive(false);
        break;
      default:
        break;
    }

    config.node.anchoredPosition += offset;
  }

  private void AnimateNode(EcNodeConfig config, Vector3 originalPos, Vector3 originalScale)
  {
    float duration = config.duration > 0 ? config.duration : 1f;
    float delay = config.delay;

    RectTransform t = config.node;

    switch (config.entranceType)
    {
      case EntranceType.ZoomIn:
        t.DOScale(originalScale, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
        break;
      case EntranceType.ZoomOut:
        t.DOScale(originalScale, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
        break;
      case EntranceType.Top:
        t.DOAnchorPosY(originalPos.y, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
        break;
      case EntranceType.Left:
        t.DOAnchorPosX(originalPos.x, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
        break;
      case EntranceType.Right:
        t.DOAnchorPosX(originalPos.x, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
        break;

      case EntranceType.Bottom:
        t.DOAnchorPos(originalPos, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
        break;
      case EntranceType.None:
        DOVirtual.DelayedCall(delay, () =>
        {
          config.node.gameObject.SetActive(true);
        });
        break;
      default:
        break;
    }

  }
}
