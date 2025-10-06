public static class GameDefine
{
  public const float itemMoveBackSpeed = 15f;
  public const float itemMergeDuration = 0.2f;
  public const float itemCollectSpeed = 15f;
  public const float timeDelaySubGrill = 0.5f;
  public const float itemMovePrimaryDuration = 0.3f;
  public const float orderMoveSpeed = 5f;
  public const float itemScaleIntro = 0.5f;
  public const float grillLidAnim = 0.5f;
  public const float itemMoveBackDuration = 0.25f;

}

public enum GameState
{
  Loading = 0,
  Playing,
  Paused,
  GameOver,
  Tool,
  UsingBooster
}
public class ShuffleLayerData
{
  public bool canNotShuffle;
  public GrillBase grill;
  public LayerData layerData;
}


public enum StuckType
{
  OutOfTime = 0,
  OutOfMove = 1,
  OutOfTimeShipper = 3,
  OutOfMoveShipper = 4,
  SkewerJam_OutOfSpace = 5,
  SkewerJam_OutOfEnergy = 6
}
