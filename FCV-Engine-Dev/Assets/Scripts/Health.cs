using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Component that can be added to any object if it is supposed to be killed or destroyed
/// </summary>
public class Health : MonoBehaviour
{
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent<GameObject> onDeath;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool destroyOnDeath = false;

    public int CurrentHealth { get; private set; }

    public int MaxHealth => maxHealth;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }
    /// <summary>
    /// Removes health by amount
    /// </summary>
    /// <param name="amount">Amount to remove from health</param>
    public void RemoveHealth(int amount)
    {
        if (amount > 0) amount *= -1;
        ModifyHealth(amount);
    }
    
    /// <summary>
    /// Adds to health by amount
    /// </summary>
    /// <param name="amount">Amount to heal</param>
    public void AddHealth(int amount)
    {
        if (amount < 0) amount *= -1;
        ModifyHealth(amount);
    }
    /// <summary>
    /// Modifies health int and invokes related UnityEvents
    /// </summary>
    /// <param name="healthChange"></param>
    private void ModifyHealth(int healthChange)
    {
        CurrentHealth += healthChange;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        if (CurrentHealth > 0)
        {
            onHealthChanged.Invoke(CurrentHealth, maxHealth);
        }
        else
        {
            onDeath.Invoke(this.gameObject);
            
            if (destroyOnDeath)
                Destroy(gameObject);
        }
    }
}
