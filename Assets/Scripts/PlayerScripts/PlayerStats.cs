using System;
using UnityEngine;

// This component is specific to the player and handles their stats, resources, progression, and save/load functionality.
public class PlayerStats : CharacterStatsBase
{
    [Header("Player Progression")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;
    public int unspentStatPoints = 0;

    // Events for UI updates and game systems
    public event Action OnCoreStatChanged; // For systems like GameManager that need to know about Sense
    public event Action OnStatsUpdated;    // A general event for UI to refresh everything

    #region Save System Integration

    // This is the data structure that will be saved to a file.
    [System.Serializable]
    public class SaveData
    {
        public int level;
        public int experience;
        public int unspentStatPoints;

        // We only need to save the base values, as modifiers are temporary.
        public int strengthBase;
        public int dexterityBase;
        public int vitalityBase;
        public int intelligenceBase;
        public int wisdomBase;
        public int enduranceBase;
        public int senseBase;

        public SaveData(PlayerStats stats)
        {
            level = stats.level;
            experience = stats.experience;
            unspentStatPoints = stats.unspentStatPoints;

            // --- FIX APPLIED HERE ---
            // Explicitly cast the float BaseValue to an int for saving.
            strengthBase = (int)stats.Strength.BaseValue;
            dexterityBase = (int)stats.Dexterity.BaseValue;
            vitalityBase = (int)stats.Vitality.BaseValue;
            intelligenceBase = (int)stats.Intelligence.BaseValue;
            wisdomBase = (int)stats.Wisdom.BaseValue;
            enduranceBase = (int)stats.Endurance.BaseValue;
            senseBase = (int)stats.Sense.BaseValue;
        }
    }

    public void SaveState()
    {
        SaveSystem.SavePlayerStats(this);
        Debug.Log("Player stats saved.");
    }

    public void LoadState()
    {
        SaveData data = SaveSystem.LoadPlayerStats();
        if (data == null) return;

        // Load progression
        level = data.level;
        experience = data.experience;
        unspentStatPoints = data.unspentStatPoints;
        experienceToNextLevel = CalculateExperienceForNextLevel();

        // Load base stat values
        Strength.BaseValue = data.strengthBase;
        Dexterity.BaseValue = data.dexterityBase;
        Vitality.BaseValue = data.vitalityBase;
        Intelligence.BaseValue = data.intelligenceBase;
        Wisdom.BaseValue = data.wisdomBase;
        Endurance.BaseValue = data.enduranceBase;
        Sense.BaseValue = data.senseBase;

        // Recalculate max resources based on loaded stats and fully heal.
        RecalculateMaxResources();
        FullRestore();

        OnStatsUpdated?.Invoke(); // Notify UI
        Debug.Log("Player stats loaded.");
    }

    #endregion

    protected override void Awake()
    {
        base.Awake(); // Calls the Awake from CharacterStatsBase

        // Check for a save file. If none, initialize a new character.
        if (SaveSystem.LoadPlayerStats() != null)
        {
            LoadState();
        }
        else
        {
            InitializeNewCharacter();
        }
    }

    // A test method for saving and loading. You would call these from a UI menu.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SaveState();
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            LoadState();
        }
    }

    private void InitializeNewCharacter()
    {
        Debug.Log("No save file found. Initializing new character stats.");
        RecalculateMaxResources();
        FullRestore();
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
        OnStatsUpdated?.Invoke();
    }

    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        unspentStatPoints += 5;
        experienceToNextLevel = CalculateExperienceForNextLevel();

        RecalculateMaxResources();
        FullRestore();
        OnStatsUpdated?.Invoke();
        Debug.Log($"Player Leveled Up to Level {level}!");
    }

    public void AllocateStatPoint(StatType statToAllocate)
    {
        if (unspentStatPoints <= 0) return;

        unspentStatPoints--;

        switch (statToAllocate)
        {
            case StatType.Strength: Strength.BaseValue++; break;
            case StatType.Dexterity: Dexterity.BaseValue++; break;
            case StatType.Vitality: Vitality.BaseValue++; break;
            case StatType.Intelligence: Intelligence.BaseValue++; break;
            case StatType.Wisdom: Wisdom.BaseValue++; break;
            case StatType.Endurance: Endurance.BaseValue++; break;
            case StatType.Sense: Sense.BaseValue++; break;
        }

        RecalculateMaxResources();

        if (statToAllocate == StatType.Sense)
        {
            NotifyCoreStatChanged();
        }

        OnStatsUpdated?.Invoke();
    }

    private void RecalculateMaxResources()
    {
        maxHealth = 25 + ((int)Vitality.Value * 5);
        maxMana = 25 + ((int)Wisdom.Value * 5);
        maxEnergy = 25 + ((int)Endurance.Value * 5);

        FullRestore();
    }

    private void FullRestore()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEnergy = maxEnergy;
    }

    private int CalculateExperienceForNextLevel()
    {
        return level * 100;
    }

    /// <summary>
    /// This is a public "notifier" method that the BuffManager can call
    /// to safely trigger the OnCoreStatChanged event.
    /// </summary>
    public void NotifyCoreStatChanged()
    {
        OnCoreStatChanged?.Invoke();
        OnStatsUpdated?.Invoke();
    }
}