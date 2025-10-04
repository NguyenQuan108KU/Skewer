


using UnityEngine;

public class ItemPlaceHolder : MonoBehaviour
{
  [SerializeField] private SpriteRenderer spriteRenderer;

  public void SetSprite(Sprite sprite)
  {
    spriteRenderer.sprite = sprite;
  }

  public void Setup()
  {

  }

  public void OnCreateObj(params object[] args)
  {

  }

  public void OnReturnObj()
  {

  }
}
