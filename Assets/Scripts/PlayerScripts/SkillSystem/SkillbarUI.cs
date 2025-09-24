using UnityEngine;

// This script manages the main HUD element for the 9 assignable skill slots.
public class SkillbarUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("An array of the 9 SkillSlotUI components, in order from 1 to 9.")]
    [SerializeField] private SkillSlotUI[] skillSlots;

    private SkillbarManager _skillbarManager;

    private void Start()
    {
        _skillbarManager = SkillbarManager.Instance;
        if (_skillbarManager != null)
        {
            // Subscribe to the event to automatically update the UI when a slot changes.
            _skillbarManager.OnSkillSlotChanged += UpdateSlot;
        }

        // Initialize all slots at the start of the game.
        for (int i = 0; i < skillSlots.Length; i++)
        {
            UpdateSlot(i);
        }
    }

    private void OnDestroy()
    {
        if (_skillbarManager != null)
        {
            _skillbarManager.OnSkillSlotChanged -= UpdateSlot;
        }
    }

    /// <summary>
    /// Updates the visual appearance of a single slot on the skill bar.
    /// </summary>
    private void UpdateSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= skillSlots.Length) return;

        // Get the skill assigned to this slot from the manager.
        Skill assignedSkill = _skillbarManager.GetSkillInSlot(slotIndex);

        // Tell the UI element to display the skill (or clear itself if no skill is assigned).
        skillSlots[slotIndex].Display(assignedSkill);
    }
}