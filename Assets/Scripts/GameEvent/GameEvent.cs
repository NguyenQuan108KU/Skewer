

using UnityEngine.Events;

public static class GameEvent
{
  private static UnityEvent<ScreenType> _onResizeScreen = new UnityEvent<ScreenType>();
  public static UnityEvent<ScreenType> OnResizeScreen => _onResizeScreen;
  private static UnityEvent _onSortGrill = new UnityEvent();
  public static UnityEvent OnSortGrill => _onSortGrill;
  private static UnityEvent _onUserFirstTouch = new UnityEvent();
  public static UnityEvent OnUserFirstTouch => _onUserFirstTouch;
}
