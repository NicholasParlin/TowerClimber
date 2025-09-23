using System;
using UnityEngine;

[RequireComponent(typeof(InventoryManager), typeof(CharacterController))]
public class PlayerStats : CharacterStatsBase
{
    public static PlayerStats Instance { get; private set; }

    [Header("Player Progression")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;
    public int unspentStatPoints = 0;

    public event Action OnStatsUpdated;

    #region Save System Integration
    [System.Serializable]
    public class SaveData
    {
        public int currentLevel;
        public int currentExperience;
        public int unspentStatPoints;
        public int strengthBase, dexterityBase, vitalityBase, intelligenceBase, wisdomBase, enduranceBase, senseBase;
        public float currentHealth;
        public float currentMana;
        public float currentEnergy;
        public float[] position;
        public float[] rotation;

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
            currentHealth = playerStats.currentHealth;
            currentMana = playerStats.currentMana;
            currentEnergy = playerStats.currentEnergy;
            position = new float[3];
            position[0] = playerStats.transform.position.x;
            position[1] = playerStats.transform.position.y;
            position[2] = playerStats.transform.position.z;
            rotation = new float[4];
            rotation[0] = playerStats.transform.rotation.x;
            rotation[1] = playerStats.transform.rotation.y;
            rotation[2] = playerStats.transform.rotation.z;
            rotation[3] = playerStats.transform.rotation.w;
        }
    }

    public void SaveState() { SaveSystem.SavePlayerStats(this); }
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

            RecalculateAllDerivedStats();

            currentHealth = data.currentHealth;
            currentMana = data.currentMana;
            currentEnergy = data.currentEnergy;

            if (currentHealth > maxHealth) currentHealth = maxHealth;
            if (currentMana > maxMana) currentMana = maxMana;
            if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;

            Vector3 loadedPosition = new Vector3(data.position[0], data.position[1], data.position[2]);
            Quaternion loadedRotation = new Quaternion(data.rotation[0], data.rotation[1], data.rotation[2], data.rotation[3]);

            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = loadedPosition;
                transform.rotation = loadedRotation;
                controller.enabled = true;
            }
            else
            {
                transform.position = loadedPosition;
                transform.rotation = loadedRotation;
            }

            OnStatsUpdated?.Invoke();
            Debug.Log("Player stats and location loaded.");
        }
        else
        {
            RecalculateAllDerivedStats();
            RestoreAllResources();
            Debug.Log("No save data found. Initializing new character stats.");
        }
    }
    #endregion

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        base.Awake();
        SubscribeToPlayerStatChanges();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnsubscribeFromPlayerStatChanges();
    }

    private void SubscribeToPlayerStatChanges()
    {
        Wisdom.OnValueChanged += RecalculatePlayerDerivedStats;
        Endurance.OnValueChanged += RecalculatePlayerDerivedStats;
    }

    private void UnsubscribeFromPlayerStatChanges()
    {
        Wisdom.OnValueChanged -= RecalculatePlayerDerivedStats;
        Endurance.OnValueChanged -= RecalculatePlayerDerivedStats;
    }

    private void RecalculatePlayerDerivedStats()
    {
        HealthRegen.BaseValue = CalculateRegenValue(Endurance.Value, maxHealth);
        EnergyRegen.BaseValue = CalculateRegenValue(Endurance.Value, maxEnergy);
        ManaRegen.BaseValue = CalculateRegenValue(Wisdom.Value, maxMana);
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
        OnStatsUpdated?.Invoke();
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
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
        RestoreAllResources();
    }

    public void RestoreHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnStatsUpdated?.Invoke();
    }

    public void RestoreMana(float amount)
    {
        currentMana += amount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        OnStatsUpdated?.Invoke();
    }

    public void RestoreEnergy(float amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        OnStatsUpdated?.Invoke();
    }

    private float CalculateRegenValue(float statValue, float maxResource)
    {
        float baseRegenPerSecond = 5f / 60f;
        float midRegenPerSecond = 8.5f;

        if (statValue <= 5) return baseRegenPerSecond;
        if (statValue <= 1250)
        {
            float t = Mathf.InverseLerp(5, 1250, statValue);
            return Mathf.Lerp(baseRegenPerSecond, midRegenPerSecond, t);
        }
        if (statValue < 1875)
        {
            float t = Mathf.InverseLerp(1250, 1875, statValue);
            return Mathf.Lerp(midRegenPerSecond, maxResource, t);
        }
        return maxResource;
    }

    public void NotifyStatsUpdated()
    {
        OnStatsUpdated?.Invoke();
    }
}