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

    private bool _canMove = true;
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

    private void OnEnable()
    {
        if (_stateManager != null)
        {
            _stateManager.OnStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (_stateManager != null)
        {
            _stateManager.OnStateChanged -= HandleStateChanged;
        }
    }

    private void Update()
    {
        if (!_canMove)
        {
            _controller.Move(Vector3.zero);
            return;
        }
        HandleMovement();
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        _currentInput = moveInput;
    }

    private void HandleMovement()
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

    private void HandleStateChanged(CharacterState newState)
    {
        _canMove = (newState == CharacterState.Idle || newState == CharacterState.Moving);
        if (!_canMove)
        {
            _currentInput = Vector2.zero;
        }
    }
}