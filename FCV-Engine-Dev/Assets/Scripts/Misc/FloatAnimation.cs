using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that can be added to any object to give it a floating animation in game
/// </summary>
public class FloatAnimation : MonoBehaviour
{
    [SerializeField] private Vector2 floatEndPoint = new (0, -.5f);
    [SerializeField] private float animationTime = .5f;
    [SerializeField] private AnimationCurve easingCurve;

    private Vector2 startPoint;

    private bool isFloating = true;

    private void Awake()
    {
        startPoint = transform.position;
    }
    
    /// <summary>
    /// Starts coroutine for the float animation logic
    /// </summary>
    private void Start()
    {
        StartCoroutine(FloatAnimationLogic());
    }
    
    /// <summary>
    /// Makes the item float up and down using a easing curve and coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator FloatAnimationLogic()
    {
        float t = 0;
        while (isFloating)
        {
            t += Time.deltaTime;
            t %= animationTime;
            float alpha = Mathf.PingPong(t / animationTime * 2, 1f);
            alpha = easingCurve.keys.Length > 1 ? easingCurve.Evaluate(alpha) : alpha;
            
            transform.position = Vector2.Lerp(startPoint, startPoint + floatEndPoint, alpha);
            yield return new WaitForEndOfFrame();
        }
    }
    
    /// <summary>
    /// Stops the floating animation
    /// </summary>
    public void StopAnimation()
    {
        isFloating = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + floatEndPoint);
    }
}
