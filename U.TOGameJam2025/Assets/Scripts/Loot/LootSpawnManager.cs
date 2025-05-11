using System;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawnManager : MonoBehaviour
{
    [Header("Loot Spawn Zones")]
    [SerializeField] private GameObject smallLootSpawnZones, mediumLootSpawnZones, largeLootSpawnZones;

    [Header("Loot Prefabs (Per Category)")]
    [SerializeField] private List<GameObject> smallLootPrefabs;
    [SerializeField] private List<GameObject> mediumLootPrefabs;   
    [SerializeField] private List<GameObject> largeLootPrefabs;

    [Header("Loot Spawn Settings")]
    [SerializeField] private float totalLootSpawnPrice;
    [SerializeField] [Range(0, 100)] private float smallToLargeLootBias;
    [SerializeField] private Transform lootParentTransform;
    private float currentLootSpawnPrice;    
    private List<Transform> smallLootSpawnPoints = new List<Transform>();
    private List<Transform> mediumLootSpawnPoints = new List<Transform>();  
    private List<Transform> largeLootSpawnPoints = new List<Transform>();

    void Awake()
    {
        smallLootSpawnPoints.AddRange(smallLootSpawnZones.GetComponentsInChildren<Transform>());
        mediumLootSpawnPoints.AddRange(mediumLootSpawnZones.GetComponentsInChildren<Transform>());
        largeLootSpawnPoints.AddRange(largeLootSpawnZones.GetComponentsInChildren<Transform>()); 

        GameStateManager.OnGameStateManagerInitialized += SpawnLootFromBias;
    }

    void OnDisable()
    {
        GameStateManager.OnGameStateManagerInitialized -= SpawnLootFromBias;
    }

   private void SpawnLootFromBias()
    {
        currentLootSpawnPrice = 0f;

        float smallWeight = Mathf.Clamp01((100f - smallToLargeLootBias) / 100f);
        float largeWeight = Mathf.Clamp01(smallToLargeLootBias / 100f);
        float mediumWeight = 1f;

        float totalWeight = smallWeight + mediumWeight + largeWeight;
        smallWeight /= totalWeight;
        mediumWeight /= totalWeight;
        largeWeight /= totalWeight;

        const int maxAttempts = 100; // Prevent infinite loops

        for (int i = 0; i < maxAttempts && currentLootSpawnPrice <= totalLootSpawnPrice; i++)
        {
            float rand = UnityEngine.Random.value;

            if (rand < smallWeight)
            {
                if (!TrySpawnFromCategory(smallLootPrefabs, smallLootSpawnPoints)) continue;
            }
            else if (rand < smallWeight + mediumWeight)
            {
                if (!TrySpawnFromCategory(mediumLootPrefabs, mediumLootSpawnPoints)) continue;
            }
            else
            {
                if (!TrySpawnFromCategory(largeLootPrefabs, largeLootSpawnPoints)) continue;
            }
        }
    }
    

    private bool TrySpawnFromCategory(List<GameObject> prefabs, List<Transform> spawnPoints)
    {
        if (spawnPoints.Count == 0 || prefabs.Count == 0)
            return false;

        GameObject candidatePrefab = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];

        if (!candidatePrefab.TryGetComponent(out LootItem lootItem))
        {
            Debug.LogError($"Prefab {candidatePrefab.name} does not have a LootItem component.");
            return false;
        }

        // Too expensive
        if (currentLootSpawnPrice + lootItem.Value > totalLootSpawnPrice)
        {
            if (!TryFindAffordableItem(prefabs, out candidatePrefab, totalLootSpawnPrice - currentLootSpawnPrice))
                return false;

            lootItem = candidatePrefab.GetComponent<LootItem>();
        }

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
        Instantiate(candidatePrefab, spawnPoint.position, Quaternion.identity, lootParentTransform);

        currentLootSpawnPrice += lootItem.Value;
        return true;
    }

    private bool TryFindAffordableItem(List<GameObject> prefabs, out GameObject affordablePrefab, float maxPrice)
    {
        foreach (var prefab in prefabs)
        {
            if (prefab.TryGetComponent(out LootItem lootItem) && lootItem.Value <= maxPrice)
            {
                affordablePrefab = prefab;
                return true;
            }
        }

        affordablePrefab = null;
        return false;
    }
}
