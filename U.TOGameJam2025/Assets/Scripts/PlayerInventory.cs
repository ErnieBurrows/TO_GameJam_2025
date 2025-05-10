using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    #region Singleton
    private static PlayerInventory instance;
    public static PlayerInventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerInventory>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("PlayerInventory");
                    instance = obj.AddComponent<PlayerInventory>();
                }
            }
            return instance;
        }
    }
    #endregion

    public float currentMoney = 0;
}
