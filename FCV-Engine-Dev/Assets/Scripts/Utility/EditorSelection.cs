using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class EditorSelection
{
    public static event System.Action OnEditorInteracted;
    private static string lastFolderPath;
    
    static EditorSelection()
    {
        EditorApplication.projectChanged += OnSelectionChanged;
        EditorApplication.searchChanged += OnSelectionChanged;
        EditorApplication.hierarchyChanged += OnSelectionChanged;
        Selection.selectionChanged += OnSelectionChanged;
        EditorApplication.update += CheckForFolderChange;
    }

    private static void OnSelectionChanged()
    {
        OnEditorInteracted?.Invoke();
    }
    
    private static void CheckForFolderChange()
    {
        string currentFolderPath = GetActiveFolderPath();

        if (string.IsNullOrEmpty(currentFolderPath) || currentFolderPath == lastFolderPath) return;
        
        lastFolderPath = currentFolderPath;
        OnSelectionChanged();
    }

    private static string GetActiveFolderPath()
    {
        // Use reflection to access the internal ProjectBrowser class
        var projectBrowserType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        
        if (projectBrowserType == null) return null;

        var projectBrowserInstance = projectBrowserType.GetField("s_LastInteractedProjectBrowser").GetValue(null);

        if (projectBrowserInstance == null) return null;

        var currentFolderPath = projectBrowserType.GetMethod("GetActiveFolderPath", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(projectBrowserInstance, null) as string;

        return currentFolderPath;
    }
}
