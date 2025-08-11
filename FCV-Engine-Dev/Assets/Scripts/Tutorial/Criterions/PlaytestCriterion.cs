using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

public class PlaytestCriterion : Criterion
{
    private bool enteredPlaymode = false;
    private bool exitedPlaymode = false;
    public override void StartTesting()
    {
        base.StartTesting();
        
        enteredPlaymode = false;
        exitedPlaymode = false;

        EditorApplication.playModeStateChanged += PlaymodeChanged;
        EditorApplication.update += UpdateCompletion;
    }

    private void PlaymodeChanged(PlayModeStateChange change)
    {
        Debug.Log("Playmode changed: " + change);
        if (change is PlayModeStateChange.EnteredPlayMode or PlayModeStateChange.ExitingEditMode)
        {
            enteredPlaymode = true;
        }

        if (enteredPlaymode && change is PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.EnteredEditMode)
        {
            exitedPlaymode = true;
        }
    }

    public override void StopTesting()
    {
        base.StopTesting();
        EditorApplication.playModeStateChanged -= PlaymodeChanged;
        EditorApplication.update -= UpdateCompletion;
    }

    protected override bool EvaluateCompletion()
    {
        return enteredPlaymode && exitedPlaymode;
    }

    public override bool AutoComplete()
    {
        return false;
    }
}
