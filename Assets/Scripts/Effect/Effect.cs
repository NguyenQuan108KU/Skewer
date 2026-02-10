using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public GameObject effectPrefabs;
    private float soundTimer = 0f;
    public float soundInterval = 5f;
    public GameObject button;

    private void OnEnable()
    {
        GameEvent.OnEffect.AddListener(PlayEffect);
        GameEvent.HideButton.AddListener(HideButton);
        GameEvent.ActiveButton.AddListener(ActiveButton);
    }

    private void OnDisable()
    {
        GameEvent.OnEffect.RemoveListener(PlayEffect);
        GameEvent.HideButton.RemoveListener(HideButton);
        GameEvent.ActiveButton.RemoveListener(ActiveButton);
    }
    private void Update()
    {
        soundTimer += Time.deltaTime;

        if (soundTimer >= soundInterval)
        {
            soundTimer = 0f;
            SoundManager.Instance.PlaySound(SoundType.Tuttkile);
        }
    }
    public void PlayEffect(Item item)
    {
        if (effectPrefabs != null && !item.isCheck)
        {
            item.isCheck = true;
            var grill = item.GetComponentInParent<PrimaryGrill>();
            item.transform.SetParent(null, true);
            var itemRenderer = item.GetComponent<SpriteRenderer>();
            if (itemRenderer == null)
            {
                itemRenderer = item.GetComponentInChildren<SpriteRenderer>();
                if (itemRenderer == null) return;
            }
            int baseOrder = itemRenderer.sortingOrder;
            itemRenderer.sortingOrder = baseOrder + 2;
            var fx = Instantiate(effectPrefabs, item.transform.position, Quaternion.identity);
            var fxRenderer = fx.GetComponent<SpriteRenderer>();
            if (fxRenderer != null)
            {
                fxRenderer.sortingOrder = itemRenderer.sortingOrder - 1;
            }
            Destroy(grill.gameObject);
            StartCoroutine(DelayRefresh());
        }
    }
    IEnumerator DelayRefresh()
    {
        yield return null;
        GameEvent.OnSetFirstGrill?.Invoke();
    }
    public void HideButton()
    {
        button.SetActive(false);
    }
    public void ActiveButton()
    {
        button.SetActive(true);
    }
}
