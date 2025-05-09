using UnityEngine;

[CreateAssetMenu(fileName = "LootItemData", menuName = "Scriptable Objects/LootItemData")]
public class LootItemData : ScriptableObject
{
    public string itemName;
    public float weight;
    public int value;
}
