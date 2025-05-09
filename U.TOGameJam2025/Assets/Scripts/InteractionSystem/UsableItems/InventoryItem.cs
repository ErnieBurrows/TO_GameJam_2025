using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(PlaySoundOnUse))]
public class InventoryItem : MonoBehaviour, IUsable
{
    [field: SerializeField] public UnityEvent OnUse {get; private set;}

    private ItemState _itemState = ItemState.DROPPED;

    public void Use(GameObject actor)
    {
        foreach (var component in FindObjectsByType<InteractorComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            component.DropItem(gameObject);                                 // Call the DropItem method on all InteractorComponents in the scene
        }
        OnUse?.Invoke();

        LootbagSystem.Instance.BagItem(this);

        Debug.Log("Using item: " + gameObject.name);
    }

    public void Dropped()
    {
        Debug.Log(gameObject.name + " was dropped.");
        _itemState = ItemState.DROPPED;
    }

    public void Collected()
    {
        Debug.Log(gameObject.name + " was picked up.");
        _itemState = ItemState.IN_BAG;
    }

    private enum ItemState
    {
        DROPPED,
        IN_BAG
    }
}
