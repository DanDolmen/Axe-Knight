using System;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

namespace Unity.Tutorials.Core.Editor
{
    public class ArrayChangeCriterion : Criterion
    {
        [SerializeField] private Operation operation;

        [SerializeField] private FutureObjectReference m_Target;

        [SerializeField, SerializedTypeFilter(typeof(Component), false)]
        private SerializedType m_serializedType;

        [SerializeField] private string propertyPath;

        private int startCount;

        private GameObject TargetObject => (GameObject)m_Target.SceneObjectReference.ReferencedObject;

        public override void StartTesting()
        {
            base.StartTesting();
            UpdateCompletion();
            StartValue();

            EditorApplication.update += UpdateCompletion;
        }

        /// <summary>
        /// Sets the start count variable to the original array size of the property specified
        /// </summary>
        private void StartValue()
        {
            try
            {
                if (m_Target.SceneObjectReference.ReferencedObject == null) return;
                Component component = TargetObject.GetComponent(m_serializedType.Type);
                if (!component)
                {
                    Debug.LogWarning("Component is not on object provided");
                    return;
                }

                SerializedObject serializedObject = new SerializedObject(component);
                SerializedProperty serializedProperty = serializedObject.FindProperty(propertyPath);

                if (!serializedProperty.isArray)
                {
                    Debug.LogWarning("The property path provided is not an array type");
                    return;
                }

                startCount = serializedProperty.arraySize;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void StopTesting()
        {
            base.StopTesting();

            EditorApplication.update -= UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            bool completion = false;

            try
            {
                if (m_Target.SceneObjectReference.ReferencedObject == null) return false;

                Component component = TargetObject.GetComponent(m_serializedType.Type);
                if (!component)
                {
                    Debug.LogWarning("Component is not on object provided");
                    return true;
                }

                SerializedObject serializedObject = new SerializedObject(component);

                SerializedProperty serializedProperty = serializedObject.FindProperty(propertyPath);


                if (!serializedProperty.isArray)
                {
                    Debug.LogWarning("The property path provided is not an array type");
                    return true;
                }

                switch (operation)
                {
                    case Operation.Greater:
                        completion = serializedProperty.arraySize > startCount;
                        break;
                    case Operation.Less:
                        completion = serializedProperty.arraySize < startCount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return completion;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return true;
            }
        }

        public override bool AutoComplete()
        {
            return true;
        }

        public enum Operation
        {
            Greater,
            Less
        }
    }
}
#endif