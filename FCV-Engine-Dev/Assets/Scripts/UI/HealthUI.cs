using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Connects Health with UI, made it generalised so it can be used in multiple places and be connected with multiple amounts of UI Elements
/// </summary>
public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private UnityEvent<float> healthUpdate;

    private void Awake()
    {
        if (!health) return;
        
        health.onHealthChanged.AddListener(OnHealthChanged);
    }
    
    /// <summary>
    /// Removes listeners from onHealthChanged 
    /// </summary>
    private void OnDisable()
    {
        health.onHealthChanged.RemoveListener(OnHealthChanged);
    }
    
    /// <summary>
    /// Invokes the healthUpdate UnityEvent and sending percentile health is of maxHealth
    /// </summary>
    /// <param name="currentHealth">the current health the player or object has</param>
    /// <param name="maxHealth">the max health of the player or object has</param>
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        healthUpdate.Invoke(currentHealth / (float)maxHealth);
    }
}
