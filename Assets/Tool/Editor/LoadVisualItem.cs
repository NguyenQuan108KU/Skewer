

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoadVisualItem : EditorWindow
{
  private ItemSO visualItemSO;
  private string folderPath;
  [MenuItem("GrillSort/LoadVisualItem")]
  private static void ShowWindow()
  {
    var window = GetWindow<LoadVisualItem>();
    window.titleContent = new GUIContent("LoadVisualItem");
    window.Show();
  }

  private void OnGUI()
  {
    GUILayout.Label("Select VisualItemSO", EditorStyles.boldLabel);
    visualItemSO = (ItemSO)EditorGUILayout.ObjectField("VisualItem SO", visualItemSO, typeof(ItemSO), false);
    GUILayout.Space(10);
    if (visualItemSO != null)
    {
      if (GUILayout.Button("Load Visual Item Data"))
      {
        LoadVisual();
      }
    }
  }

  private void LoadVisual()
  {
    Item[] items = FindObjectsOfType<Item>();
    List<ItemDataSO> itemDataSOs = new List<ItemDataSO>();
    foreach (var item in items)
    {
      var itemData = visualItemSO.GetItemData(item.id);
      if (itemData != null)
      {
        if (itemDataSOs.Contains(itemData) == false)
        {
          itemDataSOs.Add(itemData);
        }
      }
      var slot = item.transform.parent.parent.TryGetComponent<SlotBase>(out var s) ? s : null;
      if (slot != null)
      {
        slot.SetItemManually(item);
        EditorUtility.SetDirty(slot);
      }

      if (itemData != null)
      {
        item.Visual.SetVisual(itemData.sprite);
      }
      else
      {
        Debug.LogWarning($"ItemData not found for id: {item.id} in VisualItemSO");
      }
      EditorUtility.SetDirty(item);
    }
    LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();
    if (levelGenerator != null)
    {
      levelGenerator.itemDataList = itemDataSOs;
      EditorUtility.SetDirty(levelGenerator);
    }
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
  }
}
