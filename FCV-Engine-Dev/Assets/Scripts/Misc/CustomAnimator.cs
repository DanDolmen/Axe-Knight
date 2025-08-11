using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom animation component to base an animation on a percentile
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> animationSprites;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    /// <summary>
    /// Changes the sprite of SpriteRenderer depending on a percentile
    /// </summary>
    /// <param name="percentile">a value from 0 to 1</param>
    public void PercentileAnimation(float percentile)
    {
        int animationIndex = Mathf.FloorToInt(animationSprites.Count * percentile);
        animationIndex = Mathf.Clamp(animationIndex, 0, animationSprites.Count - 1);

        spriteRenderer.sprite = animationSprites[animationIndex];
    }
}
