#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Unity.Tutorials.Core.Editor;
using System.Collections.Generic;
using System.Reflection;

[CreateAssetMenu(
    fileName = "ButtonHasCorrectOnClickCriterion",
    menuName = "Tutorials/Criterion/Button Has Correct OnClick")]
public class ButtonHasCorrectOnClickCriterion : Criterion
{
    [Header("Scene References")]
    [SerializeField] private FutureObjectReference m_TargetButton;
    [SerializeField] private FutureObjectReference m_ExpectedTarget;

    [Header("Expected Method")]
    [SerializeField] private string m_ExpectedMethodName;

    private Button TargetButton
    {
        get
        {
            var obj = m_TargetButton?.SceneObjectReference?.ReferencedObject;
            return obj is GameObject go ? go.GetComponent<Button>() : obj as Button;
        }
    }

    private GameObject ExpectedTarget
    {
        get
        {
            var obj = m_ExpectedTarget?.SceneObjectReference?.ReferencedObject;
            return obj is GameObject go ? go : (obj as Component)?.gameObject;
        }
    }

    protected override bool EvaluateCompletion()
    {
        Debug.Log("=== EvaluateCompletion BEGIN ===");

        Debug.Log($"TargetButton resolved: {(TargetButton != null ? TargetButton.name : "null")}");
        Debug.Log($"ExpectedTarget resolved: {(ExpectedTarget != null ? ExpectedTarget.name : "null")}");
        Debug.Log($"Expected Method Name: {(string.IsNullOrEmpty(m_ExpectedMethodName) ? "null or empty" : m_ExpectedMethodName)}");

        if (TargetButton == null || ExpectedTarget == null || string.IsNullOrEmpty(m_ExpectedMethodName))
        {
            Debug.LogWarning("❌ Criterion failed: Missing reference.");
            return false;
        }

        var onClick = TargetButton.onClick;
        int count = onClick.GetPersistentEventCount();

        for (int i = 0; i < count; i++)
        {
            var target = onClick.GetPersistentTarget(i);
            var method = onClick.GetPersistentMethodName(i);

            Debug.Log($"→ Index {i}: Target = {(target != null ? target.name : "null")}, Method = {method}");

            if (target == ExpectedTarget && method == m_ExpectedMethodName)
            {
                Debug.Log("✅ Match found! Criterion passed.");
                return true;
            }
        }

        Debug.Log("❌ No match found. Criterion still incomplete.");
        return false;
    }

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

    public override bool AutoComplete() => true;

    protected override IEnumerable<FutureObjectReference> GetFutureObjectReferences()
    {
        return new[] { m_TargetButton, m_ExpectedTarget };
    }

#if UNITY_EDITOR
    private void CreateFutureReferencesIfMissing()
    {
        if (m_TargetButton == null)
        {
            m_TargetButton = CreateFutureObjectReference("Target Button");
            AssetDatabase.AddObjectToAsset(m_TargetButton, this);
        }

        if (m_ExpectedTarget == null)
        {
            m_ExpectedTarget = CreateFutureObjectReference("Expected Target");
            AssetDatabase.AddObjectToAsset(m_ExpectedTarget, this);
        }

        EditorUtility.SetDirty(this);
    }

    private FutureObjectReference CreateFutureObjectReference(string name)
    {
        var futureRef = ScriptableObject.CreateInstance<FutureObjectReference>();
        futureRef.name = name;

        var holder = ScriptableObject.CreateInstance<SceneObjectReferenceHolder>();
        holder.hideFlags = HideFlags.HideAndDontSave;

        typeof(FutureObjectReference)
            .GetField("m_ReferenceHolder", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(futureRef, holder);

        futureRef.SceneObjectReference = new SceneObjectReference();

        return futureRef;
    }
#endif
}

[CustomEditor(typeof(ButtonHasCorrectOnClickCriterion))]
public class ButtonHasCorrectOnClickCriterionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("⚙ Create Missing FutureObjectReferences"))
        {
            var criterion = (ButtonHasCorrectOnClickCriterion)target;
            var method = typeof(ButtonHasCorrectOnClickCriterion).GetMethod("CreateFutureReferencesIfMissing", BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(criterion, null);

            AssetDatabase.SaveAssets();
            Debug.Log("✅ Created missing FutureObjectReferences and saved assets.");
        }
    }
}
#endif
