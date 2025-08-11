using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A utility class for working with LayerMasks in Unity.
/// Includes functions to check if a GameObject or a layer is part of a specified LayerMask.
/// </summary>
public static class LayerMaskUtility
{
    /// <summary>
    /// Checks if a GameObject is in the specified LayerMask.
    /// </summary>
    /// <param name="obj">The GameObject to check.</param>
    /// <param name="mask">The LayerMask to check against.</param>
    /// <returns>True if the GameObject is in the specified LayerMask, otherwise false.</returns>
    public static bool IsInLayerMask(GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
    
    /// <summary>
    /// Checks if a layer is in the specified LayerMask.
    /// </summary>
    /// <param name="layer">The layer index to check.</param>
    /// <param name="mask">The LayerMask to check against.</param>
    /// <returns>True if the layer is in the specified LayerMask, otherwise false.</returns>
    public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
}
