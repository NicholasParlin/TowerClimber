using UnityEngine;

// This script requires a CharacterController to be attached to the same GameObject.
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerSkillManager))] // We need access to the skill manager
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float _turnSmoothVelocity;

    [Header("References")]
    [Tooltip("Assign the main camera's Transform here.")]
    [SerializeField] private Transform cameraTransform;

    // Component references
    private CharacterController _controller;
    private PlayerSkillManager _skillManager;

    // State variable to control movement
    private bool _canMove = true;

    private void Awake()
    {
        // Get the required components on this GameObject.
        _controller = GetComponent<CharacterController>();
        _skillManager = GetComponent<PlayerSkillManager>();

        // Ensure the camera transform is assigned to prevent errors.
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.LogWarning("Camera Transform was not assigned. Defaulting to main camera.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to the skill manager's events when this component is enabled.
        _skillManager.OnSkillActivationStart += DisableMovement;
        _skillManager.OnSkillActivationEnd += EnableMovement;
    }

    private void OnDisable()
    {
        // Always unsubscribe from events when this component is disabled to prevent memory leaks.
        _skillManager.OnSkillActivationStart -= DisableMovement;
        _skillManager.OnSkillActivationEnd -= EnableMovement;
    }

    private void Update()
    {
        // If we can't move (e.g., during a skill animation), do nothing.
        if (!_canMove)
        {
            // Ensure the character stops moving if they are locked mid-stride.
            _controller.Move(Vector3.zero);
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        // Read input from the player (e.g., WASD or controller stick).
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Only process movement if there is some input.
        if (direction.magnitude >= 0.1f)
        {
            // Calculate the angle to rotate the player based on camera direction.
            // This makes the player move relative to where the camera is looking.
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            // Smoothly rotate the player towards the target angle.
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Calculate the final movement vector.
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            // Apply movement using the CharacterController.
            _controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    // This method is called by the OnSkillActivationStart event.
    private void DisableMovement()
    {
        _canMove = false;
    }

    // This method is called by the OnSkillActivationEnd event.
    private void EnableMovement()
    {
        _canMove = true;
    }
}