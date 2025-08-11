#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Tutorials.Core.Editor;

[CreateAssetMenu(fileName = "SceneIsLoadedCriterion", menuName = "Tutorials/Criterion/Scene Is Loaded")]
public class SceneIsLoadedCriterion : Criterion
{
    [SerializeField] private string m_ExpectedSceneName;

    protected override bool EvaluateCompletion()
    {
        return SceneManager.GetActiveScene().name == m_ExpectedSceneName;
    }

    public override void StartTesting()
    {
        base.StartTesting();
        EditorApplication.update += UpdateCompletion;
    }

    public override void StopTesting()
    {
        base.StopTesting();
        EditorApplication.update -= UpdateCompletion;
    }

    public override bool AutoComplete()
    {
        return true;
    }
}
#endif