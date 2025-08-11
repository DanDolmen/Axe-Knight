#if UNITY_EDITOR
using UnityEditor;
using Utility;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// Handles scene transitions in Unity. Provides functionality for switching scenes with optional persistence of a game object across scenes. Automatically validates and adds the target scene to build settings in the editor.
/// </summary>
public class SceneSwitcher : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _sceneAsset;
#endif

    private string scenePath;

    /// <summary>
    /// Starts a scene transition to the specified scene. Ensures the provided game object, if any, persists across scene loads.
    /// </summary>
    /// <param name="transitioningGameObject">The game object to persist across scenes, or null if no persistence is required.</param>
    public void StartTransition(GameObject transitioningGameObject = null)
    {
        if (transitioningGameObject)
            DontDestroyOnLoad(transitioningGameObject);
        
        SceneManager.LoadScene(scenePath);
    }
#if UNITY_EDITOR
    
    /// <summary>
    /// Validates the target scene asset in the editor. Ensures the scene's path is in the build settings and updates the internal path reference.
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
