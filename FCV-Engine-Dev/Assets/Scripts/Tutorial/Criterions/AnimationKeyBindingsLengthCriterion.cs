using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace Unity.Tutorials.Core.Editor
{
    public class AnimationKeyBindingsLengthCriterion : Criterion
    {
        [SerializeField] private AnimationClip animationClip;
        [SerializeField] private int value;
        [SerializeField] private ArrayChangeCriterion.Operation operation;


        public override void StartTesting()
        {
            base.StartTesting();

            EditorApplication.update += UpdateCompletion;
        }

        public override void StopTesting()
        {
            base.StopTesting();

            EditorApplication.update -= UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            if (animationClip == null)
            {
                Debug.LogError("Referenced object is not of type AnimationClip.");
                return true;
            }

            EditorCurveBinding[] curveBindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);
            int length = curveBindings.Select(curveBinding => AnimationUtility.GetObjectReferenceCurve(animationClip, curveBinding).Length).Prepend(-1).Max();


            if (length == -1) return false;
            switch (operation)
            {
                case ArrayChangeCriterion.Operation.Greater:
                    return length > value;
                case ArrayChangeCriterion.Operation.Less:
                    return length < value;
            }

            return false;
        }

        public override bool AutoComplete()
        {
            return true;
        }
    }
}
#endif