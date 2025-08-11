#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// A component that uses OverlapCircle method in Physics2D to deal damage against objects with Health component and in assigned LayerMask
/// </summary>
public class Damager : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float collisionDistance = 1;
    [SerializeField] private float attackCooldown = .5f;

    [Space(15)] [Header("Direction Options"), Space(5)] 
    
    [SerializeField] private bool flippedBySpeed = false;
    [SerializeField, Range(0, 360)] private float fovAngle = 360;
    [SerializeField, Range(0, 360)] private float startAngle = 90;
    
    private new Collider2D collider2D;
    private Vector2 StartDirection => new(Mathf.Cos(CorrectedStartAngle * Mathf.Deg2Rad), Mathf.Sin(CorrectedStartAngle * Mathf.Deg2Rad));
    private float CorrectedStartAngle => xSignedDirection >= 0 ? startAngle : startAngle + 180;

    private float currentCooldown = 0;

    private bool CanAttack => currentCooldown <= 0;

    private Vector2 lastPosition;


    private int xSignedDirection = 1;

    private void Awake()
    {
        lastPosition = transform.position;
    }
    
    /// <summary>
    /// Checks for last position and in which direction the damager object is moving 
    /// </summary>
    private void LateUpdate()
    {
        xSignedDirection = flippedBySpeed ? (int) Mathf.Sign(((Vector2)transform.position - lastPosition).x) : 1;
        lastPosition = transform.position;
    }
    
    /// <summary>
    /// Controls cooldown functionality and does the OverlapCircle check and then sends the Collider2D to CollisionChecks()
    /// </summary>
    private void FixedUpdate()
    {
        if (!CanAttack)
        {
            currentCooldown -= Time.fixedDeltaTime;
            return;
        }
        // does collider hit check in layermask and if hit tries to deal damage to the colliding object
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, collisionDistance, layerMask);
        
        if (hitCollider)
            CollisionChecks(hitCollider);
    }
    
    /// <summary>
    /// Deals damage to Health component
    /// </summary>
    /// <param name="health">Health component to deal damage against</param>
    private void DealDamage(Health health)
    {
        if (!health)
        {
            print("Health component is null");
            return;
        }
        
        health.RemoveHealth(damage);
        
        SetAttackCooldown(attackCooldown);
    }
    
    /// <summary>
    /// Sets the current cooldown to provided one if time is greater than current cooldown
    /// </summary>
    /// <param name="time">cooldown time</param>
    public void SetAttackCooldown(float time)
    {
        if (time < currentCooldown) return;
        
        currentCooldown = time;
    }
    
    /// <summary>
    /// Checks if all requirments are met when collider is hit with overlap check and deals damage if success
    /// </summary>
    /// <param name="other">collider2D to check against</param>
    private void CollisionChecks(Collider2D other)
    {
        if (!FOVUtility.IsWithinFOV(transform.position, other.transform.position, StartDirection,  fovAngle))
        {
            return;
        }

        if (!other.TryGetComponent(out Health health))
        {
            return;
        }
        
        DealDamage(health);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color color;
        Gizmos.color = color = Color.red;
        color.a = .1f;
        Handles.color = color;
        
        Gizmos.DrawWireSphere(transform.position, collisionDistance);
        
        FOVUtility.DrawFOV(transform.position, StartDirection, collisionDistance, fovAngle);
    }
#endif
}
