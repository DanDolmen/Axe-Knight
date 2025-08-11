#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Unity.Tutorials.Core.Editor;

[CreateAssetMenu(fileName = "PlayModeExitedCriterion", menuName = "Tutorials/Criterion/Play Mode Exited")]
public class PlayModeExitedCriterion : Criterion
{
    private bool wasPlaying;

    public override void StartTesting()
    {
        base.StartTesting();
        wasPlaying = EditorApplication.isPlaying;
        EditorApplication.update += UpdateCompletion;
    }

    public override void StopTesting()
    {
        base.StopTesting();
        EditorApplication.update -= UpdateCompletion;
    }

    protected override bool EvaluateCompletion()
    {
        return wasPlaying && !EditorApplication.isPlaying;
    }

    public override bool AutoComplete()
    {
        return true;
    }
}
#endif