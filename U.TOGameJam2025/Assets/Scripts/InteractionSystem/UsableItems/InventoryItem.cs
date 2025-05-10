using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
[RequireComponent(typeof(PlaySoundOnUse))]
public class InventoryItem : MonoBehaviour, IUsable
{
    [field: SerializeField] public UnityEvent OnUse {get; private set;}
    public static event Action OnInventoryChanged;                        // Event to notify when money changes true is added, false is removed

    private ItemState _itemState = ItemState.DROPPED;

    public void Use(GameObject actor)
    {
        foreach (var component in FindObjectsByType<InteractorComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            component.DropItem(gameObject);                                 // Call the DropItem method on all InteractorComponents in the scene
        }
        
        OnUse?.Invoke();

        LootbagSystem.Instance.BagItem(this);  
    }

    public void Dropped()
    {
        PlayerInventory.Instance.currentMoney -= GetComponent<LootItem>().Value; 
        PlayerInventory.Instance.currentWeight -= GetComponent<LootItem>().Weight;

        _itemState = ItemState.DROPPED;

        OnInventoryChanged?.Invoke(); 
    }

    public void Collected()
    {
        PlayerInventory.Instance.currentMoney += GetComponent<LootItem>().Value; 
        PlayerInventory.Instance.currentWeight += GetComponent<LootItem>().Weight;

        _itemState = ItemState.IN_BAG;

        OnInventoryChanged?.Invoke(); 
    }

    private enum ItemState
    {
        DROPPED,
        IN_BAG
    }
}
