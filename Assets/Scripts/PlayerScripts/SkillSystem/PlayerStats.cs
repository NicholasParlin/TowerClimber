using System;
using UnityEngine;

// This component is specific to the player and handles their stats, resources, and progression.
[RequireComponent(typeof(InventoryManager))]
public class PlayerStats : CharacterStatsBase
{
    [Header("Player Progression")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;
    public int unspentStatPoints = 0;

    // Event that broadcasts when a stat relevant to the GameManager (like Sense) changes.
    public event Action OnCoreStatChanged;

    // Event that broadcasts when any stat, level, or experience changes, for UI updates.
    public event Action OnStatsUpdated;

    #region Save System Integration
    [System.Serializable]
    public class SaveData
    {
        public int currentLevel;
        public int currentExperience;
        public int unspentStatPoints;
        public int strengthBase, dexterityBase, vitalityBase, intelligenceBase, wisdomBase, enduranceBase, senseBase;

        public SaveData(PlayerStats playerStats)
        {
            currentLevel = playerStats.currentLevel;
            currentExperience = playerStats.currentExperience;
            unspentStatPoints = playerStats.unspentStatPoints;

            strengthBase = (int)playerStats.Strength.BaseValue;
            dexterityBase = (int)playerStats.Dexterity.BaseValue;
            vitalityBase = (int)playerStats.Vitality.BaseValue;
            intelligenceBase = (int)playerStats.Intelligence.BaseValue;
            wisdomBase = (int)playerStats.Wisdom.BaseValue;
            enduranceBase = (int)playerStats.Endurance.BaseValue;
            senseBase = (int)playerStats.Sense.BaseValue;
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
        if (data != null)
        {
            currentLevel = data.currentLevel;
            currentExperience = data.currentExperience;
            unspentStatPoints = data.unspentStatPoints;

            Strength.BaseValue = data.strengthBase;
            Dexterity.BaseValue = data.dexterityBase;
            Vitality.BaseValue = data.vitalityBase;
            Intelligence.BaseValue = data.intelligenceBase;
            Wisdom.BaseValue = data.wisdomBase;
            Endurance.BaseValue = data.enduranceBase;
            Sense.BaseValue = data.senseBase;

            // Recalculate max resources and restore to full after loading.
            CalculateMaxResources();
            RestoreAllResources();
            Debug.Log("Player stats loaded.");
            OnStatsUpdated?.Invoke();
        }
    }
    #endregion

    /// <summary>
    /// Adds experience to the player and checks for level up.
    /// </summary>
    public void AddExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log($"Gained {amount} XP. Total: {currentExperience}/{experienceToNextLevel}");
        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
        OnStatsUpdated?.Invoke();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);
        unspentStatPoints += 5;

        // Fully restore the player on level up.
        RestoreAllResources();
        Debug.Log($"LEVEL UP! Reached level {currentLevel}. You have {unspentStatPoints} stat points.");
    }

    /// <summary>
    /// Spends a stat point to permanently increase a base stat.
    /// </summary>
    public void AllocateStatPoint(StatType statToAllocate)
    {
        if (unspentStatPoints <= 0) return;

        unspentStatPoints--;

        switch (statToAllocate)
        {
            case StatType.Strength: Strength.BaseValue++; break;
            case StatType.Dexterity: Dexterity.BaseValue++; break;
            case StatType.Vitality:
                float oldMaxHealth = maxHealth;
                Vitality.BaseValue++;
                CalculateMaxResources();
                currentHealth += (maxHealth - oldMaxHealth);
                break;
            case StatType.Intelligence: Intelligence.BaseValue++; break;
            case StatType.Wisdom:
                float oldMaxMana = maxMana;
                Wisdom.BaseValue++;
                CalculateMaxResources();
                currentMana += (maxMana - oldMaxMana);
                break;
            case StatType.Endurance:
                float oldMaxEnergy = maxEnergy;
                Endurance.BaseValue++;
                CalculateMaxResources();
                currentEnergy += (maxEnergy - oldMaxEnergy);
                break;
            case StatType.Sense:
                Sense.BaseValue++;
                NotifyCoreStatChanged();
                break;
        }
        OnStatsUpdated?.Invoke();
    }

    /// <summary>
    /// A public method for external systems (like the BuffManager) to safely fire the core stat change event.
    /// </summary>
    public void NotifyCoreStatChanged()
    {
        OnCoreStatChanged?.Invoke();
    }

    /// <summary>
    /// Gets a specific stat object based on the StatType enum.
    /// </summary>
    public Stat GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.Strength: return Strength;
            case StatType.Dexterity: return Dexterity;
            case StatType.Vitality: return Vitality;
            case StatType.Intelligence: return Intelligence;
            case StatType.Wisdom: return Wisdom;
            case StatType.Endurance: return Endurance;
            case StatType.Sense: return Sense;
            default: return null;
        }
    }
}