using System.Collections;
using UnityEngine;

/// <summary>
/// A component that can be called to make the sprite of a Gameobject to brefly flicker/flash with a different 
/// material then the default. Can specify an amount of flashes and a total duration for the flashes.
/// </summary>
public class SimpleFlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Material flashMaterial;
    [SerializeField] float flashDurationSec = 0.2f;
    [SerializeField] int flashAmount = 1;

    private Material baseMaterial;
    private Coroutine coroutine;

    private void Start()
    {
        if (spriteRenderer == null)
            if (TryGetComponent(out SpriteRenderer renderer)) spriteRenderer = renderer; ;

        if (spriteRenderer != null)
            baseMaterial = spriteRenderer.material;
    }

    /// <summary>
    /// Checks if the Flash() Coroutine has not already been started. If not, start the Coroutine.
    /// </summary>
    public void PlayFlash()
    {
        if (coroutine == null)
            coroutine = StartCoroutine(Flash());
    }

    /// <summary>
    /// Coroutine that calculates evenly spaced flash intervals, then alternates the sprite's material 
    /// between a flash and the original material for a set number of flashes.
    /// </summary>
    IEnumerator Flash()
    {
        if (spriteRenderer == null) yield break;

        // Calc duration of flash/-es with equal spaces between flash/-es:
        float flashSpeed = flashDurationSec / ((flashAmount * 2) - 1);

        for (int i = 0; i < flashAmount; i++)
        {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(flashSpeed);

            spriteRenderer.material = baseMaterial;
            yield return new WaitForSeconds(flashSpeed);
        }

        // Set this coroutine to null, signaling that it has ended and can be restated again.
        coroutine = null;
    }
}
