using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Controls what happens to a player if they fall below this object y position
/// </summary>
public class FallDetectorScript : MonoBehaviour
{
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = Movement.PlayerTransform;
    }

    /// <summary>
    /// Checks if the player transform falls below this objects y position and if so places the player at the latest check point. 
    /// </summary>
    void Update()
    {
        if (!playerTransform) return;
        
        if (playerTransform.transform.position.y > transform.position.y) return;
            
        CheckPointManager.PlaceAtCheckPoint(playerTransform);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawLine(new Vector2(-200, transform.position.y), new Vector3(200, transform.position.y));
    }
#endif

    
}
