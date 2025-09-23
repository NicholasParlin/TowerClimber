using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CharacterPanelUI : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private TitleManager titleManager;

    [Header("Main Panel")]
    [SerializeField] private GameObject characterPanel;

    [Header("Stat Allocation")]
    [SerializeField] private Text unspentPointsText;
    [SerializeField] private Button strengthButton, dexterityButton, vitalityButton, intelligenceButton, wisdomButton, enduranceButton, senseButton;
    [SerializeField] private Text strengthText, dexterityText, vitalityText, intelligenceText, wisdomText, enduranceText, senseText;

    [Header("Skill Lists (Virtualized)")]
    [SerializeField] private VirtualizedScrollView activeSkillsScrollView;
    [SerializeField] private VirtualizedScrollView passiveSkillsScrollView;

    [Header("Title Dropdown")]
    [SerializeField] private Dropdown titleDropdown;
    [SerializeField] private Text titleDescriptionText;

    private void Start()
    {
        if (playerStats == null || playerSkillManager == null || titleManager == null)
        {
            Debug.LogError("One or more player references are not set on the CharacterPanelUI!");
            return;
        }

        playerStats.OnStatsUpdated += UpdateStatPanel;
        titleManager.OnEquippedTitleChanged += UpdateTitleDropdown;
        titleDropdown.onValueChanged.AddListener(OnTitleSelectionChanged);

        if (passiveSkillsScrollView != null)
        {
            passiveSkillsScrollView.OnItemCreated += OnPassiveSkillListingCreated;
        }

        characterPanel.SetActive(false);
        UpdateStatPanel();
    }

    private void OnDestroy()
    {
        if (playerStats != null) playerStats.OnStatsUpdated -= UpdateStatPanel;
        if (titleManager != null) titleManager.OnEquippedTitleChanged -= UpdateTitleDropdown;

        if (passiveSkillsScrollView != null)
        {
            passiveSkillsScrollView.OnItemCreated -= OnPassiveSkillListingCreated;
        }
    }

    public void Toggle()
    {
        characterPanel.SetActive(!characterPanel.activeSelf);
        if (characterPanel.activeSelf)
        {
            RefreshAllPanels();
        }
    }

    public void RefreshAllPanels()
    {
        UpdateStatPanel();
        UpdateSkillPanel();
        UpdateTitleDropdown();
    }

    private void UpdateStatPanel()
    {
        strengthText.text = $"Strength: {playerStats.Strength.Value}";
        dexterityText.text = $"Dexterity: {playerStats.Dexterity.Value}";
        vitalityText.text = $"Vitality: {playerStats.Vitality.Value}";
        intelligenceText.text = $"Intelligence: {playerStats.Intelligence.Value}";
        wisdomText.text = $"Wisdom: {playerStats.Wisdom.Value}";
        enduranceText.text = $"Endurance: {playerStats.Endurance.Value}";
        senseText.text = $"Sense: {playerStats.Sense.Value}";

        unspentPointsText.text = $"Unspent Points: {playerStats.unspentStatPoints}";
        bool hasPoints = playerStats.unspentStatPoints > 0;
        strengthButton.interactable = hasPoints;
        dexterityButton.interactable = hasPoints;
        vitalityButton.interactable = hasPoints;
        intelligenceButton.interactable = hasPoints;
        wisdomButton.interactable = hasPoints;
        enduranceButton.interactable = hasPoints;
        senseButton.interactable = hasPoints;
    }

    private void UpdateSkillPanel()
    {
        if (playerSkillManager.learnedSkills == null) return;

        List<object> activeSkillsData = new List<object>();
        List<object> passiveSkillsData = new List<object>();

        foreach (var skillList in playerSkillManager.learnedSkills.Values)
        {
            foreach (var skill in skillList)
            {
                if (skill.passiveEffectToApply != null)
                {
                    passiveSkillsData.Add(skill);
                }
                else
                {
                    activeSkillsData.Add(skill);
                }
            }
        }

        System.Action<GameObject, object> setupSkillUI = (uiObject, data) =>
        {
            SkillListingUI skillUI = uiObject.GetComponent<SkillListingUI>();
            Skill skillData = data as Skill;
            if (skillUI != null && skillData != null)
            {
                bool isPassiveActive = playerSkillManager.IsPassiveActive(skillData);
                skillUI.DisplaySkill(skillData, isPassiveActive);
            }
        };

        if (activeSkillsScrollView != null)
        {
            activeSkillsScrollView.Initialize(activeSkillsData, setupSkillUI);
        }
        if (passiveSkillsScrollView != null)
        {
            passiveSkillsScrollView.Initialize(passiveSkillsData, setupSkillUI);
        }
    }

    private void OnPassiveSkillListingCreated(GameObject itemObject)
    {
        SkillListingUI skillUI = itemObject.GetComponent<SkillListingUI>();
        if (skillUI != null)
        {
            skillUI.OnPassiveToggled += OnPassiveSkillToggled;
        }
    }

    private void OnPassiveSkillToggled(Skill skill)
    {
        if (playerSkillManager != null)
        {
            playerSkillManager.TogglePassive(skill);
            UpdateSkillPanel();
        }
    }

    private void UpdateTitleDropdown()
    {
        titleDropdown.onValueChanged.RemoveListener(OnTitleSelectionChanged);

        titleDropdown.ClearOptions();
        List<string> titleNames = titleManager.GetUnlockedTitles().Select(t => t.titleName).ToList();
        titleDropdown.AddOptions(titleNames);

        int equippedIndex = -1;
        for (int i = 0; i < titleManager.GetUnlockedTitles().Count; i++)
        {
            if (titleManager.IsTitleEquipped(titleManager.GetUnlockedTitles()[i]))
            {
                equippedIndex = i;
                break;
            }
        }

        if (equippedIndex != -1)
        {
            titleDropdown.SetValueWithoutNotify(equippedIndex);
        }

        UpdateTitleDescription();
        titleDropdown.onValueChanged.AddListener(OnTitleSelectionChanged);
    }

    private void OnTitleSelectionChanged(int index)
    {
        Title selectedTitle = titleManager.GetUnlockedTitles()[index];
        titleManager.EquipTitle(selectedTitle);
    }

    private void UpdateTitleDescription()
    {
        int currentIndex = titleDropdown.value;
        if (currentIndex >= 0 && currentIndex < titleManager.GetUnlockedTitles().Count)
        {
            titleDescriptionText.text = titleManager.GetUnlockedTitles()[currentIndex].description;
        }
        else
        {
            titleDescriptionText.text = "";
        }
    }

    public void AllocateStat(int statIndex)
    {
        playerStats.AllocateStatPoint((StatType)statIndex);
    }
}