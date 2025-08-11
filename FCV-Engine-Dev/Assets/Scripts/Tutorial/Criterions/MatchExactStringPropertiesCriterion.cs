using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace Unity.Tutorials.Core.Editor
{
    public class MatchExactStringPropertiesCriterion : Criterion
    {
        [SerializeField] private FutureObjectReference objectReference1;
        [SerializeField] private string propertyPath1 = "";

        [SerializeField] private FutureObjectReference objectReference2;
        [SerializeField] private string propertyPath2 = "";

        private Object TargetObject1 => objectReference1.SceneObjectReference.ReferencedObject;
        private Object TargetObject2 => objectReference2.SceneObjectReference.ReferencedObject;



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
            if (!TargetObject1 || !TargetObject2)
            {
                Debug.LogWarning("either or both of objectreferences are null");
                return false;
            }

            SerializedProperty string1Property = new SerializedObject(TargetObject1).FindProperty(propertyPath1);
            SerializedProperty string2Property = new SerializedObject(TargetObject2).FindProperty(propertyPath2);

            if (string1Property == null || string2Property == null)
            {
                Debug.LogWarning("either or both of strings are null");
                return false;
            }

            return string1Property.stringValue.Equals(string2Property.stringValue);
        }

        public override bool AutoComplete()
        {
            return true;
        }
    }
}
#endif