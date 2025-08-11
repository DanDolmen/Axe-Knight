using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Static utility class for finding Scriptable Objects in assets
    /// </summary>
    public static class ScriptableObjectFinder
    {
        /// <summary>
        /// Get a list with all scriptable objects of certain type in project
        /// </summary>
        /// <typeparam name="T">ScriptableObject type</typeparam>
        /// <returns>List of all scriptable objects found</returns>
        public static List<T> FindAssetsOfType<T>() where T : ScriptableObject
        {
            List<T> results = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    results.Add(asset);
                }
            }

            return results;
        }
    }
}