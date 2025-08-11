using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace Unity.Tutorials.Core.Editor
{
    public class LayerInLayerMaskCriterion : Criterion
    {
        [SerializeField] private FutureObjectReference futureObjectReference;

        [SerializeField] private string propertyPath;

        [Layer, SerializeField] private int layerToCheck;

        private Object TargetObject => futureObjectReference.SceneObjectReference.ReferencedObject;

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
            SerializedProperty layerMaskProperty = new SerializedObject(TargetObject).FindProperty(propertyPath);

            if (layerMaskProperty == null)
            {
                Debug.LogWarning("Layer Mask property is null");
                return false;
            }

            LayerMask layerMask = layerMaskProperty.intValue;


            return layerMask == (layerMask | (1 << layerToCheck));
        }


        public override bool AutoComplete()
        {
            return true;
        }
    }


    public class LayerAttribute : PropertyAttribute
    {
        // This class doesn't need any specific code
    }

    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Make sure the property is an integer
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                // Draw a Layer field instead of a regular integer field
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
            }
            else
            {
                // If the property is not an integer, display a warning
                EditorGUI.LabelField(position, label.text, "Use LayerAttribute with int.");
            }
        }
    }
}
#endif