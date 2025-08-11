using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;   // The UI panel to show/hide
    [SerializeField] private Transform slotParent;        // The GridLayoutGroup holder
    [SerializeField] private GameObject slotPrefab;       // InventorySlotPrefab with image that get replaced with sprite

    [SerializeField] private SerializableDictionary<Item, Sprite> items = new(); //Inventory Dictionary, Holds the current items with their associated sprites

    private Inventory playerInventory;

    void Start()
    {
        playerInventory = FindObjectOfType<Inventory>();
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel.activeSelf)
                CloseInventory();
            else
                OpenInventory();
        }
    }

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        items.Clear(); // Clears dictionary

        var inventoryItems = playerInventory.ReadOnlyInventory;

        foreach (Transform child in slotParent) // Clears inventory ui
        {
            Destroy(child.gameObject);
        }

        foreach (var pair in inventoryItems) // Add current items to inventory ui and dictionary
        {
            Item item = pair.Value;
            if (item == null) continue;

            Sprite sprite = item.GetComponent<SpriteRenderer>()?.sprite;

            if (sprite != null && !items.ContainsKey(item))
            {
                items.Add(item, sprite);
            }

            GameObject slot = Instantiate(slotPrefab, slotParent);
            Image image = slot.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                image.enabled = true; // image that we replace with sprite gets for some reason disabled by default, this fixes that
            }
        }
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }
}
