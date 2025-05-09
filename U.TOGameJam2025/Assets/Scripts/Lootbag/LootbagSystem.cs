using UnityEngine;

public class LootbagSystem : MonoBehaviour
{
    public static LootbagSystem Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

    
}
