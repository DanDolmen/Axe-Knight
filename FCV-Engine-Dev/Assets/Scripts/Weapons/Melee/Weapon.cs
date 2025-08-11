using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Weapon base class to hold all information and basic functions for a weapon mechanic
/// </summary>
[RequireComponent(typeof(FloatAnimation))]
public class Weapon : MonoBehaviour
{
	[SerializeField] private KeyCode fireButton;
	[SerializeField] protected bool useLeftClick = false;
	[SerializeField] protected int damage;
	[SerializeField] protected float cooldown;

	[SerializeField] protected UnityEvent fireAction;

	private float currentCooldown = 0;

	protected bool Equipped = false;
	public bool IsEquipped => Equipped;
	public bool autoFire;
	
	/// <summary>
	/// Equips weapon and subcribes to fireAction
	/// </summary>
	public virtual void Equip()
	{
		Equipped = true;
		fireAction.AddListener(Fire);
	}
	/// <summary>
	/// Unequips and un-subscribes to fireAction
	/// </summary>
	public virtual void UnEquip()
	{
		Equipped = false;
		fireAction.RemoveListener(Fire);
	}
	
	/// <summary>
	/// Updates cooldown and controls input for invoking fireAction
	/// </summary>
	protected virtual void Update()
	{
		if (!Equipped) return;
		
		currentCooldown -= currentCooldown >= 0 ? Time.deltaTime : 0;
		
		bool leftClick;
		
		if(autoFire)
		{
			leftClick = useLeftClick && Input.GetMouseButton(0);
			if ((Input.GetKey(fireButton) || leftClick) && CanFire()) fireAction.Invoke();
		}
		else
		{
			
			leftClick = useLeftClick && Input.GetMouseButtonDown(0);
			if ((Input.GetKeyDown(fireButton) || leftClick) && CanFire()) fireAction.Invoke();
		}
		
	}
	
	/// <summary>
	/// Base function for Fire, should be overriden to control what happens on fire
	/// </summary>
	protected virtual void Fire()
	{
		currentCooldown = cooldown;
	}

	/// <summary>
	/// If weapon hits: this function removes health from the Health component provided
	/// </summary>
	/// <param name="health">Health component to deal damage to</param>
	/// <param name="direction">direction hit came from, put to (0,0) if no knockback is desired</param>
	protected virtual void DealDamage(Health health, Vector2 direction)
	{
		if (!health) return;

		if (health.TryGetComponent(out Knockback knockback))
		{
			knockback.ApplyForce(direction, (float)damage / health.MaxHealth);
		}
		
		health.RemoveHealth(damage);
	}
	
	/// <summary>
	/// function for if the cooldown has elapsed
	/// </summary>
	/// <returns>true if cooldown has elapsed</returns>
	protected virtual bool CanFire()
	{
		return currentCooldown <= 0f;
	}
}
