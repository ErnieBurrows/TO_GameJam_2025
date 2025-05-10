using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public Vector2 sensitivity;
    public Vector2 rotation;
    public Transform orientation;

    private PlayerInput playerInput;
    private InputAction lookAction;

    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        lookAction = playerInput.actions.FindAction("Look");
        lookAction.Enable();

        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false; 
    }

    void LateUpdate()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>() * sensitivity;

        rotation.y += lookInput.x;                                          // Horizontal rotation
        rotation.x -= lookInput.y;                                          // Vertical rotation

        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f); 

        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);   // Apply rotation to the camera
        orientation.rotation = Quaternion.Euler(0, rotation.y, 0);          // Apply rotation to the orientation (player body)
    }
}
