using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player's camera movement
/// </summary>
public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;

    [Header("Look Settings")]
    [SerializeField, Range(0f, 5f)] private float lookSensitivity = 0.5f;
    [SerializeField, Range(-90f, 90f)] private float lookClamp = 75f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Prevent movement while in inventory
        if (InventorySystem.instance.IsInventoryOpen) return;

        // prevent movement while paused
        if (PauseManager.instance.IsPaused) return;

        // prevent movement while in dialogue
        if (DialogueManager.instance.IsDialogueActive) return;

        Vector2 lookInput = Mouse.current.delta.ReadValue();

        // Horizontal rotation
        float horizontalRotation = lookInput.x * lookSensitivity;
        transform.Rotate(0, horizontalRotation, 0);

        // Vertical rotation
        float verticalRotation = -lookInput.y * lookSensitivity;
        Vector3 cameraEuler = _cameraTransform.localEulerAngles;
        float newVerticalAngle = cameraEuler.x + verticalRotation;

        // Normalize angle to -180 to 180 range, then clamp
        if (newVerticalAngle > 180)
            newVerticalAngle -= 360;
        newVerticalAngle = Mathf.Clamp(newVerticalAngle, -lookClamp, lookClamp);
        _cameraTransform.localEulerAngles = new Vector3(newVerticalAngle, 0, 0);
    }
}
