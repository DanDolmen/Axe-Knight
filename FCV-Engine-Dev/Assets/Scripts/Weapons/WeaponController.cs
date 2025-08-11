using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
/// <summary>
/// Controls what weapons are picked up, which ones are to be active and sends event accordingly
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform weaponParentTransform;
    [SerializeField] private float pickupDistance = 1f;

    [SerializeField] public UnityEvent<Weapon, int> weaponPickedUp;
    [SerializeField] public UnityEvent<int> weaponChanged;


    private int currentIndex;
    private List<Weapon> equippedWeapons = new();
    
    
    private List<Weapon> weapons;

    private static readonly KeyCode[] NumberKeys =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9
    };

    private void Start()
    {
        weapons = FindObjectsOfType<Weapon>().ToList();
    }
    
    /// <summary>
    /// Checks for distance to weapons and adds them to list if they are close enough to be picked up.
    /// Checks for inputs to control which is the active weapon
    /// </summary>
    private void Update()
    {
        // checks for distance to weapons and add them if they are close enough
        for (int i = weapons.Count - 1; i >= 0; i--)
        {
            if (Vector2.Distance(transform.position, weapons[i].transform.position) > pickupDistance) continue;
            
            AddWeapon(weapons[i]);
            SetActiveWeapon(equippedWeapons.Count - 1);
        }
        
        CheckForInput();
    }

    /// <summary>
    /// Sets the weapon up to be stored in the weapon controller
    /// </summary>
    /// <param name="weapon">Weapon to be added</param>
    private void AddWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(weaponParentTransform);
        weapon.transform.localPosition = Vector2.zero;
        weapon.gameObject.SetActive(false);
        
        equippedWeapons.Add(weapon);
        weapons.Remove(weapon);
        weaponPickedUp.Invoke(weapon, equippedWeapons.Count);
        weapon.GetComponent<FloatAnimation>()?.StopAnimation();
    }
    
    /// <summary>
    /// If input key was pressed set that to active weapon, only checks up to amount of active weapons
    /// </summary>
    private void CheckForInput()
    {
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            if (!Input.GetKeyDown(NumberKeys[i])) continue;
            
            SetActiveWeapon(i);
        }
    }
    /// <summary>
    /// Unequips old weapons and equips new one
    /// </summary>
    /// <param name="index">index for weapon</param>
    private void SetActiveWeapon(int index)
    {
        equippedWeapons[currentIndex].UnEquip();
        equippedWeapons[currentIndex].gameObject.SetActive(false);
            
        currentIndex = index;
            
        equippedWeapons[currentIndex].gameObject.SetActive(true);
        equippedWeapons[currentIndex].Equip();
        
        weaponChanged.Invoke(currentIndex);
    }
}
