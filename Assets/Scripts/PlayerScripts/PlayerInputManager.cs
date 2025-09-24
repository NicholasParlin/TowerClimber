using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterStateManager))] // NEW: Now requires the state manager
public class PlayerInputManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private CharacterPanelUI characterPanelUI;
    [SerializeField] private PauseMenuUI pauseMenuUI;
    [SerializeField] private PlayerInteraction playerInteraction;

    // NEW: Reference to the state manager
    private CharacterStateManager _stateManager;

    [Header("UI & Interaction Key Bindings")]
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;
    [SerializeField] private KeyCode characterPanelKey = KeyCode.C;
    [SerializeField] private KeyCode pauseMenuKey = KeyCode.Escape;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Active Skill Bindings")]
    [SerializeField] private List<SkillBinding> activeSkillBindings = new List<SkillBinding>();

    // State variable to control actions
    private bool _canAct = true;

    [System.Serializable]
    private class SkillBinding
    {
        public KeyCode key;
        public Archetype archetype;
        public int skillIndex;
    }

    private void Awake()
    {
        // Get the state manager component.
        _stateManager = GetComponent<CharacterStateManager>();
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
        // UI inputs are always checked, regardless of the player's state.
        HandleUIPanelInput();

        // The pause key should also always be checked.
        if (Input.GetKeyDown(pauseMenuKey))
        {
            if (pauseMenuUI != null) { pauseMenuUI.TogglePauseMenu(); }
        }

        // Don't process any game actions if the game is paused or the player can't act.
        if (PauseMenuUI.isGamePaused || !_canAct)
        {
            return;
        }

        // Action inputs are only checked if the player is in a state that allows them to act.
        HandleInteractionInput();
        HandleActiveSkillInput();
    }

    private void HandleUIPanelInput()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            if (inventoryUI != null) { inventoryUI.Toggle(); }
        }

        if (Input.GetKeyDown(characterPanelKey))
        {
            if (characterPanelUI != null) { characterPanelUI.Toggle(); }
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (playerInteraction != null)
            {
                playerInteraction.TryInteract();
            }
        }
    }

    private void HandleActiveSkillInput()
    {
        if (playerSkillManager == null) return;
        foreach (SkillBinding binding in activeSkillBindings)
        {
            if (Input.GetKeyDown(binding.key))
            {
                playerSkillManager.AttemptToUseSkill(binding.archetype, binding.skillIndex);
            }
        }
    }

    /// <summary>
    /// This method is called by the OnStateChanged event from the state manager.
    /// </summary>
    private void HandleStateChanged(CharacterState newState)
    {
        // Use the CanAct property from the state manager to determine if action keys should be enabled.
        _canAct = _stateManager.CanAct;
    }
}