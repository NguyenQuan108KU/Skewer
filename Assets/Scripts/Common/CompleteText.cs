using UnityEngine;

public class CompleteText : MonoBehaviour
{
  [SerializeField] SpriteRenderer spriteRenderer;
  [SerializeField] Sprite[] textSprites;

  public void SetRandomTextSprite()
  {
    int idx = Random.Range(0, textSprites.Length);
    spriteRenderer.sprite = textSprites[idx];
  }

  public void SetTextSprite(Sprite sprite)
  {
    spriteRenderer.sprite = sprite;
  }
}
