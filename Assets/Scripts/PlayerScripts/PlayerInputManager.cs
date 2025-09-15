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

    [Header("UI Key Bindings")]
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;
    [SerializeField] private KeyCode characterPanelKey = KeyCode.C;
    [SerializeField] private KeyCode pauseMenuKey = KeyCode.Escape;

    [Header("Active Skill Bindings")]
    [Tooltip("Set up which keys correspond to which active skills here.")]
    [SerializeField] private List<SkillBinding> activeSkillBindings = new List<SkillBinding>();

    // A helper class to make binding keys to skills easy in the Inspector.
    [System.Serializable]
    private class SkillBinding
    {
        public KeyCode key;
        public Archetype archetype;
        [Tooltip("The index of the skill in the archetype list (0 = first skill, 1 = second, etc.)")]
        public int skillIndex;
    }

    private void Update()
    {
        HandleUIPanelInput();
        HandleActiveSkillInput();
    }

    private void HandleUIPanelInput()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            // --- TODO RESOLVED ---
            if (inventoryUI != null)
            {
                inventoryUI.Toggle();
            }
        }

        if (Input.GetKeyDown(characterPanelKey))
        {
            // --- TODO RESOLVED ---
            if (characterPanelUI != null)
            {
                characterPanelUI.Toggle();
            }
        }

        if (Input.GetKeyDown(pauseMenuKey))
        {
            // This is where you would call your Pause Menu manager.
            Debug.Log("Toggle Pause Menu");
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