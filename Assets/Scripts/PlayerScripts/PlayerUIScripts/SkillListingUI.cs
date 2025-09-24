using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkillListingUI : MonoBehaviour
{
    public event Action<Skill> OnPassiveToggled;
    public event Action<Skill> OnSkillSelected; // NEW: Event for selecting a skill

    [Header("UI References")]
    [SerializeField] private Text skillNameText;
    [SerializeField] private Text skillDescriptionText;
    [SerializeField] private Button skillToggleButton;
    [SerializeField] private Text skillToggleButtonText;

    private Skill _skill;
    private bool _isPassiveActive;
    private Button _selectButton;

    private void Awake()
    {
        _selectButton = GetComponent<Button>();
        _selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

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
        _isPassiveActive = !_isPassiveActive;
        UpdateButtonVisuals();
        OnPassiveToggled?.Invoke(_skill);
    }

    private void OnSelectButtonClicked()
    {
        // When this UI element is clicked, broadcast the skill that was selected.
        OnSkillSelected?.Invoke(_skill);
    }

    private void UpdateButtonVisuals()
    {
        if (_skill != null && _skill.passiveEffectToApply != null)
        {
            skillToggleButtonText.text = _isPassiveActive ? "ACTIVE" : "INACTIVE";
        }
    }
}