using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
/// <summary>
/// A custom editor for the Moving Platform class to give access to the player in-editor to customize points and visualize how moving platform will look along points
/// </summary>
[CustomEditor(typeof(MovingPlatform)), CanEditMultipleObjects]
public sealed class MovingPlatformEditor : Editor
{
	private GameObject previewObject;
	private float previewPositions;

	private MovingPlatform movingPlatform;

	private SerializedProperty worldPointsSerializedProperty;
	private SerializedProperty loopTypeSerializedProperty;
	private ReorderableList worldPointsReordableList;

	private Vector3[] worldPoints;
	private MovingPlatform.LoopType LoopType => (MovingPlatform.LoopType)System.Enum.ToObject(typeof(MovingPlatform.LoopType), loopTypeSerializedProperty.enumValueIndex);

	private void OnEnable()
	{
		movingPlatform = target as MovingPlatform;
		if (movingPlatform != null) worldPoints = movingPlatform.WorldPoints;

		if (!EditorApplication.isPlayingOrWillChangePlaymode)
		{
			Selection.selectionChanged += SelectionChanged;
			EditorApplication.playModeStateChanged += PlayModeStateChanged;
		}

		worldPointsSerializedProperty = serializedObject.FindProperty("worldPoints");
		loopTypeSerializedProperty = serializedObject.FindProperty("loopType");


		worldPointsReordableList = new ReorderableList(serializedObject,
								   serializedObject.FindProperty("worldPoints"),
								   true, true, true, true);

		worldPointsReordableList.drawHeaderCallback = (Rect rect) =>
		{
			EditorGUI.LabelField(rect, "Positions");
		};

		worldPointsReordableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
			SerializedProperty element = worldPointsReordableList.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
				element, GUIContent.none);
		};

		worldPointsReordableList.onAddCallback = (ReorderableList l) =>
		{
			// Add current transform position when + is clicked
			var index = l.serializedProperty.arraySize;
			l.serializedProperty.arraySize++;
			l.index = index;
			var element = l.serializedProperty.GetArrayElementAtIndex(index);
			element.vector3Value = movingPlatform.transform.position;
		};
	}

	private void OnDisable()
	{
		DestroyPreviewObject();
	}

	/// <summary>
	/// Destroys and un-subscribes from events if state is ExitingEditMode
	/// </summary>
	/// <param name="state">PlayModeStateChange Enum from EditorApplication.playModeStateChanged</param>
	private void PlayModeStateChanged(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.ExitingEditMode)
		{
			DestroyPreviewObject();
			Selection.selectionChanged -= SelectionChanged;
			EditorApplication.playModeStateChanged -= PlayModeStateChanged;
		}
	}

	/// <summary>
	/// Destroys the object used to preview path
	/// </summary>
	private void DestroyPreviewObject()
	{
		if (!previewObject) return;

		for (int i = previewObject.transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(previewObject.transform.GetChild(0).gameObject);
		}

		DestroyImmediate(previewObject);
	}

	/// <summary>
	/// If the target MovingPlatform is not this it will try to destroy preview object
	/// </summary>
	private void SelectionChanged()
	{
		if (movingPlatform == null) return;
		if (Selection.activeTransform != movingPlatform.transform)
			DestroyPreviewObject();
	}

	private void OnSceneGUI()
	{
		MakePositionAndLineHandles();
	}

	/// <summary>
	/// Makes position handles to control the points where the moving platform travels through and makes lines between them
	/// </summary>
	private void MakePositionAndLineHandles()
	{
		worldPoints = movingPlatform.WorldPoints;
		if (worldPoints.Length <= 0)
			return;

		Vector3 previousPoint = Vector3.positiveInfinity;

		for (int i = 0; i < worldPoints.Length; i++)
		{
			// produces a position handle to easily assign positions for moving platform

			EditorGUI.BeginChangeCheck();
			Vector3 newPosition = Handles.PositionHandle(worldPoints[i], Quaternion.identity);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(movingPlatform, "Move Position Handle");
				worldPoints[i] = newPosition;
			}

			// Draws dotted lines between the points the platform will move between
			if (previousPoint != Vector3.positiveInfinity)
			{
				Handles.DrawDottedLine(previousPoint, worldPoints[i], 10);
			}

			previousPoint = worldPoints[i];
		}
		// Draws final dotted line if it is a loop
		if (LoopType == MovingPlatform.LoopType.LOOP)
		{
			Handles.DrawDottedLine(previousPoint, worldPoints[0], 10);
		}
	}

	/// <summary>
	/// Creates a copy of the moving platform object but sets all sprite alpha value to 0.2
	/// </summary>
	private void CreatePreviewObject()
	{
		previewObject = Instantiate(movingPlatform.gameObject);
		DestroyImmediate(previewObject.GetComponent<MovingPlatform>());
		SpriteRenderer[] renderers = previewObject.GetComponentsInChildren<SpriteRenderer>();
		foreach (var renderer in renderers)
		{
			Color color = renderer.color;
			color.a = .2f;

			renderer.color = color;
		}

		previewObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
	}

	public override void OnInspectorGUI()
	{
		DrawWorldPositionList();

		base.OnInspectorGUI();



		if (EditorApplication.isPlayingOrWillChangePlaymode) return;

		// Slider for the preview so that the whole path can be displayed
		previewPositions = EditorGUILayout.Slider("Preview Position", previewPositions, 0, 1);

		if (!previewObject)
			CreatePreviewObject();

		List<Vector3> points = worldPoints == null ? new() : new(worldPoints);

		if (points == null || points.Count == 0) return;

		if (LoopType == MovingPlatform.LoopType.LOOP) points.Add(points[0]);

		Vector3 lerpedPosition = PointLerp.LerpPoints(points.ToArray(), previewPositions);
		previewObject.transform.position = lerpedPosition;
	}

	private void DrawWorldPositionList()
	{
		serializedObject.Update();
		worldPointsReordableList.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}

}