using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace Unity.Tutorials.Core.Editor
{
    public class InstantiateSpriteCriterion : Criterion
    {
        [SerializeField] private Sprite sprite;

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
            foreach (var spriteRenderer in FindObjectsOfType<SpriteRenderer>())
            {
                if (spriteRenderer.sprite == sprite)
                {
                    return true;
                }
            }

            return false;
        }


        public override bool AutoComplete()
        {
            Instantiate(sprite, Vector3.zero, Quaternion.identity);

            return true;
        }
    }
}
#endif