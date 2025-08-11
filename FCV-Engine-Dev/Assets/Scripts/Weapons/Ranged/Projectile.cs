using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Used by ranged weapon to make an object travel in direction and then invoke hit event once it collides with an object
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
	[SerializeField] private float speed = 10;

	[SerializeField] private float lifetime = 5;

	[SerializeField] private LayerMask hittableLayer;

	public UnityEvent<Health, Vector2> hitAction;
	private void Awake()
	{
		GetComponent<Rigidbody2D>().isKinematic = true;
		GetComponent<Collider2D>().isTrigger = true;
	}
	
	/// <summary>
	/// Controls lifetime and velocity of projectile
	/// </summary>
	private void Update()
	{
		lifetime -= Time.deltaTime;

		if (lifetime < 0)
		{
			Destroy(gameObject);
		}
		
		transform.position += transform.right * (speed * Time.deltaTime);
	}
	
	/// <summary>
	/// Checks if Collider2D is in LayerMask and tries to get health component; invokes hitAction if checks succeeds and destroys projectiles Game Object
	/// </summary>
	/// <param name="other">Collider2D</param>
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!LayerMaskUtility.IsInLayerMask(other.gameObject, hittableLayer)) return;
		if (other.gameObject.TryGetComponent(out Health health))
		{
			hitAction.Invoke(health, other.transform.position - transform.position);
		}
		if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(SoundType.BulletImpact);
		Destroy(gameObject);
	}
}
