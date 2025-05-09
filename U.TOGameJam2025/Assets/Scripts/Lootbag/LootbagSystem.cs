using System.Collections.Generic;
using UnityEngine;

public class LootbagSystem : MonoBehaviour
{
    // --------------------------------------------------
    // [[SINGLETON]]
    public static LootbagSystem Instance;
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
        Debug.Log(_itemDropLocation);
    }
    // --------------------------------------------------
    public void BagItem(InventoryItem item)
    {
        _inventoryItems.Add(item);

        // Put item into lootbag on screen
        item.gameObject.layer = 8; // Change to LootBag layer
        item.transform.position = _itemDropLocation;
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX;
    }
    // --------------------------------------------------
    public void Dropitem(InventoryItem item)
    {
        _inventoryItems.Remove(item);
    }
}
