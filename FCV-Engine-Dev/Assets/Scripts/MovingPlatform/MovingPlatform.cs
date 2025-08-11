using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// A component to make an object move between determined positions with different options for movement types and speeds
/// </summary>
[SelectionBase]
public class MovingPlatform : MonoBehaviour
{
	public enum LoopType
	{
		LOOP,
		PINGPONG
	}

	[SerializeField] private LayerMask collisionLayerMask;
	[SerializeField] private LoopType loopType;
	private bool pingPongReversing = false;


	[HideInInspector, SerializeField] private Vector3[] worldPoints;
	int previousWorldPointsLength;

	public Vector3[] WorldPoints => worldPoints;

	public float speed = 5f; // Speed in units per second

	private int currentPointIndex = 0; // Index of the current target point
	private Vector3 currentTarget; // Current target point
	private Vector3 previousPosition;
	private float journeyLength; // Distance to the next point
	private float timeSinceStart; // Time when the movement started

	private List<float> totalTravelLength;
	private List<Transform> objectsOnPlatform = new();

	private void Awake()
	{
#if UNITY_EDITOR
		if (worldPoints.Length == 0 || worldPoints == null)
		{
			Debug.LogError($"No points defined for movement on {name}");
			EditorApplication.ExitPlaymode();
			return;
		}
#endif

		currentTarget = worldPoints[0];
	}

	private void Start()
	{
		// Initialize the first target
		SetNextTarget();
	}


	/// <summary>
	/// Moves platform in a constant speed no matter the distance between current points
	/// </summary>
	private void Update()
	{
		if (worldPoints.Length == 0)
			return;

		// Calculate the distance covered based on time
		timeSinceStart += Time.deltaTime;

		float finalSpeed = journeyLength / speed;

		float T = timeSinceStart / finalSpeed;

		T = Mathf.Clamp01(T);

		// Move the object to the current target
		transform.position = Vector3.Lerp(previousPosition, currentTarget, T);

		// Check if the object has reached the target
		if (T > 0.99f)
		{
			// Set the next target
			SetNextTarget();
		}
	}

	/// <summary>
	/// Sets the next point for the moving platform to move towards and sets the length of the journey it is suppose to travel
	/// </summary>
	private void SetNextTarget()
	{
		currentPointIndex = GetNextIndex(currentPointIndex, worldPoints.Length, loopType); // Loop through the points
		previousPosition = currentTarget;
		currentTarget = worldPoints[currentPointIndex];
		timeSinceStart = 0;
		journeyLength = Vector3.Distance(previousPosition, currentTarget);
	}

	/// <summary>
	/// Gets the next index on moving platform based on the movement type
	/// </summary>
	/// <param name="currentIndex">current index platform is going to</param>
	/// <param name="arrayLength">The number of points in pointList</param>
	/// <param name="movementType">which type of movement is it (Loop, PingPong)</param>
	/// <returns>the next index platform should move to</returns>
	private int GetNextIndex(int currentIndex, int arrayLength, LoopType movementType)
	{
		switch (movementType)
		{
			// Loop behavior: Move to the next index, wrapping around to 0 if needed
			case LoopType.LOOP:
				return (currentIndex + 1) % arrayLength;

			// Ping-Pong-behavior: Move to the next index, reversing direction at the ends
			case LoopType.PINGPONG when !pingPongReversing:
				{
					if (currentIndex + 1 < arrayLength) return currentIndex + 1;
					// Reverse direction at the end
					pingPongReversing = true;
					return currentIndex - 1;
				}

			// Ping-Pong-Behaviour: Control reversing in ping-pong behaviour
			case LoopType.PINGPONG when currentIndex - 1 < 0:
				// Reverse direction at the start
				pingPongReversing = false;
				return currentIndex + 1;

			case LoopType.PINGPONG:
				return currentIndex - 1;

			default:
				return currentIndex; // Default case (should not be reached)
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!LayerMaskUtility.IsInLayerMask(other.gameObject, collisionLayerMask)) return;

		if (objectsOnPlatform.Contains(other.transform)) return;

		if (other.transform.position.y - 0.15f < transform.position.y + 0.1f) return;

		objectsOnPlatform.Add(other.transform);

		other.transform.parent = transform;
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		if (!objectsOnPlatform.Contains(other.transform)) return;

		objectsOnPlatform.Remove(other.transform);

		other.transform.parent = null;
	}
}

