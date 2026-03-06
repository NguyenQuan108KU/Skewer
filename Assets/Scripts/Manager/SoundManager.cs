

using System.Collections.Generic;
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
  [SerializeField] private AudioClip dropGrill;
  [SerializeField] private AudioClip boxAppear;
  [SerializeField] private AudioClip warning;
  [SerializeField] private AudioClip sfxTuttkile;
  [SerializeField] private AudioClip boxClosed;
  [SerializeField] private AudioClip soundEasy;
  [SerializeField] private AudioClip soundHard;
  [SerializeField] private List<AudioClip> obstacleIceSfxList;
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
      case SoundType.DropGrill:
        if (!dropGrill) return;
        PlaySound(dropGrill);
        break;
      case SoundType.BoxAppear:
        if (!boxAppear) return;
        PlaySound(boxAppear);
        break;
      case SoundType.Warning:
        if (!warning) return;
        PlaySound(warning);
        break;
      case SoundType.BoxClosed:
        if (!boxClosed) return;
        PlaySound(boxClosed);
        break;
            case SoundType.Tuttkile:
                if (!sfxTuttkile) return;
                PlaySound(sfxTuttkile);
                break;
            case SoundType.Easy:
                if (!soundEasy) return;
                PlaySound(soundEasy);
                break;
            case SoundType.Hard:
                if (!soundHard) return;
                PlaySound(soundHard);
                break;
            default:
        Debug.LogWarning("Sound type not handled: " + soundType);
        break;

    }
  }
  public void PlayObstacleIceSound(int state)
  {
    if (state < 0 || state >= obstacleIceSfxList.Count) return;
    var clip = obstacleIceSfxList[state];
    if (clip == null) return;
    PlaySound(clip);
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
  DoneOrder,
  DropGrill,
  BoxAppear,
  Warning,
  Tuttkile,
  BoxClosed,
  Easy,
  Hard
}
