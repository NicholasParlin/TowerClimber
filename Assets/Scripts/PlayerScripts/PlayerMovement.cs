using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterStateManager))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float _turnSmoothVelocity;

    [Header("References")]
    [Tooltip("Assign the main camera's Transform here.")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController _controller;
    private CharacterStateManager _stateManager;

    private Vector2 _currentInput = Vector2.zero;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stateManager = GetComponent<CharacterStateManager>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    // Removed the OnEnable and OnDisable methods as they are no longer needed for this script.

    // No Update() method is needed here anymore, the state machine will drive movement.

    public void SetMoveInput(Vector2 moveInput)
    {
        _currentInput = moveInput;
        // Inform the state manager if movement is being pressed. This is crucial for the state machine to work.
        _stateManager.IsMovementPressed = moveInput.x != 0 || moveInput.y != 0;
    }

    // This method is now PUBLIC so it can be called by PlayerMovingState.
    public void HandleMovement()
    {
        Vector3 direction = new Vector3(_currentInput.x, 0f, _currentInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    // The HandleStateChanged method has been removed as this logic is now controlled by the states themselves.
}