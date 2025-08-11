using System;
using System.Reflection;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using Utility;

/// <summary>
/// This class handles Tutorial Objects events and subscribes RefreshMasking objects functions to all the tutorial for easy setup
/// </summary>
public static class EventListenerAdder
{
    /// <summary>
    /// Adds a listener to each Tutorial object provided
    /// </summary>
    /// <param name="eventAssets">The tutorials</param>
    /// <param name="targetAsset">RefreshMasking asset for subscribing its functions to the event</param>
    public static void AddListenerToTutorialObjects(Tutorial[] eventAssets, RefreshMasking targetAsset)
    {
        foreach (var asset in eventAssets)
        {
            if (asset == null) continue;
            
                // Add the persistent listener
                if (!HasListenerSubscribed(asset.PageInitiated, targetAsset.StartConstantRefreshing))
                {
                    UnityEventTools.AddPersistentListener(asset.PageInitiated, targetAsset.StartConstantRefreshing);
                    asset.PageInitiated.SetPersistentListenerState(asset.PageInitiated.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
                }
                if (!HasListenerSubscribed(asset.Completed, targetAsset.StopConstantRefreshing))
                {
                    UnityEventTools.AddPersistentListener(asset.Completed, targetAsset.StopConstantRefreshing);
                    asset.Completed.SetPersistentListenerState(asset.Completed.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
                }
                if (!HasListenerSubscribed(asset.GoingBack, targetAsset.StopConstantRefreshing))
                {
                    UnityEventTools.AddPersistentListener(asset.GoingBack, targetAsset.StopConstantRefreshing);
                    asset.GoingBack.SetPersistentListenerState(asset.GoingBack.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
                }
                if (!HasListenerSubscribed(asset.Quit, targetAsset.StopConstantRefreshing))
                {
                    UnityEventTools.AddPersistentListener(asset.Quit, targetAsset.StopConstantRefreshing);
                    asset.Quit.SetPersistentListenerState(asset.Quit.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
                }
                


                // Mark the asset as dirty so changes are saved
                EditorUtility.SetDirty(asset);
        }

        // Save changes
        AssetDatabase.SaveAssets();
    }
    /// <summary>
    /// Checks if a UnityEventBase has unityaction subscribed
    /// </summary>
    /// <param name="eventBase">Event to check against</param>
    /// <param name="action">The function to check</param>
    /// <returns>true if it has function subscribed</returns>
    private static bool HasListenerSubscribed(UnityEventBase eventBase, UnityAction action)
    {
        for (int i = 0; i < eventBase.GetPersistentEventCount(); i++)
        {
            Debug.Log(eventBase.GetPersistentMethodName(i) + "  " + action.Method.Name);
            if (eventBase.GetPersistentMethodName(i).Equals(action.Method.Name))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Removes all listeners from tutorial objects
    /// </summary>
    /// <param name="eventAssets">The tutorials to change</param>
    public static void RemoveListenerFromScriptableObjects(Tutorial[] eventAssets)
    {
        foreach (var asset in eventAssets)
        {
            if (asset == null) continue;
            
            RemoveListener(asset.PageInitiated);
            RemoveListener(asset.Completed);
            RemoveListener(asset.Quit);
            RemoveListener(asset.GoingBack);

            // Mark the asset as dirty so changes are saved
            EditorUtility.SetDirty(asset);
            Debug.Log($"Removed listener from {asset.name}");
        }

        // Save changes
        AssetDatabase.SaveAssets();
    }
    
    /// <summary>
    /// Removes all listeners from a UnityEvent
    /// </summary>
    /// <param name="unityEvent">UnityEventBase to remove listeners from</param>
    private static void RemoveListener(UnityEventBase unityEvent)
    {
        for (int i = unityEvent.GetPersistentEventCount() - 1; i >= 0; i--)
        {
            UnityEventTools.RemovePersistentListener(unityEvent, i);
        }

    }
}