using System;
using UnityEngine;

/// <summary>
/// A checkpoint that will constantly check distance to player and if close enough add itself as latest CheckPointReached in CheckPointManager class
/// </summary>
public class CheckPoint : MonoBehaviour
{
	private Transform playerTransform;
	private bool isActivated = false;

	/// <summary>
	/// Gets player transform from static field in Movement class and sets player position to be the latest checkpoint reached.
	/// </summary>
	private void Start()
	{
		playerTransform = Movement.PlayerTransform;

		CheckPointManager.CheckPointReached(playerTransform.position);
	}

	/// <summary>
	/// Checks if the player is close enough to trigger checkpoint reached; activates the checkpoint. sets it's position to latest and destroys itself.
	/// </summary>
	private void Update()
	{
		if (!isActivated && Vector2.Distance(playerTransform.position, transform.position) <= CheckPointManager.CheckPointAcquiredDistance)
			ActivateCheckPoint();
	}

	/// <summary>
	/// Sets the check point's position to latest and plays the activation animation. 
	/// </summary>
	private void ActivateCheckPoint()
	{
		isActivated = true;
		CheckPointManager.CheckPointReached(transform.position);
		gameObject.TryGetComponent(out Animator anim);
		if (anim != null)
			anim.SetBool("isActivated", isActivated);
	}

	/// <summary>
	/// Destroys this script from the gameobject. Called from the end of the activation animation. 
	/// </summary>
	private void DestroySelf()
	{
		Destroy(this);
	}
}
