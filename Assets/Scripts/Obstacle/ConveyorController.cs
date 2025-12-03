

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : EntityBase
{
  public override EntityType entityType => EntityType.Conveyor;
  [SerializeField] protected MoveType moveType;
  protected ConveyorData conveyData;
  [SerializeField] protected Transform container;
  protected Vector3 startPosition;
  protected Vector3 endPosition;
  protected List<PrimaryGrill> grills;
  protected float spacing = 4f;
  protected Vector3 direction;
  protected Vector3 conveySpeed;
  protected Vector3 visualSpeed;
  protected Material materialVisual;
  [SerializeField] protected float visualRatio = -1.2f;
  [SerializeField] protected SpriteRenderer visual;
  protected bool paused;
  protected bool moving = true;
  // protected EventBinding<GameStateChangeEvent> onGameStateChange;
  protected bool hasSubGrill = false;
  [SerializeField] protected Material visualMaterial;



  private void Start()
  {
    GameEvent.OnUserFirstTouch.AddListener(StartMovement);
  }
  public PrimaryGrill GetGrill(int grillId)
  {
    var grill = GameLogicHandler.Instance.GrillManager.GetGrill(grillId);
    return grill;
  }

  public virtual void GetGrills()
  {
    grills = new List<PrimaryGrill>();
    hasSubGrill = false;
    foreach (var grillId in conveyData.grillIds)
    {
      var grill = GetGrill(grillId);
      if (grill != null)
      {
        grills.Add(grill);
        grill.SetMaskVisible(true);
        if (!hasSubGrill && grill.HasSubGrills())
        {
          hasSubGrill = true;
        }
      }
    }

    grills.Sort((a, b) => Vector3.SqrMagnitude(a.transform.position - endPosition).CompareTo(Vector3.SqrMagnitude(b.transform.position - endPosition)));
  }

  public void CalculatePositions()
  {
    if (conveyData.grillIds.Count == 0) return;

    int count = conveyData.grillIds.Count;
    startPosition = transform.position - (moveType == MoveType.Horizontal ? Vector3.right : Vector3.up) * spacing * 0.5f * count;
    endPosition = transform.position + (moveType == MoveType.Horizontal ? Vector3.right : Vector3.up) * spacing * 0.5f * count;
    if (conveyData.speed < 0)
    {
      var temp = startPosition;
      startPosition = endPosition;
      endPosition = temp;
    }
  }
  public void SetData(ConveyorData conveyData)
  {
    if (conveyData.speed == 0) conveyData.speed = -1;
    this.conveyData = conveyData;
    transform.position = conveyData.position.ToVector3();
    materialVisual = Instantiate(visualMaterial);
    CalculatePositions();
    GetGrills();
    paused = false;
    moving = true;
    direction = moveType == MoveType.Horizontal
            ? (conveyData.speed > 0 ? Vector3.right : Vector3.left)
            : (conveyData.speed > 0 ? Vector3.up : Vector3.down);
    // StartMovement();
  }

  private void StartMovement()
  {
    conveySpeed = direction * Mathf.Clamp(Mathf.Abs(conveyData.speed), -1, 1);
    visualSpeed = visualRatio * conveySpeed;
    materialVisual.SetVector("_UVScrollSpeed", visualSpeed);
    visual.material = materialVisual;
    StartCoroutine(IEMovement());
  }

  IEnumerator IEMovement()
  {
    //Vector3 movementDirection = direction; // * Mathf.Abs(conveyData.speed);
    PrimaryGrill firstGrill = grills[0];
    while (true)
    {
      if (moving)
      {
        if (paused)
        {
          paused = false;
          materialVisual.SetVector("_UVScrollSpeed", visualSpeed);
        }

        foreach (var grill in grills)
        {
          grill.transform.Translate(conveySpeed * Time.deltaTime);
        }

        if ((firstGrill.transform.position - endPosition).sqrMagnitude < 0.2f)
        {
          grills.Remove(firstGrill);
          Vector3 rePos = grills[grills.Count - 1].transform.position - direction * spacing;
          firstGrill.transform.position = GetRePosition(rePos);
          firstGrill.OnResetConveyorCircle();
          grills.Add(firstGrill);
          firstGrill = grills[0];
        }
      }
      else if (!paused)
      {
        paused = true;
        materialVisual.SetVector("_UVScrollSpeed", Vector3.zero);
      }

      yield return null;
    }
  }
  private Vector3 GetRePosition(Vector3 position)
  {
    if (moveType == MoveType.Vertical) return position;
    if (conveyData.speed > 0)
    {
      if (position.x > startPosition.x) return startPosition;
    }
    else
    {
      if (position.x < startPosition.x) return startPosition;
    }

    return position;
  }
  public void Setup()
  {
  }

  public virtual void OnCreateObj(params object[] args)
  {
    paused = false;
  }

  public virtual void OnReturnObj()
  {
    StopAllCoroutines();
    paused = false;
  }
}
