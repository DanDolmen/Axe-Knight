using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

/// <summary>
/// Handles the main menu operations, such as starting the game and quitting the application.
/// It also manages adding scenes to the build settings for the editor.
/// </summary>
public class MainMenuHandler : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _sceneAsset;
#endif

    /// <summary>
    /// Scene path from scene asset
    /// </summary>
    private string scenePath;
    
    /// <summary>
    /// Loads a scene using scene path in class
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(scenePath);
    }
    
    /// <summary>
    /// Closes the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
    
#if UNITY_EDITOR
    
    /// <summary>
    /// Assigns the scene path from a SceneAsset variable, which is not compilable in builds.
    /// Also adds the scene to the build settings if it is not already present.
    /// </summary>
    private void OnValidate()
    {
        var sceneAssetPath = AssetDatabase.GetAssetPath(_sceneAsset);
        bool sceneAdded = false;
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (sceneAdded) continue;
            
            sceneAdded = scene.path == sceneAssetPath;
        }

        scenePath = sceneAssetPath;
        
        if (sceneAdded) return;
        
        EditorSceneUtility.AddSceneToBuildSettings(_sceneAsset);
    }
#endif
}
