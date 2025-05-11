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

    // private void SpawnLootFromBias()
    // {
    //     currentLootSpawnPrice = 0;

    //     List<(List<GameObject> prefabs, List<Transform> spawnPoints)> lootCategories = new()
    //     {
    //         (largeLootPrefabs, largeLootSpawnPoints),
    //         (mediumLootPrefabs, mediumLootSpawnPoints),   
    //         (smallLootPrefabs, smallLootSpawnPoints)
    //     };


    //     // Bias influences the starting index (0 = large, 1 = medium, 2 = small)
    //     int startIndex = Mathf.Clamp(Mathf.RoundToInt(100f - smallToLargeLootBias) / 50, 0, lootCategories.Count - 1);

    //     for (int i = startIndex; i <lootCategories.Count; i++)
    //     {
    //         TrySpawnFromCategory(lootCategories[i].prefabs, lootCategories[i].spawnPoints);

    //         if (currentLootSpawnPrice >= totalLootSpawnPrice) break;
            
    //     }
    // }

    private void SpawnLootFromBias()
{
    currentLootSpawnPrice = 0f;

    // Define weights based on bias (0 = small-heavy, 100 = large-heavy)
    float smallWeight = Mathf.Clamp01((100f - smallToLargeLootBias) / 100f);
    float largeWeight = Mathf.Clamp01(smallToLargeLootBias / 100f);
    float mediumWeight = 1f; // Always normalized later

    // Normalize weights so they sum to 1
    float totalWeight = smallWeight + mediumWeight + largeWeight;
    smallWeight /= totalWeight;
    mediumWeight /= totalWeight;
    largeWeight /= totalWeight;

    while (currentLootSpawnPrice < totalLootSpawnPrice)
    {
        float rand = UnityEngine.Random.value;

        // Decide which category to spawn from based on weights
        if (rand < smallWeight)
        {
            TrySpawnFromCategory(smallLootPrefabs, smallLootSpawnPoints);
        }
        else if (rand < smallWeight + mediumWeight)
        {
            TrySpawnFromCategory(mediumLootPrefabs, mediumLootSpawnPoints);
        }
        else
        {
            TrySpawnFromCategory(largeLootPrefabs, largeLootSpawnPoints);
        }
    }
}

    private void TrySpawnFromCategory(List<GameObject> prefabs, List<Transform> spawnPoints)
    {
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        while(availableSpawnPoints.Count > 0 && currentLootSpawnPrice < totalLootSpawnPrice)
        {
            int randomIndex = UnityEngine.Random.Range(0, prefabs.Count);
            GameObject candidatePrefab = prefabs[randomIndex];

            if(!candidatePrefab.TryGetComponent(out LootItem lootItem))
            {
                Debug.LogError($"Prefab {candidatePrefab.name} does not have a LootItem component.");
                continue;
            }   

            if (currentLootSpawnPrice + lootItem.Value > totalLootSpawnPrice)
            {
                // Try a cheaper item if this one's too expensive
                if (!TryFindAffordableItem(prefabs, out candidatePrefab, totalLootSpawnPrice - currentLootSpawnPrice))
                    break;

                lootItem = candidatePrefab.GetComponent<LootItem>();
            }

            int spawnIndex = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[spawnIndex];
            availableSpawnPoints.RemoveAt(spawnIndex);

            Instantiate(candidatePrefab, spawnPoint.position, Quaternion.identity, lootParentTransform);

            currentLootSpawnPrice += lootItem.Value;
        }
    }

    private bool TryFindAffordableItem(List<GameObject> lootPrefabs, out GameObject result, float maxPrice)
    {
        List<GameObject> affordable = lootPrefabs.FindAll(obj =>
        {
            if (obj.TryGetComponent(out LootItem item))
                return item.Value <= maxPrice;
            return false;
        });

        if (affordable.Count > 0)
        {
            result = affordable[UnityEngine.Random.Range(0, affordable.Count)];
            return true;
        }

        result = null;
        return false;
    }
}
