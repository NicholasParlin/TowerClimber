using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillListingUI : MonoBehaviour
{
    public event Action<Skill> OnPassiveToggled;

    [SerializeField] private Text skillNameText;
    [SerializeField] private Text skillDescriptionText;
    [SerializeField] private Button skillToggleButton;
    [SerializeField] private Text skillToggleButtonText;

    private Skill _skill;
    private bool _isPassiveActive;

    public void DisplaySkill(Skill skill, bool isPassiveActive)
    {
        _skill = skill;
        _isPassiveActive = isPassiveActive;
        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.description;

        bool isPassive = skill.passiveEffectToApply != null;
        skillToggleButton.gameObject.SetActive(isPassive);

        if (isPassive)
        {
            skillToggleButton.onClick.RemoveAllListeners();
            skillToggleButton.onClick.AddListener(OnTogglePassive);
            UpdateButtonVisuals();
        }
    }

    private void OnTogglePassive()
    {
        // Invert the active state for immediate visual feedback
        _isPassiveActive = !_isPassiveActive;
        UpdateButtonVisuals();
        // Broadcast the event for the main panel to handle the logic
        OnPassiveToggled?.Invoke(_skill);
    }

    private void UpdateButtonVisuals()
    {
        if (_skill != null && _skill.passiveEffectToApply != null)
        {
            skillToggleButtonText.text = _isPassiveActive ? "ACTIVE" : "INACTIVE";
        }
    }
}