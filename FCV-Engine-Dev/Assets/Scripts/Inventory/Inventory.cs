using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that holds all picked up items for player
/// </summary>
public class Inventory : MonoBehaviour
{
    /// <summary>
    /// Dictionary that holds all items with their names as key
    /// </summary>
    private Dictionary<string, Item> InventoryItems { get; set; } = new();

    /// <summary>
    /// Public read-only access to the inventory items
    /// </summary>
    public IReadOnlyDictionary<string, Item> ReadOnlyInventory => InventoryItems;

    private void Awake()
    {
        InventoryItems = new();
    }
    
    /// <summary>
    /// Tries to add item to inventory and if name already exists does nothing
    /// </summary>
    /// <param name="itemName">name of item</param>
    /// <param name="item">Item class reference</param>
    public void TryAddItem(string itemName, Item item)
    {
        InventoryItems.TryAdd(itemName, item);
    }
    
    /// <summary>
    /// Tries to remove item using item name
    /// </summary>
    /// <param name="itemName">name of item</param>
    /// <returns>Returns true if item was removed, false if not</returns>
    public bool TryRemoveItem(string itemName)
    {
        return InventoryItems.Remove(itemName);
    }
    /// <summary>
    /// Checks if inventory has the item name specified
    /// </summary>
    /// <param name="itemName">name of item</param>
    /// <returns>returns true if item exists, false if not</returns>
    public bool HasItem(string itemName)
    {
        return InventoryItems.ContainsKey(itemName);
    }
}
