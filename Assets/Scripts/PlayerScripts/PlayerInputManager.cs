using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterStateManager))]
public class PlayerInputManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerSkillManager playerSkillManager;
    // MODIFIED: The direct reference to PauseMenuUI is no longer needed here.
    // [SerializeField] private PauseMenuUI pauseMenuUI; 
    [SerializeField] private PlayerInteraction playerInteraction;

    private CharacterStateManager _stateManager;

    [Header("UI & Interaction Key Bindings")]
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;
    [SerializeField] private KeyCode characterPanelKey = KeyCode.C;
    [SerializeField] private KeyCode questJournalKey = KeyCode.J;
    [SerializeField] private KeyCode pauseMenuKey = KeyCode.Escape;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Active Skill Bindings")]
    [SerializeField] private List<SkillBinding> activeSkillBindings = new List<SkillBinding>();

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
        _stateManager = GetComponent<CharacterStateManager>();
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
        HandleUIPanelInput();

        // MODIFIED: Pause key now goes through the UIManager as well.
        if (Input.GetKeyDown(pauseMenuKey))
        {
            var pauseMenuPanel = UIManager.Instance.GetComponentInChildren<PauseMenuUI>(true);
            if (pauseMenuPanel != null) UIManager.Instance.TogglePanel(pauseMenuPanel);
        }

        if (PauseMenuUI.isGamePaused || !_canAct)
        {
            return;
        }

        HandleInteractionInput();
        HandleActiveSkillInput();
    }

    private void HandleUIPanelInput()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            var inventoryPanel = UIManager.Instance.GetComponentInChildren<InventoryUI>(true);
            if (inventoryPanel != null) UIManager.Instance.TogglePanel(inventoryPanel);
        }

        if (Input.GetKeyDown(characterPanelKey))
        {
            var characterPanel = UIManager.Instance.GetComponentInChildren<CharacterPanelUI>(true);
            if (characterPanel != null) UIManager.Instance.TogglePanel(characterPanel);
        }

        if (Input.GetKeyDown(questJournalKey))
        {
            var questJournalPanel = UIManager.Instance.GetComponentInChildren<QuestJournalUI>(true);
            if (questJournalPanel != null) UIManager.Instance.TogglePanel(questJournalPanel);
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

    private void HandleStateChanged(CharacterState newState)
    {
        _canAct = _stateManager.CanAct;
    }
}