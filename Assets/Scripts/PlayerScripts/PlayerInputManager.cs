using UnityEngine;
using System.Collections.Generic;

// This script is the central hub for ALL player inputs that are not directly related to movement.
public class PlayerInputManager : MonoBehaviour
{
    [Header("Component References")]
    [Tooltip("Assign the GameObject with the PlayerSkillManager component.")]
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [Tooltip("Assign the GameObject with the InventoryUI component.")]
    [SerializeField] private InventoryUI inventoryUI;
    [Tooltip("Assign the GameObject with the CharacterPanelUI component.")]
    [SerializeField] private CharacterPanelUI characterPanelUI;
    [Tooltip("Assign the GameObject with the PauseMenuUI component.")]
    [SerializeField] private PauseMenuUI pauseMenuUI;
    [Tooltip("Assign the GameObject with the PlayerInteraction component.")]
    [SerializeField] private PlayerInteraction playerInteraction; // New reference

    [Header("UI & Interaction Key Bindings")]
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;
    [SerializeField] private KeyCode characterPanelKey = KeyCode.C;
    [SerializeField] private KeyCode pauseMenuKey = KeyCode.Escape;
    [SerializeField] private KeyCode interactKey = KeyCode.E; // New key binding

    [Header("Active Skill Bindings")]
    [Tooltip("Set up which keys correspond to which active skills here.")]
    [SerializeField] private List<SkillBinding> activeSkillBindings = new List<SkillBinding>();

    // A helper class to make binding keys to skills easy in the Inspector.
    [System.Serializable]
    private class SkillBinding
    {
        public KeyCode key;
        public Archetype archetype;
        public int skillIndex;
    }

    private void Update()
    {
        // The pause key should always be checked, even when the game is paused.
        if (Input.GetKeyDown(pauseMenuKey))
        {
            if (pauseMenuUI != null) { pauseMenuUI.TogglePauseMenu(); }
        }

        // Don't process any other game input if the game is paused.
        if (PauseMenuUI.isGamePaused)
        {
            return;
        }

        HandleUIPanelInput();
        HandleInteractionInput(); // New input handler
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

    // This method handles the interaction key press.
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
}