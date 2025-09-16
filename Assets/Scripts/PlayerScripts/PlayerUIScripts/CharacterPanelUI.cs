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

    [Header("Title Dropdown")]
    [SerializeField] private Dropdown titleDropdown;
    [SerializeField] private Text titleDescriptionText;

    // Lists to keep track of the active UI objects from the pool.
    private List<GameObject> _activeSkillListingObjects = new List<GameObject>();
    private List<GameObject> _passiveSkillListingObjects = new List<GameObject>();

    private void Start()
    {
        if (playerStats == null || playerSkillManager == null || titleManager == null)
        {
            Debug.LogError("One or more player references are not set on the CharacterPanelUI!");
            return;
        }

        // Subscribe to events so the UI automatically refreshes when stats or titles change.
        playerStats.OnStatsUpdated += UpdateStatPanel;
        titleManager.OnEquippedTitleChanged += UpdateTitleDropdown;

        // Add a listener for when the user changes the dropdown selection.
        titleDropdown.onValueChanged.AddListener(OnTitleSelectionChanged);

        characterPanel.SetActive(false);
        UpdateStatPanel();
    }

    private void OnDestroy()
    {
        if (playerStats != null) playerStats.OnStatsUpdated -= UpdateStatPanel;
        if (titleManager != null) titleManager.OnEquippedTitleChanged -= UpdateTitleDropdown;
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
        // Return all existing UI objects to the pool.
        foreach (var skillObject in _activeSkillListingObjects) { ObjectPooler.Instance.ReturnToPool("SkillListing", skillObject); }
        foreach (var skillObject in _passiveSkillListingObjects) { ObjectPooler.Instance.ReturnToPool("SkillListing", skillObject); }
        _activeSkillListingObjects.Clear();
        _passiveSkillListingObjects.Clear();

        if (playerSkillManager.learnedSkills == null) return;

        // Get new UI objects from the pool for each learned skill.
        foreach (var skillList in playerSkillManager.learnedSkills.Values)
        {
            foreach (var skill in skillList)
            {
                GameObject skillGO = ObjectPooler.Instance.GetFromPool("SkillListing", Vector3.zero, Quaternion.identity);
                if (skillGO == null) continue;

                var skillUI = skillGO.GetComponent<SkillListingUI>();

                if (skill.isPassive)
                {
                    skillGO.transform.SetParent(passiveSkillsContentArea, false);
                    skillUI.SetupPassive(skill, playerSkillManager);
                    _passiveSkillListingObjects.Add(skillGO);
                }
                else
                {
                    skillGO.transform.SetParent(activeSkillsContentArea, false);
                    skillUI.SetupActive(skill);
                    _activeSkillListingObjects.Add(skillGO);
                }
            }
        }
    }

    private void UpdateTitleDropdown()
    {
        titleDropdown.onValueChanged.RemoveListener(OnTitleSelectionChanged); // Prevent firing event during refresh

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

        titleDropdown.onValueChanged.AddListener(OnTitleSelectionChanged); // Re-add listener
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