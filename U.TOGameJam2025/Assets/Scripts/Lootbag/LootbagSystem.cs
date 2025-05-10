using System.Collections.Generic;
using UnityEngine;

public class LootbagSystem : MonoBehaviour
{
    // --------------------------------------------------
    // [[SINGLETON]]
    public static LootbagSystem Instance;
    // --------------------------------------------------
    // [[REFERENCES]]
    private Camera _lootbagCamera;

    public Camera LootbagCamera => _lootbagCamera;
    // --------------------------------------------------
    private List<InventoryItem> _inventoryItems = new List<InventoryItem>();
    private Vector3 _itemDropLocation;
    // --------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;

        _itemDropLocation = transform.Find("DropPosition").position;
        _lootbagCamera = transform.Find("Camera").GetComponent<Camera>();
    }
    // --------------------------------------------------
    public void BagItem(InventoryItem item)
    {
        _inventoryItems.Add(item);
        item.Collected();

        // Put item into lootbag on screen
        item.gameObject.layer = 8; // Change to LootBag layer
        item.transform.position = _itemDropLocation;
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX;
    }
    // --------------------------------------------------
    public void DropItem(InventoryItem item)
    {
        _inventoryItems.Remove(item);
        item.Dropped();

        // Put item back into the game world.
        item.gameObject.layer = 7; // Interactable Layer
        item.transform.position = GameObject.Find("Player").transform.forward + (Vector3.forward * 1.5f);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
    }
}
