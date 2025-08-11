using UnityEngine;
using UnityEngine.SceneManagement;

public static class CheckPointManager
{
	public const float CheckPointAcquiredDistance = 1;

	private static Vector2 lastCheckPoint = Vector2.negativeInfinity;

	/// <summary>
	/// places playerTransform at the latest checkpoint stored
	/// </summary>
	/// <param name="playerTransform">transform to place at latest checkpoint</param>
	public static void PlaceAtCheckPoint(Transform playerTransform)
	{
		if (lastCheckPoint == Vector2.negativeInfinity)
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		playerTransform.position = lastCheckPoint;
		ResetPlayerVelocity(playerTransform);
	}

	/// <summary>
	/// Stores the latest checkpoint position reached
	/// </summary>
	/// <param name="position">checkpoint position</param>
	public static void CheckPointReached(Vector2 position)
	{
		lastCheckPoint = position;
	}

	/// <summary>
	/// Resets the players momentum to avoid an infinite build up of velocity.
	/// </summary>
	/// <param name="playerTransform">transform to place at latest checkpoint</param>
	public static void ResetPlayerVelocity(Transform playerTransform)
	{
		if (playerTransform.gameObject.TryGetComponent(out Rigidbody2D rb))
		{
			rb.velocity = Vector3.zero;
		}
		else Debug.Log("Could not find the players Rigidbody!");
	}
}