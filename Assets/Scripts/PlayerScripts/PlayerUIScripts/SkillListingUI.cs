using UnityEngine;
using UnityEngine.UI;
// using TMPro;

// This script goes on the prefab for a single skill listing in the Character Panel.
public class SkillListingUI : MonoBehaviour
{
    [SerializeField] private Text skillNameText;
    [SerializeField] private Text skillDescriptionText;
    [SerializeField] private Button skillToggleButton; // Only used for passive skills

    private Skill _skill;
    private PlayerSkillManager _skillManager;

    // Setup for a simple active skill display
    public void SetupActive(Skill skill)
    {
        _skill = skill;
        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.description;
        skillToggleButton.gameObject.SetActive(false); // Hide the toggle button
    }

    // Setup for a passive skill with a toggle button
    public void SetupPassive(Skill skill, PlayerSkillManager skillManager)
    {
        _skill = skill;
        _skillManager = skillManager;
        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.description;

        skillToggleButton.gameObject.SetActive(true);
        skillToggleButton.onClick.AddListener(OnTogglePassive);

        // Update the button's appearance based on the skill's current state
        UpdateButtonVisuals();
    }

    private void OnTogglePassive()
    {
        if (_skillManager != null && _skill != null)
        {
            // The PlayerSkillManager would need a method to handle toggling passives
            // _skillManager.TogglePassive(_skill); 
            // UpdateButtonVisuals();
            Debug.Log("Toggling passive skill: " + _skill.skillName + " (Logic to be implemented in PlayerSkillManager)");
        }
    }

    private void UpdateButtonVisuals()
    {
        // The PlayerSkillManager would need a method to check if a passive is active
        // bool isActive = _skillManager.IsPassiveActive(_skill);
        // skillToggleButton.GetComponentInChildren<Text>().text = isActive ? "ON" : "OFF";
    }
}