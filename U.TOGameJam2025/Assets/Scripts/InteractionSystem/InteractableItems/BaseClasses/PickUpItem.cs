using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable
{
    [field: SerializeField] public bool KeepWorldPosition { get; private set;}

    private Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public GameObject PickUp()
    {
        if(rb != null)
            rb.isKinematic = true;

        return gameObject; 
    }
}
