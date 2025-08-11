using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utility;
using Object = UnityEngine.Object;

namespace Unity.Tutorials.Core.Editor
{
    /// <summary>
    /// Criterion to check if string property or element in array of type string matches specified item in ItemDatabase 
    /// </summary>
    public class MatchItemNameCriterion : Criterion
    {
        [SerializeField] private FutureObjectReference futureObjectReference;
        [SerializeField] private string referenceStringPropertyPath;
        [SerializeField] private MatchType matchType = default;
        [SerializeField] private int index;
        
        private ItemDatabase itemDatabase;
        
        private string ItemName {
            get
            {
                switch (matchType)
                {
                    case MatchType.First:
                        return itemDatabase.GetAllItemNames()[0];
                    case MatchType.Last:
                        return itemDatabase.GetAllItemNames()[^1];
                    case MatchType.Index:
                        return itemDatabase.GetAllItemNames()[index];
                    default:
                        return itemDatabase.GetAllItemNames()[0];
                }
            }
        }
        
        private Object ReferenceObject => futureObjectReference.SceneObjectReference.ReferencedObject;

        public override void StartTesting()
        {
            base.StartTesting();

            itemDatabase = ScriptableObjectFinder.FindAssetsOfType<ItemDatabase>()[0];
            
            EditorApplication.update += UpdateCompletion;
        }

        public override void StopTesting()
        {
            base.StopTesting();
            
            EditorApplication.update -= UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            if (itemDatabase == null)
            {
                Debug.LogWarning("Item database is null.");
                return true;
            } 

            if (referenceStringPropertyPath.IsNullOrEmpty())
            {
                Debug.LogWarning("ReferenceString property path is null or empty.");
                return true;
            }

            if (futureObjectReference == null)
            {
                Debug.LogWarning("Future object reference is null.");
                return true;
            }
            
            SerializedProperty stringProperty = new SerializedObject(ReferenceObject).FindProperty(referenceStringPropertyPath);

            if (stringProperty == null)
            {
                Debug.LogWarning("ReferenceString property is null.");
                return false;
            }
            
            if (stringProperty is { propertyType: SerializedPropertyType.String })
            {
                return stringProperty.stringValue == ItemName;
            }

            if (stringProperty.isArray)
            {
                SerializedProperty element = stringProperty.GetArrayElementAtIndex(0);

                if (element.propertyType != SerializedPropertyType.String)
                {
                    Debug.LogWarning("ReferenceString property type is not string.");
                    return false;
                }
                
                return element.stringValue == ItemName;
            }

            return false;
        }


        public override bool AutoComplete()
        {
            return false;
        }
    }
    internal enum MatchType
    {
        First,
        Last,
        Index
    }
}