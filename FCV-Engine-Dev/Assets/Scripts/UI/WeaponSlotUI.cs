using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

/// <summary>
/// Handles the individual weapon slot UI, including displaying weapon sprites, slot indices, and managing the border state.
/// </summary>
public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text slotPosition;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image borderImage;

    /// <summary>
    /// Sets the image for the weapon slot.
    /// </summary>
    /// <param name="weaponSprite">The sprite representing the weapon to display in the slot.</param>
    public void SetImage(Sprite weaponSprite)
    {
        weaponImage.sprite = weaponSprite;
    }
    
    /// <summary>
    /// Sets the text for the weapon slot, indicating the key to press for equipping the weapon.
    /// </summary>
    /// <param name="key">The key index (typically incremented from the weapon controller) associated with the weapon.</param>
    public void SetSlotIndex(int key)
    {
        slotPosition.text = $"{key}";
    }

    /// <summary>
    /// Sets the active state of the slot border.
    /// </summary>
    /// <param name="active">True to activate the border, false to deactivate it.</param>
    public void SetBorderActiveState(bool active)
    {
        borderImage.enabled = active;
    }
}
