#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Destroys the gameobject when an object in the layermask jumps on it.
/// </summary>
public class JumpStrikeEliminator : MonoBehaviour
{
	[SerializeField] private int damage = 10;
	[SerializeField] private LayerMask collisionLayerMask;
	[SerializeField, Range(0, 180)] private float angle;

	/// <summary>
	/// Checks if collided object is inside a specified FOV and layer mask; destroys the GameObject it is attached to if succeeds with checks
	/// </summary>
	/// <param name="other"></param>
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!LayerMaskUtility.IsInLayerMask(other.gameObject, collisionLayerMask)) return;

		if (!FOVUtility.IsWithinFOV(transform.position, other.transform.position, Vector2.up, angle)) return;

		if (TryGetComponent(out Health health))
		{
			if (TryGetComponent(out Knockback knockback))
				knockback.ApplyForce(transform.right, (float)damage / health.MaxHealth);

			if (other.transform.TryGetComponent(out Knockback playerKnockback))
				playerKnockback.ApplyForce(-transform.right, (float)damage / health.MaxHealth);

			health.RemoveHealth(damage);
			return;
		}

		Destroy(this.gameObject);
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Handles.color = new Color(1, 1, 1, .1f);
		FOVUtility.DrawFOV(transform.position, Vector2.up, 1f, angle);
	}
#endif
}
