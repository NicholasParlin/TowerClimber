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
        // Listen for the inventory key.
        if (Input.GetKeyDown(inventoryKey))
        {
            // This would call a public Toggle() method on the UI script
            Debug.Log("Toggle Inventory");
        }

        // Listen for the character panel key.
        if (Input.GetKeyDown(characterPanelKey))
        {
            // This would call a public Toggle() method on the UI script
            Debug.Log("Toggle Character Panel");
        }

        // Listen for the pause menu key.
        if (Input.GetKeyDown(pauseMenuKey))
        {
            Debug.Log("Toggle Pause Menu");
        }
    }

    private void HandleActiveSkillInput()
    {
        // Ensure we have a reference to the skill manager before trying to use skills.
        if (playerSkillManager == null) return;

        foreach (SkillBinding binding in activeSkillBindings)
        {
            if (Input.GetKeyDown(binding.key))
            {
                // Tell the skill manager to try and use the skill from this binding.
                playerSkillManager.AttemptToUseSkill(binding.archetype, binding.skillIndex);
            }
        }
    }
}