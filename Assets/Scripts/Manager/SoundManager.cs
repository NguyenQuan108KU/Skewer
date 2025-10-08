

using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{
  [SerializeField] private AudioSource bgmSource;
  [SerializeField] private AudioSource sfxSource;

  [LunaPlaygroundAsset(fieldSection: "Background Settings")]
  [SerializeField] private AudioClip BGM;
  [SerializeField] private AudioClip Win;
  [SerializeField] private AudioClip Lose;
  [SerializeField] private AudioClip Confetti;

  [SerializeField] private AudioClip itemPick;
  [SerializeField] private AudioClip itemMerge;
  [SerializeField] private AudioClip doneOrder;

  public void PlaySound(AudioClip audioClip, float volume = 1)
  {
    if (audioClip == null) return;
    sfxSource.PlayOneShot(audioClip, volume);
  }

  public void PlaySound(SoundType soundType)
  {
    switch (soundType)
    {
      case SoundType.BGM:
        if (bgmSource.isPlaying || BGM == null) return;
        bgmSource.clip = BGM;
        bgmSource.Play();
        break;
      case SoundType.Win:
        if (!Win) return;
        PlaySound(Win);
        break;
      case SoundType.Lose:
        if (!Lose) return;
        PlaySound(Lose);
        break;
      case SoundType.Confetti:
        if (!Confetti) return;
        PlaySound(Confetti);
        break;
      case SoundType.ItemPick:
        if (!itemPick) return;
        PlaySound(itemPick);
        break;
      case SoundType.ItemMerge:
        if (!itemMerge) return;
        PlaySound(itemMerge);
        break;
      case SoundType.DoneOrder:
        if (!doneOrder) return;
        PlaySound(doneOrder);
        break;
      default:
        Debug.LogWarning("Sound type not handled: " + soundType);
        break;

    }
  }
}


public enum SoundType
{
  BGM,
  Win,
  Lose,
  Confetti,
  ItemPick,
  ItemMerge,
  DoneOrder
}
