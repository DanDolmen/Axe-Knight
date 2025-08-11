using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

#if UNITY_EDITOR
namespace Unity.Tutorials.Core.Editor
{
    public class TextureImporterIntPropertyCriterion : Criterion
    {
        [SerializeField] private Object referencedObject;
        [SerializeField] private string propertyPath;
        [FormerlySerializedAs("enumIntTargetValue")] [SerializeField] private int intTargetValue;

        private TextureImporter textureImporter;

        public override void StartTesting()
        {
            base.StartTesting();
            string path = AssetDatabase.GetAssetPath(referencedObject);
            textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            EditorApplication.update += UpdateCompletion;
        }

        public override void StopTesting()
        {
            base.StopTesting();

            EditorApplication.update -= UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            SerializedObject serializedObject = new SerializedObject(textureImporter);

            if (textureImporter == null)
            {
                Debug.LogError("No TextureImporter found for the selected texture.");
                return true;
            }

            // Find the spriteMode property
            SerializedProperty property = serializedObject.FindProperty(propertyPath);

            if (property == null)
            {
                Debug.LogError("property not found.");
                return true;
            }

            if (property.isArray)
                return property.arraySize > intTargetValue;
            

            int value = property.numericType == SerializedPropertyNumericType.Float ? Mathf.RoundToInt(property.floatValue) : property.intValue;

            
            return value == intTargetValue;
        }

        public override bool AutoComplete()
        {
            return true;
        }
    }
}
#endif