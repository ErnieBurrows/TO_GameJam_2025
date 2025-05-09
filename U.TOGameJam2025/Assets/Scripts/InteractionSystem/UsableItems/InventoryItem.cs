using UnityEngine;
using UnityEngine.Events;

public class InventoryItem : MonoBehaviour, IUsable
{
    [field: SerializeField] public UnityEvent OnUse {get; private set;}

    public void Use(GameObject actor)
    {
        OnUse?.Invoke();
        Debug.Log("Using item: " + gameObject.name);
    }
}
