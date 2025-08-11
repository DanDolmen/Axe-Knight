using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Represents a pressure-sensitive button that activates when an object stands on it for a specified duration. Supports optional bounce-back functionality when the object leaves before full activation. Fires events when fully activated and during intermediate states for animations.
/// </summary>
public class PressureButton : MonoBehaviour
{
    [SerializeField] private float standTime = 1;
    [SerializeField] private LayerMask layerMask;
    // if the buttons is supposed to bounce back if button is not fully pressed when player leaves it
    [SerializeField] private bool bounceBack; 
    [SerializeField] private float pushedYOffset = -.5f;

    public UnityEvent buttonFired;
    // invokes event whenever the state is changed of the button, used for animations
    public UnityEvent<float> percentilePushed; 

    private float time = 0;
    private bool isButtonFired = false;
    private bool standingOnButton;
    private Vector2 startPosition;
    private float WorldYOffset => startPosition.y + pushedYOffset;

    private void Awake()
    {
        startPosition = transform.position;
        percentilePushed.AddListener(AnimateOffset);
    }
    
    /// <summary>
    /// Handles bounce-back behavior, gradually resetting the button's state when not fully activated and no object is standing on it. Only applicable if bounceBack is enabled.
    /// </summary>
    private void Update()
    {
        if (standingOnButton || isButtonFired) return;
        if (!bounceBack) return;

        if (time <= 0) return;
        // bounces back to original state if player is not standing anymore and buttonfired has not been activated
        time -= Time.deltaTime;
        time = Mathf.Clamp(time, 0, standTime);
        percentilePushed.Invoke(Mathf.Clamp01(time/standTime));
    }

    /// <summary>
    /// Detects when an object leaves the button's trigger area. Updates the standing state and stops activation if conditions are met.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!LayerMaskUtility.IsInLayerMask(other.gameObject, layerMask) || isButtonFired) return;

        standingOnButton = false;
    }

    /// <summary>
    /// Detects when an object enters the button's trigger area. Sets the standing state and prepares for activation.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!LayerMaskUtility.IsInLayerMask(other.gameObject, layerMask) || isButtonFired) return;

        standingOnButton = true;
    }
    
    /// <summary>
    /// Handles the activation process while an object stays on the button. Gradually increases the activation state and fires the buttonFired event when fully activated.
    /// </summary>
    /// <param name="other">The collider that stays within the trigger.</param>
    private void OnTriggerStay2D(Collider2D other)
    { 
        if (!LayerMaskUtility.IsInLayerMask(other.gameObject, layerMask) || isButtonFired) return;

        time += Time.deltaTime;
        
        percentilePushed.Invoke(Mathf.Clamp01(time/standTime));
        
        if (time < standTime) return;

        isButtonFired = true;
        buttonFired.Invoke();
    }
    
    /// <summary>
    /// Handles bounce-back behavior, gradually resetting the button's state when not fully activated and no object is standing on it. Only applicable if bounceBack is enabled.
    /// </summary>
    private void AnimateOffset(float percentile)
    {
        float startY = startPosition.y;

        float currentY = Mathf.Lerp(startY, WorldYOffset, percentile);
        Vector2 currentPosition = transform.position;
        currentPosition.y = currentY;
        transform.position = currentPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(new Vector3(0, pushedYOffset, 0), .1f);
    }
}
