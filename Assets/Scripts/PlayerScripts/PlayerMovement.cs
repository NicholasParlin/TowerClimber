using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterStateManager))] // NEW: Now requires the state manager
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
    private CharacterStateManager _stateManager; // NEW: Reference to the state manager

    // State variable to control movement
    private bool _canMove = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stateManager = GetComponent<CharacterStateManager>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.LogWarning("Camera Transform was not assigned. Defaulting to main camera.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to the state manager's event.
        if (_stateManager != null)
        {
            _stateManager.OnStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        // Always unsubscribe from events.
        if (_stateManager != null)
        {
            _stateManager.OnStateChanged -= HandleStateChanged;
        }
    }

    private void Update()
    {
        // If we can't move, do nothing.
        if (!_canMove)
        {
            _controller.Move(Vector3.zero);
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// This method is called by the OnStateChanged event from the state manager.
    /// </summary>
    private void HandleStateChanged(CharacterState newState)
    {
        // Movement is only allowed in the Idle and Moving states.
        _canMove = (newState == CharacterState.Idle || newState == CharacterState.Moving);
    }
}