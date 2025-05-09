using System;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractorComponent : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickupUI;
    [SerializeField] [Range(1.0f, 3.0f)] [Min(1.0f)] private float interactDistance;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction interactInput, dropInput, useInput;

    private RaycastHit hit;
    private GameObject currentHitObject;

    private void Start()
    {
        #region Input Action Assets
        interactInput = inputActionAsset.FindAction("Interact");         
        dropInput = inputActionAsset.FindAction("Drop");                    
        useInput = inputActionAsset.FindAction("Use");                                                          

        interactInput.performed += OnInteract;                                          
        dropInput.performed += OnDrop;                                             
        useInput.performed += OnUse;                      

        interactInput.Enable();                                                   
        dropInput.Enable();                                                        
        useInput.Enable();   
        #endregion      
    }

    void OnDestroy()
    {
        interactInput.performed -= OnInteract;                                 
        dropInput.performed -= OnDrop;                                         
        useInput.performed -= OnUse;                                             
    }
    
    #region Interaction Methods
    private void OnUse(InputAction.CallbackContext context)
    {
        Debug.Log("Use action performed!");                                 // Placeholder for use action
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        Debug.Log("Drop action performed!");                                 // Placeholder for drop action
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if(hit.collider != null)
        {
            Debug.Log(hit.collider.name);
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
            GameObject hitObject = hit.collider.gameObject;                         // Get the object that was hit by the raycast
            
            if(hitObject == currentHitObject) return;                                   // If the hit object is the same as the current one, return early

            if(currentHitObject != null)
            {
                currentHitObject.GetComponent<Highlight>()?.ToggleHighlight(false);     // Disable highlight on the previous object
                pickupUI.SetActive(false);                                              // Disable pickup UI if the object is not interactable     
            }

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
