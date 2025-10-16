

using System;
using System.Collections;
using UnityEngine;

public class GameplayController : SingletonBase<GameplayController>
{

  [HideInInspector] public bool IsStart = false;
  public LayerMask layerMaskItem;
  public GameState gameState = GameState.Loading;
  public EndCard endCard;
  private Item itemTmp = null;
  [LunaPlaygroundField(fieldSection: "Game Settings")]
  public bool HaveBGM = true;
  public static event Action OnFinishGame;
  public static event Action OnGameObjectFinish;


  [Header("End Game")]
  public GameObject panelWin;
  public GameObject panelLose;
  public GameObject panelLoseTime;
  public GameObject particleWin;


  [Header("Limit Move")]
  [LunaPlaygroundField(fieldSection: "Limit Move")]
  public int totalClickItem = 0;
  [HideInInspector] private int countClickItem = 0;
  [LunaPlaygroundField(fieldSection: "Limit Move")]
  public int totalOrder = 0;
  [HideInInspector] private int countOrder = 0;

  public bool isReady = false;
  private void Awake()
  {
    gameState = GameState.Playing;
  }

  private void Start()
  {
    GameLogicHandler.Instance.OnCompleteCollectItem += ((OrderEntity orderEntity) =>
    {
      countOrder++;
    });
  }
  public void OnStartGame()
  {
    GameEvent.OnUserFirstTouch.Invoke();
    IsStart = true;
    PlayableAPI.LogEventStart();
    if (HaveBGM) SoundManager.Instance.PlaySound(SoundType.BGM);
  }


  private void Update()
  {
    if (gameState == GameState.GameOver || !isReady)
    {
      return;
    }
    if (Input.GetMouseButtonDown(0))
    {

      if (!IsStart)
      {
        OnStartGame();
      }
      OnHandleMouseDown(Input.mousePosition);
    }
    if (Input.GetMouseButtonUp(0))
    {
      OnHandleMouseUp();
    }
  }

  private void OnHandleMouseDown(Vector2 mousePos)
  {

    if ((totalClickItem > 0 && countClickItem >= totalClickItem) ||
        (totalOrder > 0 && countOrder >= totalOrder))
    {
      if (!isCallFinishGame)
      {
        isCallFinishGame = true;
        PlayableAPI.GameEnded();
      }
      GoToStore();
      return;
    }
    countClickItem++;
    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(mousePos);
    var col = Physics2D.OverlapPoint(touchPosition, layerMaskItem);
    if (col)
    {
      col.transform.TryGetComponent<Item>(out itemTmp);
      if (itemTmp != null)
      {
        itemTmp.OnHandleMouseDown();
      }
    }
  }

  private void OnHandleMouseUp()
  {
    if (itemTmp == null) return;
    itemTmp.OnHandleMouseUp();
    itemTmp = null;
  }

  public void GameOver(bool isWin = false, StuckType? stuckType = null)
  {
    if (gameState == GameState.GameOver) return;
    gameState = GameState.GameOver;
    OnFinishGame?.Invoke();
    if (!isCallFinishGame)
    {
      isCallFinishGame = true;
      PlayableAPI.GameEnded();
    }
    OnGameObjectFinish?.Invoke();
    StartCoroutine(IGameOver(isWin, stuckType));
  }
  IEnumerator IGameOver(bool IsWin = false, StuckType? stuckType = null)
  {
    yield return new WaitForSeconds(0.5f);
    if (IsWin)
    {
      SoundManager.Instance.PlaySound(SoundType.Confetti);
      if (particleWin) particleWin.SetActive(true);
      yield return new WaitForSeconds(particleWin != null ? 1f : 0f);
      panelWin.SetActive(true);
      SoundManager.Instance.PlaySound(SoundType.Win);
      PlayableAPI.LogEventWin();
    }
    else
    {
      if (stuckType == StuckType.SkewerJam_OutOfSpace)
      {
        panelLose.SetActive(true);
      }
      else
        panelLoseTime.SetActive(true);
      SoundManager.Instance.PlaySound(SoundType.Lose);
      PlayableAPI.LogEventFailed();
    }
  }
  [HideInInspector] public bool isCallFinishGame = false;
  public void GoToStore()
  {
    if (!isCallFinishGame)
    {
      isCallFinishGame = true;
      PlayableAPI.GameEnded();
    }
    PlayableAPI.GoToStore();
  }
}
