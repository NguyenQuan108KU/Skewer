// Assets/Editor/ItemSOExtractorWindow.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// YÊU CẦU: Các class LevelData/GrillData/LayerData/ItemData/Vector3Data
// đã có sẵn trong project (runtime). Ở đây EditorWindow chỉ dùng chúng để parse JSON.

public class ItemSOExtractorWindow : EditorWindow
{
  [Header("Inputs")]
  public ItemSO sourceItemSO;        // SO đầy đủ
  public TextAsset levelJson;        // JSON LevelData

  [Header("Options")]
  public bool onlyPositiveIds = true;
  public bool sortAscending = true;
  public bool logSummary = true;

  [MenuItem("Tools/ItemSO/Extract From Level")]
  public static void Open()
  {
    var win = GetWindow<ItemSOExtractorWindow>("Extract ItemSO From Level");
    win.minSize = new Vector2(420, 260);
  }

  private void OnGUI()
  {
    EditorGUILayout.LabelField("Source Data", EditorStyles.boldLabel);
    sourceItemSO = (ItemSO)EditorGUILayout.ObjectField("Source ItemSO (Full)", sourceItemSO, typeof(ItemSO), false);
    levelJson = (TextAsset)EditorGUILayout.ObjectField("LevelData JSON", levelJson, typeof(TextAsset), false);

    EditorGUILayout.Space(6);
    EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
    onlyPositiveIds = EditorGUILayout.Toggle(new GUIContent("Only IDs > 0", "Bỏ qua id <= 0 khi quét level"), onlyPositiveIds);
    sortAscending = EditorGUILayout.Toggle(new GUIContent("Sort Result By ID", "Sắp xếp danh sách id kết quả"), sortAscending);
    logSummary = EditorGUILayout.Toggle(new GUIContent("Log Summary", "In log số lượng id và id nào không tìm thấy trong SO"), logSummary);

    EditorGUILayout.Space(12);

    using (new EditorGUI.DisabledScope(sourceItemSO == null || levelJson == null))
    {
      if (GUILayout.Button("Preview IDs From Level"))
      {
        TryPreviewIds();
      }

      EditorGUILayout.Space(6);

      if (GUILayout.Button("Create Filtered ItemSO Asset..."))
      {
        TryCreateFilteredSO();
      }
    }

    if (sourceItemSO == null || levelJson == null)
    {
      EditorGUILayout.HelpBox("Chọn Source ItemSO và LevelData JSON để tiếp tục.", MessageType.Info);
    }
  }

  private void TryPreviewIds()
  {
    if (!ValidateInputs()) return;

    if (!TryParseLevel(levelJson, out LevelData level))
      return;

    var ids = CollectUniqueIds(level, onlyPositiveIds);
    if (sortAscending)
      ids.Sort();

    var msg = $"[Preview] Found {ids.Count} unique ids in LevelData.\nIDs: {string.Join(", ", ids)}";
    Debug.Log(msg);
    EditorUtility.DisplayDialog("Preview IDs", $"Found {ids.Count} unique ids.\n(Chi tiết xem Console)", "OK");
  }

  private void TryCreateFilteredSO()
  {
    if (!ValidateInputs()) return;

    if (!TryParseLevel(levelJson, out LevelData level))
      return;

    // 1) Lấy danh sách id duy nhất trong LevelData
    var ids = CollectUniqueIds(level, onlyPositiveIds);
    if (sortAscending)
      ids.Sort();

    // 2) Lọc từ sourceItemSO theo ids (không mutate sourceItemSO; clone entry)
    var resultList = new List<ItemDataSO>();
    var notFoundIds = new List<int>();

    foreach (var id in ids)
    {
      var found = sourceItemSO.itemDataList?.Find(e => e != null && e.id == id);
      if (found != null)
      {
        // Clone để asset mới không dùng chung instance với asset gốc
        var clone = new ItemDataSO
        {
          id = found.id,
          sprite = found.sprite
        };
        resultList.Add(clone);
      }
      else
      {
        notFoundIds.Add(id);
      }
    }

    if (logSummary)
    {
      Debug.Log($"[Create] IDs in Level: {ids.Count}, Matched in SO: {resultList.Count}, Missing: {notFoundIds.Count}");
      if (notFoundIds.Count > 0)
        Debug.LogWarning($"[Create] Missing IDs (not in Source SO): {string.Join(", ", notFoundIds)}");
    }

    // 3) Tạo SO mới
    var savePath = EditorUtility.SaveFilePanelInProject(
        "Save Filtered ItemSO",
        "ItemSO_Filtered",
        "asset",
        "Chọn nơi lưu ScriptableObject mới");

    if (string.IsNullOrEmpty(savePath))
      return; // user cancel

    var newSO = ScriptableObject.CreateInstance<ItemSO>();
    newSO.itemDataList = resultList; // list đã clone

    AssetDatabase.CreateAsset(newSO, savePath);
    EditorUtility.SetDirty(newSO);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    EditorGUIUtility.PingObject(newSO);
    Selection.activeObject = newSO;

    EditorUtility.DisplayDialog("Done", $"Created: {savePath}\nCount: {resultList.Count}", "OK");
  }

  private bool ValidateInputs()
  {
    if (sourceItemSO == null)
    {
      EditorUtility.DisplayDialog("Error", "Chưa chọn Source ItemSO.", "OK");
      return false;
    }
    if (levelJson == null)
    {
      EditorUtility.DisplayDialog("Error", "Chưa chọn LevelData JSON (TextAsset).", "OK");
      return false;
    }
    if (sourceItemSO.itemDataList == null)
    {
      EditorUtility.DisplayDialog("Error", "Source ItemSO không có itemDataList.", "OK");
      return false;
    }
    return true;
  }

  private static bool TryParseLevel(TextAsset json, out LevelData level)
  {
    level = null;
    if (json == null || string.IsNullOrEmpty(json.text))
    {
      EditorUtility.DisplayDialog("Error", "LevelData JSON rỗng.", "OK");
      return false;
    }

    try
    {
      level = JsonUtility.FromJson<LevelData>(json.text);
    }
    catch (Exception e)
    {
      Debug.LogError($"Parse LevelData JSON failed: {e}");
      EditorUtility.DisplayDialog("Error", "Không parse được LevelData từ JSON.\nXem Console để biết chi tiết.", "OK");
      return false;
    }

    if (level == null || level.grillData == null)
    {
      EditorUtility.DisplayDialog("Error", "LevelData không hợp lệ hoặc thiếu grillData.", "OK");
      return false;
    }

    return true;
  }

  // Thu thập ID duy nhất bằng List (không HashSet)
  private static List<int> CollectUniqueIds(LevelData level, bool onlyPositive)
  {
    var uniqueIds = new List<int>();
    if (level?.grillData == null) return uniqueIds;

    foreach (var grill in level.grillData)
    {
      if (grill == null || grill.layer == null) continue;
      foreach (var layer in grill.layer)
      {
        if (layer == null || layer.itemData == null) continue;
        foreach (var item in layer.itemData)
        {
          if (item == null) continue;
          var id = item.id;
          if (onlyPositive && id <= 0) continue;
          if (!uniqueIds.Contains(id))
            uniqueIds.Add(id);
        }
      }
    }

    return uniqueIds;
  }
}
