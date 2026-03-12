


using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerProgress : MonoBehaviour
{
  public Sprite fillImageRed;
  public Image fillImage;
  public Slider slider;
  //[LunaPlaygroundField(fieldSection: "Timer Settings")]
  public float timer = 60;
  public TMP_Text timerText;
  //[LunaPlaygroundField(fieldSection: "Timer Settings")]
  public bool haveTimer = true;
  private bool IsStart = false;
  [SerializeField] private bool isFormatTimeMMSS = true;
    //PS: Warning Timer
    public GameObject warningSprite;
    public float timeWarning;
    private Tween warningTween;
    private bool isWarning = false;
    public float warningSoundTimer = 0f;
    private float warningSoundInterval = 1f; // 1 giây kêu 1 lần
    void Start()
  {

    if (!haveTimer)
    {
      gameObject.SetActive(false);
      return;
    }
    if (slider)
    {
      slider.maxValue = timer;
      slider.value = timer;
    }
    if (timerText) SetTime(timer);

    GameEvent.OnUserFirstTouch.AddListener(() =>
    {
      IsStart = true;
    });
  }
  private bool isFillRed = false;
  void Update()
  {
    if (IsStart && timer >= 0)
    {
      timer -= Time.deltaTime;
      if (timerText) SetTime(timer >= 0 ? timer : 0);
      if (slider) slider.value = timer;
      if (timer <= timeWarning && fillImage && fillImageRed && !isFillRed)
      {
        isFillRed = true;
        fillImage.sprite = fillImageRed;
        transform.DOShakeScale(1f, 0.1f, 1, 90, false).SetLoops(-1, LoopType.Restart);
      }
            //ps
            if (timer <= timeWarning)
            {
                if (!isWarning)
                {
                    isWarning = true;
                    warningSoundTimer = 0f;
                    SoundManager.Instance.PlaySound(SoundType.Warning);

                    if (warningSprite)
                    {
                        warningSprite.SetActive(true);

                        CanvasGroup cg = warningSprite.GetComponent<CanvasGroup>();
                        if (cg == null)
                            cg = warningSprite.AddComponent<CanvasGroup>();

                        warningTween = cg.DOFade(0f, 0.5f)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine);
                    }
                }
                else
                {
                    warningSoundTimer += Time.deltaTime;
                    if (warningSoundTimer >= warningSoundInterval)
                    {
                        warningSoundTimer = 0f;
                        SoundManager.Instance.PlaySound(SoundType.Warning);
                    }
                }
            }

            //
            if (timer <= 0)
      {
        GameplayController.Instance.GameOver();
                warningSprite.SetActive(false);
            }
    }
  }

  private void SetTime(float time)
  {
    // set format second:milliseconds
    int seconds = Mathf.FloorToInt(time);
    int milliseconds = Mathf.FloorToInt((time - seconds) * 100);
    if (!isFormatTimeMMSS)
      timerText.text = string.Format("{0:00}:{1:00}", seconds, milliseconds);
    else
    {
      int minutes = Mathf.FloorToInt(time / 60);
      timerText.text = string.Format("{0:00}:{1:00}", minutes, Mathf.FloorToInt(time % 60));
    }
  }
}
