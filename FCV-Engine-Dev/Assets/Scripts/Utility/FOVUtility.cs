#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// A utility class for handling Field of View (FOV) calculations and visualizations in Unity.
/// Includes functions to check if a target is within the FOV and range, and to draw the FOV in the scene view.
/// </summary>
public static class FOVUtility
{
#if UNITY_EDITOR
    
    /// <summary>
    /// This function draws an arc based on specified values. SHOULD ONLY BE USED IN OnDrawGizmos/OnDrawGizmosSelected
    /// </summary>
    /// <param name="origin">Start point of the FOV.</param>
    /// <param name="direction">Start direction of the arc.</param>
    /// <param name="maxDistance">Distance of the arc.</param>
    /// <param name="fovAngle">Maximum angle of the arc in degrees.</param>
    /// <returns>void</returns>
    public static void DrawFOV(Vector3 origin, Vector3 direction, float maxDistance, float fovAngle)
    {
        // Calculate the frustum vertices
        Vector3 right = Quaternion.Euler(0, 0, fovAngle / 2) * direction;
        Vector3 left = Quaternion.Euler(0, 0, -fovAngle / 2) * direction;

        // Draw the frustum edges
        Gizmos.DrawLine(origin, origin + direction * maxDistance); // Forward line
        Gizmos.DrawLine(origin, origin + right * maxDistance);    // Right edge
        Gizmos.DrawLine(origin, origin + left * maxDistance);     // Left edge
        
        Handles.DrawSolidArc(origin, Vector3.back, right.normalized, fovAngle, maxDistance);
    }
#endif
    
    /// <summary>
    /// Checks if the target is within the Field of View and range of the source.
    /// </summary>
    /// <param name="source">The position of the source.</param>
    /// <param name="target">The position of the target.</param>
    /// <param name="direction">The direction from the source to check the FOV angle from.</param>
    /// <param name="maxDistance">The maximum distance for the FOV check.</param>
    /// <param name="fovAngle">The angle of the FOV in degrees.</param>
    /// <returns>True if the target is within both the FOV and range, otherwise false.</returns>
    public static bool IsWithinFOVAndRange(Vector3 source, Vector3 target, Vector3 direction, float maxDistance, float fovAngle)
    {
        // Calculate the vector from source to target
        Vector3 toTarget = target - source;

        // Calculate the distance to the target
        float distanceToTarget = toTarget.magnitude;

        // Check if the target is within the maximum distance
        if (distanceToTarget > maxDistance)
        {
            return false;
        }

        return IsWithinFOV(source, target, direction, fovAngle);
    }
    
    /// <summary>
    /// Checks whether a position is within a Field of View angle from another position.
    /// </summary>
    /// <param name="source">The source position.</param>
    /// <param name="target">The target position.</param>
    /// <param name="direction">The direction from the source to check the angle from.</param>
    /// <param name="fovAngle">The angle in degrees.</param>
    /// <returns>True if the target is within the FOV, otherwise false.</returns>
    public static bool IsWithinFOV(Vector3 source, Vector3 target, Vector3 direction, float fovAngle)
    {
        // Calculate the vector from source to target
        Vector3 toTarget = target - source;
        
        // Normalize the direction and toTarget vectors
        Vector3 normalizedDirection = direction.normalized;
        Vector3 normalizedToTarget = toTarget.normalized;

        // Calculate the angle between the direction and toTarget vectors
        float angle = Vector3.Angle(normalizedDirection, normalizedToTarget);

        // Check if the target is within the FOV angle
        return angle <= fovAngle / 2;
    }
    
    
}
