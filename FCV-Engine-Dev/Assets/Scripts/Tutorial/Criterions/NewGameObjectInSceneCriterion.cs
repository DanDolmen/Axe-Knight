using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Criterion for checking if a new gameobject has been added to hierarchy in currently active scene
/// </summary>
public class NewGameObjectInSceneCriterion : Criterion
{
    private int initialGameObjectCount;
    
    public override void StartTesting()
    {
        base.StartTesting();
        
        initialGameObjectCount = SceneManager.GetActiveScene().rootCount;

        EditorApplication.hierarchyChanged += UpdateCompletion;
    }


    public override void StopTesting()
    {
        base.StopTesting();
        
        EditorApplication.hierarchyChanged -= UpdateCompletion;
    }

    protected override bool EvaluateCompletion()
    {
        return SceneManager.GetActiveScene().rootCount > initialGameObjectCount; ;
    }

    public override bool AutoComplete()
    {
        return false;
    }
}
