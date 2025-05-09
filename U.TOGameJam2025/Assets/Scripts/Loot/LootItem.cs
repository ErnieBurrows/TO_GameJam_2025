using UnityEngine;

public class LootItem : MonoBehaviour
{
    [SerializeField] private LootItemData lootItemData;

    public float Value => lootItemData.value;
    public float Weight => lootItemData.weight;
    public string ItemName => lootItemData.itemName;
}
