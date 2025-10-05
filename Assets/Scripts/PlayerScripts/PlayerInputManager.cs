using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterStateManager), typeof(PlayerMovement))]
public class PlayerInputManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private PlayerInteraction playerInteraction;
    private PlayerMovement _playerMovement;

    [Header("UI Panel References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private CharacterPanelUI characterPanelUI;
    [SerializeField] private QuestJournalUI questJournalUI;
    [SerializeField] private PauseMenuUI pauseMenuUI;

    private CharacterStateManager _stateManager;
    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _stateManager = GetComponent<CharacterStateManager>();
        _playerMovement = GetComponent<PlayerMovement>();
        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // REMOVED: No longer subscribing to OnStateChanged
        // if (_stateManager != null) _stateManager.OnStateChanged += HandleStateChanged;

        _inputActions.Player.Enable();

        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Player.Interact.performed += OnInteract;
        _inputActions.Player.Pause.performed += OnPause;

        _inputActions.Player.ToggleInventory.performed += OnToggleInventory;
        _inputActions.Player.ToggleCharacterPanel.performed += OnToggleCharacterPanel;
        _inputActions.Player.ToggleQuestJournal.performed += OnToggleQuestJournal;

        _inputActions.Player.Skill1.performed += ctx => OnSkillButtonPressed(0);
        _inputActions.Player.Skill2.performed += ctx => OnSkillButtonPressed(1);
        _inputActions.Player.Skill3.performed += ctx => OnSkillButtonPressed(2);
        _inputActions.Player.Skill4.performed += ctx => OnSkillButtonPressed(3);
        _inputActions.Player.Skill5.performed += ctx => OnSkillButtonPressed(4);
        _inputActions.Player.Skill6.performed += ctx => OnSkillButtonPressed(5);
        _inputActions.Player.Skill7.performed += ctx => OnSkillButtonPressed(6);
        _inputActions.Player.Skill8.performed += ctx => OnSkillButtonPressed(7);
        _inputActions.Player.Skill9.performed += ctx => OnSkillButtonPressed(8);
    }

    private void OnDisable()
    {
        // REMOVED: No longer unsubscribing from OnStateChanged
        // if (_stateManager != null) _stateManager.OnStateChanged -= HandleStateChanged;

        _inputActions.Player.Disable();

        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;
        _inputActions.Player.Interact.performed -= OnInteract;
        _inputActions.Player.Pause.performed -= OnPause;

        _inputActions.Player.ToggleInventory.performed -= OnToggleInventory;
        _inputActions.Player.ToggleCharacterPanel.performed -= OnToggleCharacterPanel;
        _inputActions.Player.ToggleQuestJournal.performed -= OnToggleQuestJournal;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (_playerMovement != null)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            _playerMovement.SetMoveInput(moveInput);
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (CanProcessGameplayInput() && playerInteraction != null) playerInteraction.TryInteract();
    }

    private void OnSkillButtonPressed(int slotIndex)
    {
        if (CanProcessGameplayInput() && playerSkillManager != null)
        {
            SkillbarManager.Instance.ActivateSkillInSlot(slotIndex);
        }
    }

    private void OnToggleInventory(InputAction.CallbackContext context) => TogglePanel(inventoryUI);
    private void OnToggleCharacterPanel(InputAction.CallbackContext context) => TogglePanel(characterPanelUI);
    private void OnToggleQuestJournal(InputAction.CallbackContext context) => TogglePanel(questJournalUI);

    private void OnPause(InputAction.CallbackContext context)
    {
        if (UIManager.Instance != null && UIManager.Instance.TopPanel != null)
        {
            UIManager.Instance.CloseTopPanel();
        }
        else
        {
            TogglePanel(pauseMenuUI);
        }
    }

    private void TogglePanel(UIPanel panel)
    {
        if (UIManager.Instance == null || panel == null) return;

        if (panel.IsOpen) UIManager.Instance.CloseTopPanel();
        else UIManager.Instance.OpenPanel(panel);
    }

    // REMOVED: The HandleStateChanged method is no longer needed.

    private bool CanProcessGameplayInput()
    {
        // MODIFIED: Instead of a CanAct bool, we now check the type of the current state.
        // This is a more robust way to check if the player can perform actions.
        bool canAct = _stateManager.CurrentState is PlayerIdleState || _stateManager.CurrentState is PlayerMovingState;
        return !PauseMenuUI.isGamePaused && canAct;
    }
}