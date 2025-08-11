using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to add force to attached objects rigidbody2D for easier access
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Knockback : MonoBehaviour
{
    [SerializeField] private float knockbackFactor = 2f;
    
    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    
    /// <summary>
    /// Applies a force in a direction with a force depending on a percentile
    /// </summary>
    /// <param name="direction">direction to apply force</param>
    /// <param name="amount">a value between 0-1 to applie towards max force</param>
    public void ApplyForce(Vector2 direction, float amount)
    {
        amount = Mathf.Clamp01(amount);
        rb2D.AddForce((direction.normalized + Vector2.up).normalized * (knockbackFactor * amount), ForceMode2D.Impulse);
    }
}
