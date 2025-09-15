using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
// using TMPro; // Uncomment if you use TextMeshPro and change Dropdown/Text references

// This is the master script for the main character panel, which shows stats, skills, and titles.
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

    [Header("Skill Lists (Assign Scroll View Content)")]
    [Tooltip("The parent object where active skill prefabs will be instantiated.")]
    [SerializeField] private Transform activeSkillsContentArea;
    [Tooltip("The parent object where passive skill prefabs will be instantiated.")]
    [SerializeField] private Transform passiveSkillsContentArea;
    [SerializeField] private GameObject skillListPrefab;

    [Header("Title Dropdown")]
    [SerializeField] private Dropdown titleDropdown; // NEW: Reference to a Dropdown component
    [SerializeField] private Text titleDescriptionText; // NEW: Text to show the selected title's description

    private void Start()
    {
        if (playerStats == null || playerSkillManager == null || titleManager == null)
        {
            Debug.LogError("One or more player references are not set on the CharacterPanelUI!");
            return;
        }

        // Subscribe to events so the UI automatically refreshes when stats or titles change.
        playerStats.OnStatsUpdated += UpdateStatPanel;
        titleManager.OnEquippedTitleChanged += UpdateTitleDescription;

        // Add a listener for when the user changes the dropdown selection.
        titleDropdown.onValueChanged.AddListener(OnTitleSelectionChanged);

        characterPanel.SetActive(false);
        UpdateStatPanel();
    }

    private void OnDestroy()
    {
        if (playerStats != null) playerStats.OnStatsUpdated -= UpdateStatPanel;
        if (titleManager != null) titleManager.OnEquippedTitleChanged -= UpdateTitleDescription;
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
        foreach (Transform child in activeSkillsContentArea) Destroy(child.gameObject);
        foreach (Transform child in passiveSkillsContentArea) Destroy(child.gameObject);

        if (playerSkillManager.learnedSkills == null) return;
        foreach (var skillList in playerSkillManager.learnedSkills.Values)
        {
            foreach (var skill in skillList)
            {
                GameObject skillGO = Instantiate(skillListPrefab);
                var skillUI = skillGO.GetComponent<SkillListingUI>();

                if (skill.isPassive)
                {
                    skillGO.transform.SetParent(passiveSkillsContentArea, false);
                    skillUI.SetupPassive(skill, playerSkillManager);
                }
                else
                {
                    skillGO.transform.SetParent(activeSkillsContentArea, false);
                    skillUI.SetupActive(skill);
                }
            }
        }
    }

    /// <summary>
    /// NEW: Rewritten method to populate the Dropdown instead of a list.
    /// </summary>
    private void UpdateTitleDropdown()
    {
        titleDropdown.ClearOptions();

        List<string> titleNames = titleManager.GetUnlockedTitles().Select(t => t.titleName).ToList();
        titleDropdown.AddOptions(titleNames);

        // Find the index of the currently equipped title to set the dropdown's value.
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
            // Set the value without triggering the onValueChanged event.
            titleDropdown.SetValueWithoutNotify(equippedIndex);
        }

        UpdateTitleDescription();
    }

    /// <summary>
    /// NEW: Called when the player selects a new title from the dropdown.
    /// </summary>
    private void OnTitleSelectionChanged(int index)
    {
        Title selectedTitle = titleManager.GetUnlockedTitles()[index];
        titleManager.EquipTitle(selectedTitle);
    }

    /// <summary>
    /// NEW: Updates the description text based on the currently selected title.
    /// </summary>
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