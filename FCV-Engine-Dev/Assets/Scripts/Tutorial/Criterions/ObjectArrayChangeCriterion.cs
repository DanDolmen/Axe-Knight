using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
namespace Unity.Tutorials.Core.Editor
{
    public class ObjectArrayChangeCriterion : Criterion
    {
        [SerializeField] private Operation operation;

        [SerializeField] private ObjectReference m_Target;

        [SerializeField] private string propertyPath;

        private int startCount;


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
                if (m_Target == null)
                {
                    Debug.LogWarning("target is not provided");
                    return;
                }

                SerializedObject serializedObject = new SerializedObject(m_Target.SceneObjectReference.ReferencedObject);
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
                if (m_Target == null)
                {
                    Debug.LogWarning("Component is not on object provided");
                    return false;
                }

                SerializedObject serializedObject = new SerializedObject(m_Target.SceneObjectReference.ReferencedObject);

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