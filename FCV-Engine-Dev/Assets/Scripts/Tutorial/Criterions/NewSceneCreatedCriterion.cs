using UnityEditor;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;

// Very specific Criterion to check wether a new scene has been created or not
namespace Unity.Tutorials.Core.Editor
{
    public class NewSceneCreatedCriterion : Criterion
    {
        private Scene startScene;
    
        public override void StartTesting()
        {
            startScene = EditorSceneManager.GetActiveScene();
            
            base.StartTesting();
            UpdateCompletion();


            EditorApplication.update += UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            return startScene != EditorSceneManager.GetActiveScene();
        }

        public override void StopTesting()
        {
            base.StopTesting();
            EditorApplication.update -= UpdateCompletion;
        }

        public override bool AutoComplete()
        {
            return true;
        }
    }
}
#endif
