using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;
using Utility;
/// <summary>
/// GUI for setting up Tutorials to work with the RefreshMasking class
/// </summary>
public class TutorialObjectSetup : EditorWindow
{
    private RefreshMasking refreshMasking;
    
    [MenuItem("Tools/TutorialObjectSetup...", priority = 100)]
    private static void Init()
    {
        TutorialObjectSetup window = GetWindow<TutorialObjectSetup>();
        window.Show();
    }

    private void OnGUI()
    {
        refreshMasking = (RefreshMasking) EditorGUILayout.ObjectField("Select RefreshMasking Object", refreshMasking , typeof(RefreshMasking), false);

        if (GUILayout.Button("Setup Masking Events"))
            SetupTutorials();
        
        if (GUILayout.Button("Remove Masking Events"))
            RemoveEventsFromTutorials();
    }

    private void RemoveEventsFromTutorials()
    {
        List<Tutorial> tutorials = ScriptableObjectFinder.FindAssetsOfType<Tutorial>();

        EventListenerAdder.RemoveListenerFromScriptableObjects(tutorials.ToArray());
    }

    private void SetupTutorials()
    {
        List<Tutorial> tutorials = ScriptableObjectFinder.FindAssetsOfType<Tutorial>();

        EventListenerAdder.AddListenerToTutorialObjects(tutorials.ToArray(), refreshMasking);
    }
    
    
}

