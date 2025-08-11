using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Custom collider component to make events from collider to be shown in inspector by using UnityEvents
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
class Custom2DCollider : MonoBehaviour
{
    [HideInInspector, SerializeField] private bool isTrigger = false;
    [HideInInspector, SerializeField] private bool checkItems = false;
    [HideInInspector, SerializeField] private bool needAllItems = false;
    [SerializeField] private LayerMask collisionLayers;
    [HideInInspector, SerializeField, ItemName] private List<string> itemsToCheck;
    
    [HideInInspector, SerializeField] private UnityEvent onCollisionEnter;

    [HideInInspector, SerializeField] private UnityEvent onCollisionStay;

    [HideInInspector, SerializeField] private UnityEvent onCollisionExit;
    
    [HideInInspector, SerializeField] private UnityEvent onTriggerEnter;
    
    [HideInInspector, SerializeField] private UnityEvent onTriggerStay;
    
    [HideInInspector, SerializeField] private UnityEvent onTriggerExit;
    

	private void OnValidate()
	{
		GetComponent<Rigidbody2D>().isKinematic = true;
	}
	
    private void Awake()
    {
        if (TryGetComponent(out Collider2D collider2D))
        {
            collider2D.isTrigger = isTrigger;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FireEvent(ref onTriggerEnter, other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        FireEvent(ref onTriggerStay, other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        FireEvent(ref onTriggerExit, other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        FireEvent(ref onCollisionEnter, other.gameObject);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        FireEvent(ref onCollisionStay, other.gameObject);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        FireEvent(ref onCollisionExit, other.gameObject);
    }

    /// <summary>
    /// Fires of the designated event and checking if collidingObject is in layermask and if check items is true checks the inventory for correct items
    /// </summary>
    /// <param name="unityEvent"></param>
    /// <param name="collidingObject"></param>
    private void FireEvent(ref UnityEvent unityEvent, GameObject collidingObject)
    {
        if (!LayerMaskUtility.IsInLayerMask(collidingObject, collisionLayers)) return;

        if (!checkItems)
        {
            unityEvent.Invoke();
            return;
        }
        
        if (collidingObject.TryGetComponent(out Inventory inventory) && CheckItems(inventory))
            unityEvent.Invoke();
    }
    
    /// <summary>
    /// Checks the inventory if the specified items are in inventory 
    /// </summary>
    /// <param name="inventory">Inventory to check against</param>
    /// <returns></returns>
    private bool CheckItems(Inventory inventory)
    {
        foreach (var hasItem in itemsToCheck.Select(inventory.HasItem))
        {
            switch (hasItem)
            {
                case true when !needAllItems:
                    return true;
                case false when needAllItems:
                    return false;
            }
        }
        
        return needAllItems;
    }
}
#if UNITY_EDITOR
/// <summary>
/// Custom editor for Custom2DCollider class.
/// Changes what unity event is shown based on if the objects collider has IsTrigger to true or not.
/// Adds field to assign what items to check for if checkItems bool is true.
/// </summary>
[CustomEditor(typeof(Custom2DCollider))]
public class Custom2DColliderEditor : Editor
{
    private List<SerializedProperty> _events;
    private SerializedProperty _isTriggerProperty;
    private SerializedProperty _checkItemsProperty;
    private SerializedProperty _itemsProperty;
    private SerializedProperty _needAllItemsProperty;
    private void OnEnable()
    {
        _isTriggerProperty = serializedObject.FindProperty("isTrigger");
        _checkItemsProperty = serializedObject.FindProperty("checkItems");
        _itemsProperty = serializedObject.FindProperty("itemsToCheck");
        _needAllItemsProperty = serializedObject.FindProperty("needAllItems");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        AssignEvents();

        EditorGUILayout.PropertyField(_isTriggerProperty);

        EditorGUILayout.PropertyField(_checkItemsProperty);

        if (_checkItemsProperty.boolValue)
        {
            EditorGUILayout.PropertyField(_needAllItemsProperty);
            EditorGUILayout.PropertyField(_itemsProperty);
        }
        
        foreach (var eventProperty in _events)
        {
            EditorGUILayout.PropertyField(eventProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AssignEvents()
    {
        _events = new();

        if (_isTriggerProperty.boolValue)
        {
            _events.Add(serializedObject.FindProperty("onTriggerEnter"));
            _events.Add(serializedObject.FindProperty("onTriggerStay"));
            _events.Add(serializedObject.FindProperty("onTriggerExit"));
        }
        else
        {
            _events.Add(serializedObject.FindProperty("onCollisionEnter"));
            _events.Add(serializedObject.FindProperty("onCollisionStay"));
            _events.Add(serializedObject.FindProperty("onCollisionExit"));
        }

    }
}
#endif