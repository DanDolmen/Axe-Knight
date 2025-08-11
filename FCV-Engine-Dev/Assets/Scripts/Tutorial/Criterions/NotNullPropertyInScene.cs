#if UNITY_EDITOR
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.Tutorials.Core.Editor
{
    public class NotNullPropertyInScene : Criterion
    {
        [SerializeField] private ObjectReference m_Target;

        [SerializeField] private string propertyPath = "";

        public override void StartTesting()
        {
            base.StartTesting();
            UpdateCompletion();

            EditorApplication.update += UpdateCompletion;
        }

        public override void StopTesting()
        {
            base.StopTesting();

            EditorApplication.update -= UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            if (m_Target.SceneObjectReference.ReferencedObject == null) return false;

            SerializedObject serializedObject = new SerializedObject(m_Target.SceneObjectReference.ReferencedObject);
            SerializedProperty serializedProperty = serializedObject.FindProperty(propertyPath);
            if (serializedProperty == null) return false;
            
            return serializedProperty.objectReferenceValue ?? false;
        }


        public override bool AutoComplete()
        {
            return true;
        }
    }
}
#endif
