using UnityEngine;
using UnityEngine.UI; // For UI elements like Text
using System.Text; // For the StringBuilder

// This script is attached to the root of an enemy's UI canvas (e.g., a health bar).
// It controls what information is displayed based on the player's Sense stat.
public class EnemyInfoDisplay : MonoBehaviour
{
    [Header("Component References")]
    [Tooltip("Reference to the enemy's own stats component.")]
    [SerializeField] private CharacterStatsBase enemyStats;
    [Tooltip("Reference to the enemy's loot drop component.")]
    [SerializeField] private EnemyLootDrop enemyLootDrop;

    [Header("UI Panel References")]
    [Tooltip("The parent object for the entire display.")]
    [SerializeField] private GameObject displayPanel;
    [Tooltip("The UI Slider for the health bar.")]
    [SerializeField] private Slider healthSlider;
    [Tooltip("The UI Slider for the mana bar.")]
    [SerializeField] private Slider manaSlider;
    [Tooltip("A Text object to show primary stats.")]
    [SerializeField] private Text statsText;
    [Tooltip("A Text object to show resistances.")]
    [SerializeField] private Text resistancesText;
    [Tooltip("A parent object containing UI elements to show potential loot.")]
    [SerializeField] private GameObject lootPanel;

    private void Awake()
    {
        // Start with the display hidden.
        if (displayPanel != null)
        {
            displayPanel.SetActive(false);
        }
    }

    /// <summary>
    /// This is called by the PlayerTargeting script when the player looks at this enemy.
    /// </summary>
    public void ShowDisplay(PlayerStats playerStats)
    {
        if (enemyStats == null || playerStats == null) return;

        displayPanel.SetActive(true);
        UpdateDisplayContent(playerStats.Sense.Value, enemyStats.Intelligence.Value);
    }

    /// <summary>
    /// This is called by the PlayerTargeting script when the player looks away.
    /// </summary>
    public void HideDisplay()
    {
        displayPanel.SetActive(false);
    }

    /// <summary>
    /// The core logic that decides which UI elements to show based on the Sense vs. Intelligence check.
    /// </summary>
    private void UpdateDisplayContent(float playerSense, float enemyIntelligence)
    {
        // Start by hiding everything, then reveal based on the tiers.
        healthSlider.gameObject.SetActive(false);
        manaSlider.gameObject.SetActive(false);
        statsText.gameObject.SetActive(false);
        resistancesText.gameObject.SetActive(false);
        lootPanel.SetActive(false);

        // Tier 1: Sense >= Intelligence (reveals Health and Mana)
        if (playerSense >= enemyIntelligence)
        {
            healthSlider.gameObject.SetActive(true);
            healthSlider.maxValue = enemyStats.maxHealth;
            healthSlider.value = enemyStats.currentHealth;

            manaSlider.gameObject.SetActive(true);
            manaSlider.maxValue = enemyStats.maxMana;
            manaSlider.value = enemyStats.currentMana;
        }

        // Tier 2: Sense >= 1.25x Intelligence (reveals Primary Stats)
        if (playerSense >= enemyIntelligence * 1.25f)
        {
            statsText.gameObject.SetActive(true);
            // Use a StringBuilder for efficient text creation.
            StringBuilder statsBuilder = new StringBuilder();
            statsBuilder.AppendLine($"STR: {enemyStats.Strength.Value} | DEX: {enemyStats.Dexterity.Value}");
            statsBuilder.AppendLine($"INT: {enemyStats.Intelligence.Value} | SNS: {enemyStats.Sense.Value}");
            statsText.text = statsBuilder.ToString();
        }

        // Tier 3: Sense >= 1.5x Intelligence (reveals Resistances)
        if (playerSense >= enemyIntelligence * 1.5f)
        {
            resistancesText.gameObject.SetActive(true);
            StringBuilder resBuilder = new StringBuilder();
            resBuilder.AppendLine($"ARM: {enemyStats.Armor.Value} | MR: {enemyStats.MagicResistance.Value}");
            // Here you could add logic to show weaknesses (negative resistances) in a different color.
            resistancesText.text = resBuilder.ToString();
        }

        // Tier 4: Sense >= 2x Intelligence (reveals Potential Loot)
        if (playerSense >= enemyIntelligence * 2f)
        {
            lootPanel.SetActive(true);
            // Here you would have logic to populate the lootPanel with item icons
            // from the enemyLootDrop.PotentialLoot loot table.
        }
    }
}