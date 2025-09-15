using UnityEngine;
using UnityEngine.UI;
// using TMPro; // Uncomment if you use TextMeshPro

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

    [Header("Skill & Title Lists")]
    [Tooltip("The parent object where active skill prefabs will be instantiated.")]
    [SerializeField] private Transform activeSkillsContentArea;
    [Tooltip("The parent object where passive skill prefabs will be instantiated.")]
    [SerializeField] private Transform passiveSkillsContentArea;
    [Tooltip("The parent object where title prefabs will be instantiated.")]
    [SerializeField] private Transform titlesContentArea;
    [SerializeField] private GameObject skillListPrefab; // A prefab for displaying a skill
    [SerializeField] private GameObject titleListPrefab; // A prefab for displaying a title

    private void Start()
    {
        // Ensure all references are set
        if (playerStats == null || playerSkillManager == null || titleManager == null)
        {
            Debug.LogError("One or more player references are not set on the CharacterPanelUI!");
            return;
        }

        // Subscribe to the event so the UI automatically refreshes when stats change.
        playerStats.OnStatsUpdated += UpdateStatPanel;

        // Initialize the panel state.
        characterPanel.SetActive(false);
        UpdateStatPanel();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsUpdated -= UpdateStatPanel;
        }
    }

    private void Update()
    {
        // Simple toggle for opening/closing the character panel (e.g., with the 'C' key).
        if (Input.GetKeyDown(KeyCode.C))
        {
            characterPanel.SetActive(!characterPanel.activeSelf);
            if (characterPanel.activeSelf)
            {
                RefreshAllPanels();
            }
        }
    }

    /// <summary>
    /// Refreshes all information displayed on the character panel.
    /// </summary>
    public void RefreshAllPanels()
    {
        UpdateStatPanel();
        UpdateSkillPanel();
        UpdateTitlePanel();
    }

    /// <summary>
    /// Updates the stat display and the visibility of allocation buttons.
    /// </summary>
    private void UpdateStatPanel()
    {
        // Update stat texts
        strengthText.text = $"Strength: {playerStats.Strength.Value}";
        dexterityText.text = $"Dexterity: {playerStats.Dexterity.Value}";
        vitalityText.text = $"Vitality: {playerStats.Vitality.Value}";
        intelligenceText.text = $"Intelligence: {playerStats.Intelligence.Value}";
        wisdomText.text = $"Wisdom: {playerStats.Wisdom.Value}";
        enduranceText.text = $"Endurance: {playerStats.Endurance.Value}";
        senseText.text = $"Sense: {playerStats.Sense.Value}";

        // Update unspent points display and button interactivity
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

    /// <summary>
    /// Clears and repopulates the skill lists.
    /// </summary>
    private void UpdateSkillPanel()
    {
        // Clear existing lists
        foreach (Transform child in activeSkillsContentArea) Destroy(child.gameObject);
        foreach (Transform child in passiveSkillsContentArea) Destroy(child.gameObject);

        // Populate lists
        foreach (var skillList in playerSkillManager.learnedSkills.Values)
        {
            foreach (var skill in skillList)
            {
                // Instantiate the prefab
                GameObject skillGO = Instantiate(skillListPrefab);
                var skillUI = skillGO.GetComponent<SkillListingUI>();

                // Check if the skill is passive to place it in the correct list
                if (skill.isPassive)
                {
                    skillGO.transform.SetParent(passiveSkillsContentArea, false);
                    // Pass a reference to the PlayerSkillManager to handle toggling
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
    /// Clears and repopulates the title list.
    /// </summary>
    private void UpdateTitlePanel()
    {
        // Clear existing list
        foreach (Transform child in titlesContentArea) Destroy(child.gameObject);

        // Populate list
        foreach (var title in titleManager.GetUnlockedTitles())
        {
            GameObject titleGO = Instantiate(titleListPrefab, titlesContentArea, false);
            var titleUI = titleGO.GetComponent<TitleListingUI>();
            // Pass a reference to the TitleManager to handle equipping
            titleUI.Setup(title, titleManager);
        }
    }

    /// <summary>
    /// Public method to be called by the stat allocation buttons in the UI.
    /// </summary>
    /// <param name="statIndex">0=Str, 1=Dex, 2=Vit, 3=Int, 4=Wis, 5=End, 6=Sen</param>
    public void AllocateStat(int statIndex)
    {
        playerStats.AllocateStatPoint((StatType)statIndex);
    }
}