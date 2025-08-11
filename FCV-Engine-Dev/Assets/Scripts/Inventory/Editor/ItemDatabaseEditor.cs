using System;
using UnityEditor;
using UnityEngine;

public class ItemDatabaseEditor : EditorWindow
{
    private ItemDatabase itemDatabase;
    private int index;
    private string itemName;

    [MenuItem("Tools/Item Database Editor")]
    public static void ShowWindow()
    {
        GetWindow<ItemDatabaseEditor>("Item Data Editor");
    }

    private void CreateGUI()
    {
        GUILayout.Label("Item Database Editor", EditorStyles.boldLabel);
    }

    /// <summary>
    /// Holds functionality for all buttons in the itemDatabaseEditor (Create, Update, ClearAllItems)
    /// </summary>
    private void OnGUI()
    {
        itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>("Assets/EditorResources/ItemNameDatabase.asset");

        if (itemDatabase != null)
        {
            itemName = EditorGUILayout.TextField("Name", itemName);
            index = EditorGUILayout.IntField("index", index);
            if (index < 1) index = 1;

            if (GUILayout.Button("Add Item"))
            {
                if (!string.IsNullOrEmpty(itemName))
                {
                    itemDatabase.AddItem(itemName);
                    EditorUtility.SetDirty(itemDatabase);
                    AssetDatabase.SaveAssets();
                }
            }

            if (GUILayout.Button("Update Item"))
            {
                if (!string.IsNullOrEmpty(itemName))
                {
                    itemDatabase.UpdateName(index - 1, itemName);
                    EditorUtility.SetDirty(itemDatabase);
                    AssetDatabase.SaveAssets();
                }
            }

            if (GUILayout.Button("Clear All Items"))
            {
                itemDatabase.ClearItems();
                EditorUtility.SetDirty(itemDatabase);
                AssetDatabase.SaveAssets();
            }

            GUILayout.Space(10);
            GUILayout.Label("Current Items:", EditorStyles.boldLabel);
            int localIndex = 1;

            foreach (var item in itemDatabase.GetAllItemNames())
            {
                GUILayout.Label($"{localIndex} Name: {item}");
                localIndex++;
            }
        }
        else
        {
            GUILayout.Label("Please assign an Item Data ScriptableObject.");
        }
    }
}