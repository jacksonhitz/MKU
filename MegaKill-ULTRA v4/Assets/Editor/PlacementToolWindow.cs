using System;
using System.Collections.Generic;
using EditorTools;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class PlacementToolWindow : EditorWindow
{
    private static int toolSelected;
    private Vector2 overallScroll;
    private Vector2 prefabsScroll;
    private int prefabIndex;
    private int visualIndex;

    private static List<Transform> parentHistory = new List<Transform>();
    private static int parentHistoryIndex;

    private GUIContent[] gridContents;
    private int[] gridIndices;
    private GUIStyle prefabButtonStyle;

    const float pBoxWidth = 100f;
    const float pBoxHeight = 60f;

    [MenuItem("Tools/Placement Tools")]
    static void Init()
    {
        var window = GetWindow<PlacementToolWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        SetStyles();
        DrawTools();
    }

    private void SetStyles()
    {
        prefabButtonStyle = new GUIStyle(GUI.skin.button);
        prefabButtonStyle.imagePosition = ImagePosition.ImageAbove;
        prefabButtonStyle.fixedHeight = pBoxHeight;
        prefabButtonStyle.fixedWidth = pBoxWidth;
    }

    private void DrawTools()
    {
        overallScroll = EditorGUILayout.BeginScrollView(overallScroll);
        if (ToolManager.activeToolType == typeof(EnemyPlacementTool))
        {
            toolSelected = 3;
        }
        else if (ToolManager.activeToolType == typeof(ContainerPlacementTool))
        {
            toolSelected = 2;
        }
        else if (ToolManager.activeToolType == typeof(PrefabPlacementTool))
        {
            toolSelected = 1;
        }
        else if (ToolManager.activeToolType == typeof(ItemPlacementTool))
        {
            toolSelected = 4;
        }
        else if (ToolManager.activeToolType != typeof(NotePlacementTool))
        {
            toolSelected = 0;
        }
        else
        {
            toolSelected = -1;
        }
        var newToolSelected = GUILayout.Toolbar(
            toolSelected,
            new[] { "Notes", "Prefab", "Container", "Enemy", "Pickup" }
        );
        if (newToolSelected != toolSelected)
        {
            toolSelected = newToolSelected;
            switch (toolSelected)
            {
                case 0:
                    //ToolManager.SetActiveTool<NotePlacementTool>();
                    break;
                case 1:
                    ToolManager.SetActiveTool<PrefabPlacementTool>();
                    break;
                case 2:
                    //ToolManager.SetActiveTool<ContainerPlacementTool>();
                    break;
                case 3:
                    //ToolManager.SetActiveTool<EnemyPlacementTool>();
                    break;
                case 4:
                    // ToolManager.SetActiveTool<ItemPlacementTool>();
                    break;
            }
        }
        switch (toolSelected)
        {
            case 1:
                DrawPlacementData();
                break;
            case 2:
                // DrawContainerData();
                break;
            case 3:
                //DrawEnemyData();
                break;
            case 4:
                DrawItemData();
                break;
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawEnemyData()
    {
        var data = PrefabPlacementData.GetPlacementData();
        var so = new SerializedObject(data);
        var enemyProp = so.FindProperty(nameof(PrefabPlacementData.selectedEnemy));
        EditorGUILayout.PropertyField(enemyProp);
        EnemyPlacementTool.EnemyType = enemyProp.objectReferenceValue as Enemy;
        so.ApplyModifiedProperties();
    }

    private void DrawPlacementData()
    {
        var data = PrefabPlacementData.GetPlacementData();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+ Add New"))
        {
            var newPrefab = new PrefabPlacementData.PrefabEntry();
            data.entries.Add(newPrefab);
            if (data.selectedCategory != -1)
            {
                newPrefab.category = data.selectedCategory;
            }
        }
        if (GUILayout.Button("++ Duplicate"))
        {
            if (prefabIndex > -1 && prefabIndex < data.entries.Count)
            {
                data.entries.Add(new PrefabPlacementData.PrefabEntry(data.entries[prefabIndex]));
                SetGridContents(data.entries, data.selectedCategory);
                visualIndex = Math.Max(0, gridContents.Length - 1);
            }
        }
        if (GUILayout.Button("- Delete"))
        {
            if (prefabIndex > -1 && prefabIndex < data.entries.Count)
            {
                data.entries.RemoveAt(prefabIndex);
                SetGridContents(data.entries, data.selectedCategory);
                if (visualIndex >= gridContents.Length)
                {
                    visualIndex = Math.Max(0, gridContents.Length - 1);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        prefabsScroll = EditorGUILayout.BeginScrollView(
            prefabsScroll,
            false,
            false,
            GUILayout.MinHeight(pBoxHeight + 35f),
            GUILayout.MaxHeight(pBoxHeight * 4f + 35f)
        );
        var displayedCats = new string[data.categoryNames.Length + 1];
        displayedCats[0] = "All";
        data.categoryNames.CopyTo(displayedCats, 1);
        data.selectedCategory = EditorGUILayout.Popup(data.selectedCategory + 1, displayedCats) - 1;
        SetGridContents(data.entries, data.selectedCategory);
        EditorGUILayout.EndScrollView();
        if (prefabIndex > -1 && prefabIndex < data.entries.Count)
        {
            DrawPrefabDataEditor(data, prefabIndex);
            PrefabPlacementTool.prefab = data.entries[prefabIndex];
        }
        var newParent = (Transform)
            EditorGUILayout.ObjectField(
                "Parent",
                PrefabPlacementTool.Parent,
                typeof(Transform),
                true
            );
        SetParentWithHistory(newParent);
        EditorGUILayout.BeginHorizontal();
        using (new EditorGUI.DisabledScope(parentHistoryIndex <= 0))
        {
            if (GUILayout.Button("Back"))
            {
                parentHistoryIndex--;
                PrefabPlacementTool.Parent = parentHistory[parentHistoryIndex];
            }
        }
        using (new EditorGUI.DisabledScope(parentHistoryIndex >= parentHistory.Count - 1))
        {
            if (GUILayout.Button("Forward"))
            {
                parentHistoryIndex++;
                PrefabPlacementTool.Parent = parentHistory[parentHistoryIndex];
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SetGridContents(
        IReadOnlyList<PrefabPlacementData.PrefabEntry> entries,
        int category = -1
    )
    {
        if (gridContents == null)
        {
            gridIndices = new int[entries.Count];
            gridContents = new GUIContent[entries.Count];
        }
        else if (gridContents.Length != entries.Count)
        {
            System.Array.Resize(ref gridIndices, entries.Count);
            System.Array.Resize(ref gridContents, entries.Count);
        }

        if (entries.Count == 0)
        {
            prefabIndex = -1;
            return;
        }
        var j = 0;
        var i = 0;
        for (; i < entries.Count; i++)
        {
            if (category != -1 && entries[i].category != category)
                continue;
            var prefab = entries[i].prefab;
            var name = prefab == null ? "(empty)" : prefab.name;
            var preview = prefab == null ? null : AssetPreview.GetAssetPreview(prefab);
            gridIndices[j] = i;
            gridContents[j] = new GUIContent(name, preview);
            j++;
        }
        if (i != j)
        {
            System.Array.Resize(ref gridIndices, i);
            System.Array.Resize(ref gridContents, j);
        }
        var xCount = Mathf.RoundToInt((Screen.width) / (pBoxWidth)) - 1;
        visualIndex = GUILayout.SelectionGrid(
            Math.Min(visualIndex, gridContents.Length - 1),
            gridContents,
            xCount,
            prefabButtonStyle
        );
        try
        {
            prefabIndex = gridIndices[visualIndex];
        }
        catch (Exception) { }
    }

    static void SetParentWithHistory(Transform newParent)
    {
        if (newParent != PrefabPlacementTool.Parent)
        {
            if (parentHistory.Count > 0 && parentHistoryIndex != parentHistory.Count - 1)
            {
                parentHistory.RemoveRange(
                    parentHistoryIndex + 1,
                    parentHistory.Count - parentHistoryIndex - 1
                ); // trim branching tail
            }
            parentHistory.Add(newParent);
            if (parentHistory.Count > 32)
            {
                parentHistory.RemoveAt(0);
            }
            parentHistoryIndex = parentHistory.Count - 1;
            PrefabPlacementTool.Parent = newParent;
        }
    }

    // private void DrawContainerData()
    // {
    //     var data = PrefabPlacementData.GetPlacementData();
    //     EditorGUILayout.BeginHorizontal();
    //     if (GUILayout.Button("+ Add New"))
    //     {
    //         data.containerEntries.Add(new PrefabPlacementData.ContainerEntry());
    //         prefabIndex = Mathf.Max(0, prefabIndex - 1);
    //     }
    //     if (GUILayout.Button("++ Duplicate"))
    //     {
    //         if (prefabIndex < data.containerEntries.Count)
    //         {
    //             data.containerEntries.Add(
    //                 new PrefabPlacementData.ContainerEntry(data.containerEntries[prefabIndex])
    //             );
    //             prefabIndex = data.containerEntries.Count - 1;
    //         }
    //     }
    //     if (GUILayout.Button("- Delete"))
    //     {
    //         if (prefabIndex < data.containerEntries.Count)
    //         {
    //             data.containerEntries.RemoveAt(prefabIndex);
    //             prefabIndex = Mathf.Max(0, prefabIndex - 1);
    //         }
    //     }
    //     EditorGUILayout.EndHorizontal();
    //     prefabsScroll = EditorGUILayout.BeginScrollView(
    //         prefabsScroll,
    //         false,
    //         false,
    //         GUILayout.MinHeight(pBoxHeight + 20f),
    //         GUILayout.MaxHeight(pBoxHeight * 2f + 20f)
    //     );
    //     if (gridContents == null)
    //     {
    //         gridContents = new GUIContent[data.containerEntries.Count];
    //     }
    //     else if (gridContents.Length != data.containerEntries.Count)
    //     {
    //         System.Array.Resize(ref gridContents, data.containerEntries.Count);
    //     }
    //     for (var i = 0; i < data.containerEntries.Count; i++)
    //     {
    //         var prefab = data.containerEntries[i].prefab;
    //         var name = prefab == null ? "(empty)" : prefab.name;
    //         var preview = prefab == null ? null : AssetPreview.GetAssetPreview(prefab);
    //         gridContents[i] = new GUIContent(name, preview);
    //     }
    //     var xCount = Mathf.RoundToInt((Screen.width) / (pBoxWidth)) - 1;
    //     prefabIndex = GUILayout.SelectionGrid(prefabIndex, gridContents, xCount, prefabButtonStyle);
    //     EditorGUILayout.EndScrollView();
    //     if (prefabIndex < data.containerEntries.Count)
    //     {
    //         DrawContainerDataEditor(data, prefabIndex);
    //         PrefabPlacementTool.p = data.containerEntries[prefabIndex];
    //     }
    // }

    private void DrawPrefabDataEditor(PrefabPlacementData data, int index)
    {
        var so = new SerializedObject(data);
        var entriesProp = so.FindProperty(nameof(PrefabPlacementData.entries));
        var element = entriesProp.GetArrayElementAtIndex(index);
        var prefabProp = element.FindPropertyRelative(
            nameof(PrefabPlacementData.PrefabEntry.prefab)
        );
        var categoryProp = element.FindPropertyRelative(
            nameof(PrefabPlacementData.PrefabEntry.category)
        );
        element.isExpanded = true;
        EditorGUILayout.ObjectField(prefabProp, typeof(GameObject));
        categoryProp.intValue = EditorGUILayout.Popup(
            "Category",
            categoryProp.intValue,
            data.categoryNames
        );
        var dataElement = element.FindPropertyRelative(
            nameof(PrefabPlacementData.PrefabEntry.placementData)
        );
        dataElement.isExpanded = true;
        EditorGUILayout.PropertyField(dataElement);
        element.serializedObject.ApplyModifiedProperties();
    }

    // private void DrawContainerDataEditor(PrefabPlacementData data, int index)
    // {
    //     var so = new SerializedObject(data);
    //     var entriesProp = so.FindProperty(nameof(PrefabPlacementData.containerEntries));
    //     var element = entriesProp.GetArrayElementAtIndex(index);
    //     var prefabProp = element.FindPropertyRelative(
    //         nameof(PrefabPlacementData.ContainerEntry.prefab)
    //     );
    //     element.isExpanded = true;
    //     EditorGUILayout.ObjectField(prefabProp, typeof(GameObject));
    //     if (!CheckValidContainer(prefabProp.objectReferenceValue))
    //     {
    //         if (prefabProp.objectReferenceValue != null)
    //             Debug.LogWarning("That's not a container!");
    //         prefabProp.objectReferenceValue = null;
    //     }
    //     element
    //         .FindPropertyRelative(nameof(PrefabPlacementData.ContainerEntry.placementData))
    //         .isExpanded = true;
    //     element
    //         .FindPropertyRelative(nameof(PrefabPlacementData.ContainerEntry.containerData))
    //         .isExpanded = true;
    //     EditorGUILayout.PropertyField(element);
    //     element.serializedObject.ApplyModifiedProperties();
    // }

    private void DrawItemData()
    {
        PrefabPlacementData data = PrefabPlacementData.GetPlacementData();
        var so = new SerializedObject(data);
        var itemData = so.FindProperty(nameof(PrefabPlacementData.itemData));
        EditorGUILayout.PropertyField(itemData);
        itemData.serializedObject.ApplyModifiedProperties();
    }

    // private bool CheckValidContainer(Object obj)
    // {
    //     var go = obj as GameObject;
    //     if (go == null)
    //         return false;
    //     if (go.GetComponentInChildren<ItemContainer>() == null)
    //         return false;
    //     return true;
    // }
}
