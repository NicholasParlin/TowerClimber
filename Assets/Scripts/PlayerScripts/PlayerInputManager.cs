using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterStateManager))]
public class PlayerInputManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private PlayerInteraction playerInteraction;

    [Header("UI Panel References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private CharacterPanelUI characterPanelUI;
    [SerializeField] private QuestJournalUI questJournalUI;
    [SerializeField] private PauseMenuUI pauseMenuUI;

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

        // MODIFIED: The pause key logic is now handled within HandleUIPanelInput.

        if (PauseMenuUI.isGamePaused || !_canAct)
        {
            return;
        }

        HandleInteractionInput();
        HandleActiveSkillInput();
    }

    private void HandleUIPanelInput()
    {
        if (UIManager.Instance == null) return;

        if (Input.GetKeyDown(inventoryKey))
        {
            TogglePanel(inventoryUI);
        }

        if (Input.GetKeyDown(characterPanelKey))
        {
            TogglePanel(characterPanelUI);
        }

        if (Input.GetKeyDown(questJournalKey))
        {
            TogglePanel(questJournalUI);
        }

        // MODIFIED: Escape key now has more intelligent logic.
        if (Input.GetKeyDown(pauseMenuKey))
        {
            // If any panel is currently open, the Escape key's first job is to close it.
            if (UIManager.Instance.TopPanel != null)
            {
                UIManager.Instance.CloseTopPanel();
            }
            else // If no other panels are open, then toggle the pause menu.
            {
                TogglePanel(pauseMenuUI);
            }
        }
    }

    // A helper method to simplify toggling panels.
    private void TogglePanel(UIPanel panel)
    {
        if (panel == null) return;

        var topPanel = UIManager.Instance.TopPanel;

        if (topPanel == panel)
        {
            UIManager.Instance.CloseTopPanel();
        }
        else
        {
            UIManager.Instance.OpenPanel(panel);
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