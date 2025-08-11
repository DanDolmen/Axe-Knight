using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the UI representation of weapon slots, including adding new slots, setting images, and handling weapon changes.
/// </summary>
public class WeaponUI : MonoBehaviour
{
    [SerializeField] private WeaponSlotUI slotPrefab;
    [SerializeField] private RectTransform weaponUIHolder;

    [SerializeField] private bool setupEventsAutomatically = false;

    private List<WeaponSlotUI> weaponSlotUis;
    
    /// <summary>
    /// Sets up events for weapon actions from the WeaponController.
    /// </summary>
    private void SetupEventsAutomatically()
    {
        
        WeaponController weaponController = FindObjectOfType<WeaponController>();

        if (!weaponController) return;
        
        if (!MethodSubscribed(weaponController.weaponPickedUp, nameof(AddWeaponSlot)))
            weaponController.weaponPickedUp.AddListener(AddWeaponSlot);
        
        if (!MethodSubscribed(weaponController.weaponPickedUp, nameof(WeaponChanged)))
            weaponController.weaponChanged.AddListener(WeaponChanged);
        
    }

    private void Start()
    {
        weaponSlotUis = new List<WeaponSlotUI>();
        if (setupEventsAutomatically)
            SetupEventsAutomatically();
    }
    
    /// <summary>
    /// Checks if a Unity Event has a specific method subscribed.
    /// </summary>
    /// <param name="event">The Unity event to check for a subscribed method.</param>
    /// <param name="methodName">The name of the method to look for in the event's subscribers.</param>
    /// <returns>True if the method is subscribed to the event, false otherwise.</returns>
    private bool MethodSubscribed([NotNull] UnityEventBase @event, string methodName)
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event));
        
        for (int i = 0; i < @event.GetPersistentEventCount(); i++)
        {
            if (@event.GetPersistentMethodName(i) == methodName) return true;
        }

        return false;
    }
    
    /// <summary>
    /// Adds and sets up a weapon slot UI for the provided weapon.
    /// </summary>
    /// <param name="weapon">Weapon to be added to UI.</param>
    /// <param name="key">The key used to equip the weapon when changing weapons.</param>
    public void AddWeaponSlot(Weapon weapon, int key)
    {
        WeaponSlotUI weaponSlotUI = Instantiate(slotPrefab, weaponUIHolder);
        Sprite sprite;

        if (weapon.TryGetComponent(out SpriteRenderer spriteRenderer))
            sprite = spriteRenderer.sprite;
        else
        {
            sprite = weapon.GetComponentInChildren<SpriteRenderer>().sprite;
        }

        if (sprite)
            weaponSlotUI.SetImage(sprite);

        weaponSlotUI.SetSlotIndex(key);
        
        weaponSlotUis.Add(weaponSlotUI);
    }
    
    /// <summary>
    /// Activates the border of the weapon slot at the given index and deactivates all other borders.
    /// </summary>
    /// <param name="index">Index of the weapon slot to activate.</param>
    public void WeaponChanged(int index)
    {
        for (int i = 0; i < weaponSlotUis.Count; i++)
        {
            weaponSlotUis[i].SetBorderActiveState(i == index);
        }
    }

}
