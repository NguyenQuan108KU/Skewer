


using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LoadItemSO : EditorWindow
{
  private ItemSO itemSO;
  private string folderPath;
  [MenuItem("GrillSort/LoadItemSO")]
  private static void ShowWindow()
  {
    var window = GetWindow<LoadItemSO>();
    window.titleContent = new GUIContent("LoadItemSO");
    window.Show();
  }

  private void OnGUI()
  {
    GUILayout.Label("Select ItemSO", EditorStyles.boldLabel);
    itemSO = (ItemSO)EditorGUILayout.ObjectField("Item SO", itemSO, typeof(ItemSO), false);
    GUILayout.Space(10);
    GUILayout.Label("Select Folder", EditorStyles.boldLabel);

    if (GUILayout.Button("Select Folder"))
    {
      string path = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
      if (!string.IsNullOrEmpty(path))
      {
        folderPath = path;
      }
    }
    EditorGUILayout.LabelField("Alone Folder", folderPath);
    GUILayout.Space(10);
    if (!string.IsNullOrEmpty(folderPath) && itemSO != null)
    {
      if (GUILayout.Button("Load Item Data"))
      {
        LoadItem(folderPath);
      }
    }
  }

  private void LoadItem(string folderPath)
  {
    string[] files = System.IO.Directory.GetFiles(folderPath).Where(f => !f.EndsWith(".meta"))
                                  .ToArray();
    Debug.Log($"Found {files.Length} files in folder.");

    var sorted = files.OrderBy(p => p, FileSortUtil.ExplorerFileNameComparer) // so theo tên file kiểu Explorer
    .ToArray();
    List<ItemDataSO> itemDataList = new List<ItemDataSO>();
    int index = 1;
    foreach (var f in sorted)
    {
      string relativePath = f.Replace(Application.dataPath, "Assets");
      ItemDataSO itemData = new ItemDataSO();
      itemData.id = index++;
      itemData.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
      if (itemData.sprite != null)
      {
        itemDataList.Add(itemData);
      }
      else
      {
        Debug.LogWarning($"Sprite not found at path: {relativePath}");
      }
    }

    // Save the item data to the ItemSO
    itemSO.itemDataList = itemDataList;
    EditorUtility.SetDirty(itemSO);
    AssetDatabase.SaveAssets();
  }
}
