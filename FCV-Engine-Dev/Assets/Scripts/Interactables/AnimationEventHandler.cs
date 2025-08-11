using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour
{
    public UnityEvent onAnimationEvent;
    
    /// <summary>
    /// Functioned used to couple inspector UnityEvents and AnimationKeyFrameEvents
    /// </summary>
    public void FireAnimationEvent()
    {
        onAnimationEvent.Invoke();
    }
}
