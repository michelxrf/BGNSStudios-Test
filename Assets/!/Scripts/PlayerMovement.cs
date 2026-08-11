using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player's ground movement
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;
    private PlayerInput _playerInput;
    private Animator _animator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private Vector3 _velocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Prevent movement while in inventory
        if (InventorySystem.instance.IsInventoryOpen) return;

        // Get input from Move action
        Vector2 moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();

        // Convert 2D input to 3D movement in the character's local space
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        // Apply speed
        Vector3 moveVelocity = moveDirection.normalized * moveSpeed;

        // Apply gravity
        if (!_characterController.isGrounded)
        {
            _velocity.y += gravity * Time.deltaTime;
        }
        else if (_velocity.y < 0)
        {
            _velocity.y = 0f;
        }

        _velocity.x = moveVelocity.x;
        _velocity.z = moveVelocity.z;

        // Move the character
        _characterController.Move(_velocity * Time.deltaTime);
        _animator.SetFloat("Speed", moveVelocity.magnitude);
    }
}
