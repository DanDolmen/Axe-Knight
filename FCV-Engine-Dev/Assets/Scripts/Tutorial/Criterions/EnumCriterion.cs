using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
namespace Unity.Tutorials.Core.Editor
{
    public class EnumCriterion : Criterion
    {
        [SerializeField] private FutureObjectReference futureObjectReference;
        [SerializeField] private string propertyPath;
        [SerializeField] private int enumIntTargetValue;

        private SerializedObject serializedObject;

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
            serializedObject = new SerializedObject(futureObjectReference.SceneObjectReference.ReferencedObject);
            SerializedProperty enumProperty = serializedObject.FindProperty(propertyPath);
            if (enumProperty == null)
            {
                Debug.LogWarning("Property Path returns null");
                return true;
            }

            return enumProperty.enumValueFlag == enumIntTargetValue;
        }

        public override bool AutoComplete()
        {
            return true;
        }
    }
}
#endif