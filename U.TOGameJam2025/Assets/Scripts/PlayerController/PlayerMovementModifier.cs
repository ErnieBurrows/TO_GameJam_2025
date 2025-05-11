using System;
using UnityEngine;

public class PlayerMovementModifier : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] [Range(0.0f, 1.0f)] private float encumbranceThreshold = 0.5f; 
    [SerializeField] [Range(0.0f, 1.0f)] private float modifierBias = 0.5f; 


    public static event Action<float, bool> OnMovementModifierChanged;  // Float for modifier, bool for isEncumbered

    void OnEnable()
    {
        InventoryItem.OnInventoryChanged += UpdateMovementModifier;
    }

    void OnDisable()
    {
        InventoryItem.OnInventoryChanged -= UpdateMovementModifier;
    }

    private void UpdateMovementModifier()
    {
        float currentWeight = PlayerInventory.Instance.currentWeight;
        float maxWeight = PlayerInventory.Instance.maxWeight;

        if(currentWeight / maxWeight < encumbranceThreshold) return;

        float modifier = 1 + modifierBias - (currentWeight / maxWeight);

        bool isEncumbered = currentWeight >= maxWeight;

        OnMovementModifierChanged?.Invoke(modifier, isEncumbered);

        Debug.Log($"Current Weight: {currentWeight}, Max Weight: {maxWeight}, Modifier: {modifier}, Is Encumbered: {isEncumbered}");
    }
}
