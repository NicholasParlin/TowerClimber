using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// This script manages the UI panel for assigning skills to the skill bar.
public class SkillAssignmentPanel : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Virtualized Scroll View for the list of all learned skills.")]
    [SerializeField] private VirtualizedScrollView allSkillsScrollView;
    [Tooltip("An array of the 9 assignment buttons, in order from 1 to 9.")]
    [SerializeField] private Button[] assignmentButtons;

    // We will need a data adapter to display the skills in the list.
    private SkillDataAdapter _skillAdapter;
    private PlayerSkillManager _playerSkillManager;
    private SkillbarManager _skillbarManager;

    // This will hold the skill the player has clicked on, ready to be assigned.
    private Skill _selectedSkill;

    private void Start()
    {
        _playerSkillManager = PlayerStats.Instance.GetComponent<PlayerSkillManager>();
        _skillbarManager = SkillbarManager.Instance;
        _skillAdapter = GetComponent<SkillDataAdapter>(); // Assumes the adapter is on the same object.

        // Add a listener to each of the 9 assignment buttons.
        for (int i = 0; i < assignmentButtons.Length; i++)
        {
            int slotIndex = i; // Important for the closure
            assignmentButtons[i].onClick.AddListener(() => OnAssignmentButtonClicked(slotIndex));
        }

        // We will also need to handle selecting a skill from the list.
        // This will require an event from the SkillListingUI, which we can add next.
    }

    /// <summary>
    /// This method is called when the player clicks one of the 9 assignment buttons.
    /// </summary>
    private void OnAssignmentButtonClicked(int slotIndex)
    {
        if (_selectedSkill != null)
        {
            _skillbarManager.AssignSkillToSlot(_selectedSkill, slotIndex);
            Debug.Log($"Assigned {_selectedSkill.skillName} to slot {slotIndex + 1}.");
            // Deselect the skill after assigning it.
            _selectedSkill = null;
        }
    }

    /// <summary>
    /// This method will be called by a UI event when a skill is clicked in the list.
    /// </summary>
    public void SelectSkill(Skill skill)
    {
        _selectedSkill = skill;
        Debug.Log($"Selected skill: {skill.skillName}");
    }

    /// <summary>
    // Populates the scroll view with all the skills the player has learned.
    /// </summary>
    public void RefreshSkillList()
    {
        if (_playerSkillManager.learnedSkills == null) return;

        List<object> allSkills = new List<object>();
        foreach (var skillList in _playerSkillManager.learnedSkills.Values)
        {
            allSkills.AddRange(skillList.Cast<object>());
        }

        allSkillsScrollView.Initialize(allSkills, _skillAdapter);
    }
}