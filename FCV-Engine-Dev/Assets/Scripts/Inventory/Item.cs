using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item component to make it accessible with the Inventory system
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    [ItemName, SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;

    public string ItemName => itemName;

    private ItemDatabase itemDatabase;
    
    /// <summary>
    /// Applies specified sprite automatically from sprite variable
    /// </summary>
    private void OnValidate()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sprite = sprite;
    }
    
    /// <summary>
    /// if something collides with it checks if object has inventory; tries to add item to it and disables itself if future reuse is needed 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Adds item to inventory and deactivates the gameobject
        if (!other.TryGetComponent(out Inventory inventory)) return;
        inventory.TryAddItem(itemName, this);
        gameObject.SetActive(false);
    }
}
