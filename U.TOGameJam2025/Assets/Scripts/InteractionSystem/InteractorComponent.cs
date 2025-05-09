using System;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractorComponent : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickupUI;
    [SerializeField] [Range(1.0f, 3.0f)] [Min(1.0f)] private float interactDistance;

    [Header("Held Item")]
    [SerializeField] private Transform pickUpParentTransform;
    [SerializeField] private GameObject inHandItem;
    [SerializeField] [Range(1.0f, 50.0f)] private float throwForce; 

    [Header("Input Actions")]
    private PlayerInput playerInput;
    private InputAction interactInput, dropInput, useInput;

    [Header("Audio")]
    [SerializeField] private AudioSource pickupAudioSource;
    private RaycastHit hit;
    private GameObject currentHitObject;

    private void Start()
    {
        #region Input Action Assets
        playerInput = GetComponent<PlayerInput>();                                 

        interactInput = playerInput.actions.FindAction("Interact");         
        dropInput = playerInput.actions.FindAction("Drop");                    
        useInput = playerInput.actions.FindAction("Use");                                                          

        interactInput.performed += PickUp;                                          
        dropInput.performed += OnDrop;                                             
        useInput.performed += OnUse;                      

        interactInput.Enable();                                                   
        dropInput.Enable();                                                        
        useInput.Enable();   
        #endregion      
    }

    void OnDestroy()
    {
        interactInput.performed -= PickUp;                                 
        dropInput.performed -= OnDrop;                                         
        useInput.performed -= OnUse;                                             
    }
    
    #region Interaction Methods
    private void OnUse(InputAction.CallbackContext context)
    {
        if(inHandItem != null)
        {
            IUsable usableObject = inHandItem.GetComponent<IUsable>();                  // Get the IUsable component from the inHandItem

            if(usableObject != null)
            {
                usableObject.Use(gameObject);                                            // Call the Use method on the usable object
            }
        }
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        if(inHandItem != null)
        {
            inHandItem.transform.SetParent(null);                                                               // Remove the parent of the item in hand
            inHandItem.transform.position = playerCameraTransform.position + playerCameraTransform.forward * 2; // Drop the item in front of the player

            if(inHandItem.TryGetComponent<Rigidbody>(out Rigidbody rb)) 
            {
                rb.isKinematic = false;                                                                         // Set it to non-kinematic
                rb.AddForce(playerCameraTransform.forward * throwForce, ForceMode.Impulse);                     // Add force to the item to "Throw" it
            }

            inHandItem = null;                                                                                  // Reset the inHandItem to null  
        }
    }

    private void PickUp(InputAction.CallbackContext context)
    {
        if(hit.collider != null && inHandItem == null)
        {
            IInteractable interactableObject = hit.collider.GetComponent<IInteractable>();                      // Get the IInteractable component from the hit object

            if(interactableObject != null)
            {
                if(pickupAudioSource != null)
                {
                    pickupAudioSource.Play();                                                                   // Play the pickup audio
                    Debug.Log("Playing pickup sound");
                }
                
                inHandItem = interactableObject.PickUp();                                                       // Call the PickUp method on the interactable object
                inHandItem.transform.SetParent(pickUpParentTransform, interactableObject.KeepWorldPosition);    // Set the parent of the picked up item to the pickUpParentTransform

                if(!interactableObject.KeepWorldPosition)
                    inHandItem.transform.localPosition = Vector3.zero;                                           // Reset the local position of the picked up item

            }
        }
    }
    #endregion

    
    private void Update()
    {
        CheckForInteractable();                                                     
    }

    void CheckForInteractable()
    {
        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, interactDistance, interactableLayerMask))
        {
            GameObject hitObject = hit.collider.gameObject;                             // Get the object that was hit by the raycast
            
            if(hitObject == currentHitObject) return;                                   // If the hit object is the same as the current one, return early

            if(currentHitObject != null)
            {
                currentHitObject.GetComponent<Highlight>()?.ToggleHighlight(false);     // Disable highlight on the previous object
                pickupUI.SetActive(false);                                              // Disable pickup UI if the object is not interactable     
            }

            if(inHandItem != null) return;                                              // If an item is already in hand, skip the raycast check

            currentHitObject = hitObject;                                               // Update the current hit object

            Highlight highlight = hitObject.GetComponent<Highlight>();                  // Get the Highlight component from the hit object

            if(highlight == null)
               highlight = hitObject.AddComponent<Highlight>();                         // Add a Highlight component to the hit object if it doesn't have one

            highlight.ToggleHighlight(true);
    
            pickupUI?.SetActive(true);                                                   // Enable pickup UI if the object is interactable
        }
        else
        {
            if(currentHitObject != null)
            {
                currentHitObject.GetComponent<Highlight>()?.ToggleHighlight(false);  
                pickupUI.SetActive(false);    
                currentHitObject = null;                                                
            }
        }
    }

    void OnDrawGizmos()
    {
        Color color = (hit.collider != null) ? Color.green : Color.red;
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * interactDistance, color);
    }
}
