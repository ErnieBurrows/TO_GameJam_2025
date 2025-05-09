using UnityEngine;

public class HeldItem : MonoBehaviour, IInteractable
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

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;

        return gameObject;
    }
}
