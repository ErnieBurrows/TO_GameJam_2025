using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellPlatform : MonoBehaviour
{
    // --------------------------------------------------
    public static SellPlatform Instance;
    // --------------------------------------------------
    [SerializeField] List<TextMeshProUGUI> _platformLabelTMPro;
    // --------------------------------------------------
    private List<LootItem> _items = new List<LootItem>();
    private static float _currentValue;

    public static float CurrentValue => _currentValue;
    // --------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    // --------------------------------------------------
    private void Start()
    {
        RecalculateValue();
    }
    // --------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        LootItem lootItem = other.gameObject.GetComponent<LootItem>();
        if (lootItem)
        {
            if (_items.Contains(lootItem)) return;          // List already has this item, do nothing

            _items.Add(lootItem);
            RecalculateValue();

            Debug.Log("<SellPlatform> Item added.");
        }
    }
    // --------------------------------------------------
    private void OnTriggerExit(Collider other)
    {
        LootItem lootItem = other.gameObject.GetComponent<LootItem>();
        if (lootItem)
        {
            if (!_items.Contains(lootItem)) return;          // List isn't in the list, do nothing

            _items.Remove(lootItem);
            RecalculateValue();

            Debug.Log("<SellPlatform> Item removed.");
        }
    }
    // --------------------------------------------------
    private void RecalculateValue()
    {
        _currentValue = 0;

        foreach (var item in _items)
        {
            _currentValue += item.Value;
        }

        foreach (var display in _platformLabelTMPro)
        {
            display.text = $"Value\n${_currentValue}";
        }

        Debug.Log("<SellPlatform> Current Value -> " + _currentValue);
    }
}
