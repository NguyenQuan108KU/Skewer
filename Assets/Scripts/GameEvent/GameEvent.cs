

using UnityEngine.Events;

public static class GameEvent
{
  private static UnityEvent<ScreenType> _onResizeScreen = new UnityEvent<ScreenType>();
  public static UnityEvent<ScreenType> OnResizeScreen => _onResizeScreen;
  private static UnityEvent _onSortGrill = new UnityEvent();
  public static UnityEvent OnSortGrill => _onSortGrill;
  private static UnityEvent _onUserFirstTouch = new UnityEvent();
  public static UnityEvent OnUserFirstTouch => _onUserFirstTouch;
  private static UnityEvent _onLoadedLevel = new UnityEvent();
  public static UnityEvent OnLoadedLevel => _onLoadedLevel;

    public static UnityEvent<Item> OnEffect = new UnityEvent<Item>();
    public static UnityEvent OnSetFirstGrill = new UnityEvent();
    public static UnityEvent<int> OnPlayComplete = new UnityEvent<int>();
    public static UnityEvent HideButton = new UnityEvent();
    public static UnityEvent ActiveButton = new UnityEvent();
    public static UnityEvent StartLevel2 = new UnityEvent();    
}
