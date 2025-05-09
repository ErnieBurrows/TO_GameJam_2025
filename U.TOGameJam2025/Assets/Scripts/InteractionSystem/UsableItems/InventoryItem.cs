using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[RequireComponent(typeof(PlaySoundOnUse))]
public class InventoryItem : MonoBehaviour, IUsable
{
    [field: SerializeField] public UnityEvent OnUse {get; private set;}

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
}
