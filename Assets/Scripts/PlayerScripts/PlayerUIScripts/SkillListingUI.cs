using UnityEngine;
using UnityEngine.UI;
// using TMPro; // Uncomment and change Text to TextMeshProUGUI if you use TextMeshPro

// This script goes on the prefab for a single skill listing in the Character Panel.
public class SkillListingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text skillNameText;
    [SerializeField] private Text skillDescriptionText;
    [SerializeField] private Button skillToggleButton; // Only used for passive skills
    [SerializeField] private Text skillToggleButtonText;

    private Skill _skill;
    private PlayerSkillManager _skillManager;

    // Setup for a simple active skill display
    public void SetupActive(Skill skill)
    {
        _skill = skill;
        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.description;
        skillToggleButton.gameObject.SetActive(false); // Hide the toggle button for active skills
    }

    // Setup for a passive skill with a toggle button
    public void SetupPassive(Skill skill, PlayerSkillManager skillManager)
    {
        _skill = skill;
        _skillManager = skillManager;
        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.description;

        skillToggleButton.gameObject.SetActive(true);
        // Ensure we don't add multiple listeners if the UI is refreshed
        skillToggleButton.onClick.RemoveAllListeners();
        skillToggleButton.onClick.AddListener(OnTogglePassive);

        // Update the button's appearance based on the skill's current state
        UpdateButtonVisuals();
    }

    /// <summary>
    /// Called when the toggle button for a passive skill is clicked.
    /// </summary>
    private void OnTogglePassive()
    {
        if (_skillManager != null && _skill != null)
        {
            // Call the public method on the skill manager to toggle the passive.
            _skillManager.TogglePassive(_skill);
            // Update this button's visuals to reflect the new state.
            UpdateButtonVisuals();
        }
    }

    /// <summary>
    /// Updates the text of the toggle button to show if the passive is ON or OFF.
    /// </summary>
    private void UpdateButtonVisuals()
    {
        if (_skillManager == null || _skill == null) return;

        // Ask the skill manager if this specific passive is currently active.
        bool isActive = _skillManager.IsPassiveActive(_skill);
        skillToggleButtonText.text = isActive ? "ACTIVE" : "INACTIVE";
        // You could also change the button's color here based on the 'isActive' state.
    }
}