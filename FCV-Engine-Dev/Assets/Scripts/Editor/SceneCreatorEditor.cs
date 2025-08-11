using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

/// <summary>
/// Provides an editor window for creating new Unity scenes by duplicating a base scene. Allows users to specify the new scene name and base scene, checks for unsaved changes in the current scene, and adds the new scene to the build settings.
/// </summary>
public class SceneCreatorEditor : EditorWindow
{
    protected string NewSceneName;

    protected SceneAsset CopySceneAsset;

    protected readonly GUIContent NameContent = new GUIContent("New Scene Name");
    
    /// <summary>
    /// Initializes the editor window and sets the default new scene name.
    /// </summary>
    [MenuItem("Tools/Create New Scene...", priority = 100)]
    private static void Init()
    {
        SceneCreatorEditor window = GetWindow<SceneCreatorEditor>();
        window.Show();
        window.NewSceneName = "NewScene";
    }

    private void CreateGUI()
    {
        string[] results = AssetDatabase.FindAssets("DemoScene");
        
        if (results.Length <= 0) return;
        
        SceneAsset demoScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(results[0]));
        CopySceneAsset = demoScene;
    }

    private void OnGUI()
    {
        NewSceneName = EditorGUILayout.TextField(NameContent, NewSceneName);
        
        CopySceneAsset = (SceneAsset)EditorGUILayout.ObjectField(CopySceneAsset, typeof(SceneAsset), false);

        if (GUILayout.Button("Create"))
            CheckAndCreateScene();
    }
    
    /// <summary>
    /// Checks whether the current scene has unsaved changes and prompts the user to save them before creating a new scene. Prevents scene creation in play mode or without a specified scene name.
    /// </summary>
    protected void CheckAndCreateScene()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Cannot create scenes while in play mode.  Exit play mode first.");
            return;
        }

        if (string.IsNullOrEmpty(NewSceneName))
        {
            Debug.LogWarning("Please enter a scene name before creating a scene.");
            return;
        }

        Scene currentActiveScene = SceneManager.GetActiveScene();

        if (currentActiveScene.isDirty)
        {
            string title = currentActiveScene.name + " Has Been Modified";
            string message = "Do you want to save the changes you made to " + currentActiveScene.path +
                             "?\nChanges will be lost if you don't save them.";
            int option = EditorUtility.DisplayDialogComplex(title, message, "Save", "Don't Save", "Cancel");

            if (option == 0)
            {
                EditorSceneManager.SaveScene(currentActiveScene);
            }
            else if (option == 2)
            {
                return;
            }
        }

        CreateScene();
    }
    
    /// <summary>
    /// Creates a new scene by duplicating the specified base scene. Opens the new scene and adds it to the build settings.
    /// </summary>
    protected void CreateScene()
    {
        if (!CopySceneAsset)
        {
            EditorUtility.DisplayDialog("Error",
                "No scene to copy was assigned, this needs to be assigned so that we have a base scene to copy from",
                "OK");
            return;
        }
        
        string newScenePath = "Assets/Scenes/" + NewSceneName + ".unity";
        
        CopyAndMakeNewScene(AssetDatabase.GetAssetPath(CopySceneAsset), newScenePath);
        Scene newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
        EditorSceneUtility.AddSceneToBuildSettings(AssetDatabase.LoadAssetAtPath<SceneAsset>(newScene.path));
        Close();
    }

    /// <summary>
    /// Copies a scene from the given path and saves it at the specified new scene path. Refreshes the AssetDatabase after the operation.
    /// </summary>
    /// <param name="path">Path to the scene to copy.</param>
    /// <param name="newScenePath">Destination path for the new scene.</param>
     protected void CopyAndMakeNewScene(string path, string newScenePath)
    {
        AssetDatabase.CopyAsset(path, newScenePath);
        AssetDatabase.Refresh();
    }
}