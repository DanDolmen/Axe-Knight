using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Scriptable object to hold all items assigned in editor to make ItemName property field to work properly
/// Designed to make the string comparison easier
/// </summary>
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<string> itemNames; 

    /// <summary>
    /// Gets all item names in list
    /// </summary>
    /// <returns>list of strings with all names</returns>
    public List<string> GetAllItemNames()
    {
        return itemNames;
    }
    
    /// <summary>
    /// Gets the name of a certain index in list
    /// </summary>
    /// <param name="index">index of item</param>
    /// <returns>the name of the item</returns>
    public string GetName(int index)
    {
        return itemNames[index];
    }
    
    /// <summary>
    /// Updates the name of specified index in database
    /// </summary>
    /// <param name="index">index of item</param>
    /// <param name="itemName">new name of the item</param>
    public void UpdateName(int index, string itemName)
    {
        if (index > itemNames.Count)
        {
            Debug.LogWarning($"{itemName} already exists in database");
            return;
        }

        itemNames[index] = itemName;
    }
    
    /// <summary>
    /// returns true if the itemname is already in database
    /// </summary>
    /// <param name="itemName">name of item</param>
    /// <returns>return true if item exists, false if not</returns>
    public bool HasItem(string itemName)
    {
        return itemNames.Contains(itemName);
    }
    
    /// <summary>
    /// Tries to add item to database with itemName, gives error if item name already exists in database
    /// </summary>
    /// <param name="itemName"></param>
    public void AddItem(string itemName)
    {
        if (itemNames.Contains(itemName))
        {
            Debug.LogWarning($"{itemName} already exists in database");
            return;
        }
            
        itemNames.Add(itemName);
    }
    /// <summary>
    /// Removes all items from database
    /// </summary>
    public void ClearItems()
    {
        itemNames.Clear();
    }
}