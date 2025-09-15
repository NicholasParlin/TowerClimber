using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Slider and Text
// using TMPro; // Uncomment if you use TextMeshPro

// This script manages the player's Heads-Up Display (Health, Mana, XP bars, etc.).
// It should be attached to a parent UI Canvas object.
public class PlayerHUD : MonoBehaviour
{
    [Header("Player Reference")]
    [Tooltip("Assign the GameObject that has the PlayerStats component.")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Health Bar")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText; // Or TextMeshProUGUI

    [Header("Mana Bar")]
    [SerializeField] private Slider manaSlider;
    [SerializeField] private Text manaText; // Or TextMeshProUGUI

    [Header("Energy Bar")]
    [SerializeField] private Slider energySlider;
    [SerializeField] private Text energyText; // Or TextMeshProUGUI

    [Header("Anguish Bar")]
    [SerializeField] private Slider anguishSlider;
    [SerializeField] private Text anguishText; // Or TextMeshProUGUI

    [Header("Experience Bar")]
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private Text levelText; // Or TextMeshProUGUI

    private void OnEnable()
    {
        // Subscribe to the OnStatsUpdated event. This is the core of our efficient,
        // event-driven UI. The UpdateHUD method will ONLY be called when stats change.
        if (playerStats != null)
        {
            playerStats.OnStatsUpdated += UpdateHUD;
            // Also call it once at the start to initialize the UI with the player's starting stats.
            UpdateHUD();
        }
        else
        {
            Debug.LogError("PlayerStats reference is not set on the PlayerHUD!");
        }
    }

    private void OnDisable()
    {
        // Always unsubscribe from events when this object is disabled to prevent errors.
        if (playerStats != null)
        {
            playerStats.OnStatsUpdated -= UpdateHUD;
        }
    }

    /// <summary>
    /// This method is called by the OnStatsUpdated event from PlayerStats.
    /// It updates all the visual elements of the HUD.
    /// </summary>
    private void UpdateHUD()
    {
        if (playerStats == null) return;

        // --- Update Health Bar ---
        if (healthSlider != null)
        {
            // The slider's value is a ratio of current health to max health (0 to 1).
            healthSlider.maxValue = playerStats.maxHealth;
            healthSlider.value = playerStats.currentHealth;
        }
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(playerStats.currentHealth)} / {Mathf.CeilToInt(playerStats.maxHealth)}";
        }

        // --- Update Mana Bar ---
        if (manaSlider != null)
        {
            manaSlider.maxValue = playerStats.maxMana;
            manaSlider.value = playerStats.currentMana;
        }
        if (manaText != null)
        {
            manaText.text = $"{Mathf.CeilToInt(playerStats.currentMana)} / {Mathf.CeilToInt(playerStats.maxMana)}";
        }

        // --- Update Energy Bar ---
        if (energySlider != null)
        {
            energySlider.maxValue = playerStats.maxEnergy;
            energySlider.value = playerStats.currentEnergy;
        }
        if (energyText != null)
        {
            energyText.text = $"{Mathf.CeilToInt(playerStats.currentEnergy)} / {Mathf.CeilToInt(playerStats.maxEnergy)}";
        }

        // --- Update Anguish Bar ---
        if (anguishSlider != null)
        {
            anguishSlider.maxValue = playerStats.maxAnguish;
            anguishSlider.value = playerStats.currentAnguish;
        }
        if (anguishText != null)
        {
            anguishText.text = $"{Mathf.CeilToInt(playerStats.currentAnguish)} / {Mathf.CeilToInt(playerStats.maxAnguish)}";
        }

        // --- Update Experience Bar & Level ---
        if (experienceSlider != null)
        {
            experienceSlider.maxValue = playerStats.experienceToNextLevel;
            experienceSlider.value = playerStats.currentExperience;
        }
        if (levelText != null)
        {
            levelText.text = $"Level {playerStats.currentLevel}";
        }
    }
}