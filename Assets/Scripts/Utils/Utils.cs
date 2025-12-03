

using System;
using UnityEngine;

public static class Utils
{
  public static void SetAlpha(this SpriteRenderer sprite, float alpha)
  {
    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
  }


  public static T[] RemoveAt<T>(T[] array, int index)
  {
    if (array == null || index < 0 || index >= array.Length)
      throw new ArgumentOutOfRangeException(nameof(index));

    T[] result = new T[array.Length - 1];

    for (int i = 0, j = 0; i < array.Length; i++)
    {
      if (i == index) continue;
      result[j++] = array[i];
    }

    return result;
  }

  public static void SetLocalPositionY(this Transform transform, float y)
  {
    transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
  }
}
