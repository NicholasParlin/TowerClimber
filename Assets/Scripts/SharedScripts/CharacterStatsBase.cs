using UnityEngine;
using System.Collections.Generic;

public class CharacterStatsBase : MonoBehaviour
{
    [Header("Primary Stats")]
    public Stat Strength;
    public Stat Dexterity;
    public Stat Vitality;
    public Stat Intelligence;
    public Stat Wisdom;
    public Stat Endurance;
    public Stat Sense;

    [Header("Defensive Stats")]
    public Stat Armor;
    public Stat MagicResistance;
    [Tooltip("Resistance to being interrupted. Checked against incoming stagger power.")]
    public Stat Poise;
    [Tooltip("If an incoming stagger attack's power is at or above this value, the character is knocked down.")]
    public Stat StaggerDamageThreshold;

    [Header("Specific Resistances")]
    public Stat FireResistance;
    public Stat WaterResistance;
    public Stat NatureResistance;
    public Stat HexResistance;
    public Stat NecroResistance;
    public Stat WindResistance;
    public Stat LightningResistance;

    [Header("Regeneration Stats")]
    public Stat HealthRegen;
    public Stat ManaRegen;
    public Stat EnergyRegen;
    public Stat StaggerRegen;

    [Header("Combat Stats")]
    public Stat AttackSpeed;
    public Stat MovementSpeed;
    [Tooltip("The multiplier for critical hit damage. Base value is 1.5 for 150% damage.")]
    public Stat CriticalDamage;

    private List<Stat> _allStats;

    [Header("Primary Resources")]
    public float maxHealth = 100;
    public float currentHealth { get; protected set; }
    public float maxMana = 50;
    public float currentMana { get; protected set; }
    public float maxEnergy = 50;
    public float currentEnergy { get; protected set; }

    [Header("Special Resources")]
    public float maxAnguish = 100;
    public float currentAnguish { get; protected set; }
    [Tooltip("The base stagger value before stat modifications.")]
    public float baseMaxStagger = 100f;
    public float maxStagger { get; protected set; }
    public float currentStagger { get; protected set; }

    // MODIFIED: This is now a private reference, as other scripts should go through the state machine.
    private CharacterStateManager _stateManager;

    // NEW: We need a reference to the factory to create states.
    private PlayerStateFactory _stateFactory;

    protected virtual void Awake()
    {
        _stateManager = GetComponent<CharacterStateManager>();
        // NEW: Initialize the state factory
        _stateFactory = new PlayerStateFactory(_stateManager);

        InitializeStatList();
        SubscribeToStatChanges();
        RecalculateAllDerivedStats();
        RestoreAllResources();
    }

    protected virtual void OnDestroy()
    {
        UnsubscribeFromStatChanges();
    }

    protected virtual void Update()
    {
        HandleRegeneration();
    }

    private void SubscribeToStatChanges()
    {
        Vitality.OnValueChanged += RecalculateArmor;
        Vitality.OnValueChanged += RecalculateMaxHealth;
        Vitality.OnValueChanged += RecalculateMaxStagger;
        Endurance.OnValueChanged += RecalculateMaxStagger;
        Endurance.OnValueChanged += RecalculateMaxEnergy;
        Endurance.OnValueChanged += RecalculateStaggerThreshold;
        Wisdom.OnValueChanged += RecalculateMaxMana;
    }

    private void UnsubscribeFromStatChanges()
    {
        Vitality.OnValueChanged -= RecalculateArmor;
        Vitality.OnValueChanged -= RecalculateMaxHealth;
        Vitality.OnValueChanged -= RecalculateMaxStagger;
        Endurance.OnValueChanged -= RecalculateMaxStagger;
        Endurance.OnValueChanged -= RecalculateMaxEnergy;
        Endurance.OnValueChanged -= RecalculateStaggerThreshold;
        Wisdom.OnValueChanged -= RecalculateMaxMana;
    }

    private void RecalculateArmor() { Armor.BaseValue = Vitality.Value * 0.10f; }
    private void RecalculateStaggerThreshold() { StaggerDamageThreshold.BaseValue = Endurance.Value * 1.0f; }
    private void RecalculateMaxStagger()
    {
        float vitalityBonus = Vitality.Value * 0.05f;
        float enduranceBonus = Endurance.Value * 0.10f;
        maxStagger = baseMaxStagger * (1 + vitalityBonus + enduranceBonus);
    }

    private void RecalculateMaxHealth()
    {
        float oldMaxHealth = maxHealth;
        maxHealth = Vitality.Value * 5;
        currentHealth += maxHealth - oldMaxHealth;
    }

    private void RecalculateMaxMana()
    {
        float oldMaxMana = maxMana;
        maxMana = Wisdom.Value * 5;
        currentMana += maxMana - oldMaxMana;
    }

    private void RecalculateMaxEnergy()
    {
        float oldMaxEnergy = maxEnergy;
        maxEnergy = Endurance.Value * 5;
        currentEnergy += maxEnergy - oldMaxEnergy;
    }

    public void RecalculateAllDerivedStats()
    {
        RecalculateArmor();
        RecalculateStaggerThreshold();
        RecalculateMaxStagger();
        RecalculateMaxHealth();
        RecalculateMaxMana();
        RecalculateMaxEnergy();
    }

    private void InitializeStatList()
    {
        _allStats = new List<Stat>
        {
            Strength, Dexterity, Vitality, Intelligence, Wisdom, Endurance, Sense,
            Armor, MagicResistance, Poise, StaggerDamageThreshold,
            FireResistance, WaterResistance, NatureResistance, HexResistance, NecroResistance, WindResistance, LightningResistance,
            HealthRegen, ManaRegen, EnergyRegen, StaggerRegen,
            AttackSpeed, MovementSpeed, CriticalDamage
        };
    }

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

    public void RemoveAllModifiersFromSource(object source)
    {
        foreach (var stat in _allStats)
        {
            stat.RemoveAllModifiersFromSource(source);
        }
    }

    private void HandleRegeneration()
    {
        if (HealthRegen.Value > 0 && currentHealth < maxHealth)
        {
            currentHealth += HealthRegen.Value * Time.deltaTime;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
        }
        if (ManaRegen.Value > 0 && currentMana < maxMana)
        {
            currentMana += ManaRegen.Value * Time.deltaTime;
            if (currentMana > maxMana) currentMana = maxMana;
        }
        if (EnergyRegen.Value > 0 && currentEnergy < maxEnergy)
        {
            currentEnergy += EnergyRegen.Value * Time.deltaTime;
            if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
        }
        if (StaggerRegen.Value > 0 && currentStagger < maxStagger)
        {
            currentStagger += StaggerRegen.Value * Time.deltaTime;
            if (currentStagger > maxStagger) currentStagger = maxStagger;
        }
    }

    public void RestoreAllResources()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEnergy = maxEnergy;
        currentStagger = maxStagger;
    }

    public void RestoreStagger()
    {
        currentStagger = maxStagger;
    }

    public void CalculateMaxResources()
    {
        maxHealth = (Vitality.Value * 5);
        maxMana = (Wisdom.Value * 5);
        maxEnergy = (Endurance.Value * 5);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    public void TakeStaggerDamage(float staggerPower)
    {
        if (staggerPower <= 0) return;

        // Check if the current state can be interrupted by stagger
        bool canBeStaggered = _stateManager.CurrentState is PlayerIdleState || _stateManager.CurrentState is PlayerMovingState;
        if (!canBeStaggered) return;

        currentStagger -= staggerPower;
        Debug.Log($"{gameObject.name} took {staggerPower} stagger damage. Current stagger: {currentStagger}");

        if (currentStagger <= 0)
        {
            if (_stateManager != null)
            {
                // MODIFIED: Use the new SwitchState method with the factory
                if (staggerPower >= StaggerDamageThreshold.Value)
                {
                    _stateManager.SwitchState(_stateFactory.KnockedDown());
                }
                else
                {
                    _stateManager.SwitchState(_stateFactory.Staggered());
                }
            }
            RestoreStagger();
        }
    }

    public void SpendMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
    }

    public void SpendEnergy(float amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0) currentEnergy = 0;
    }

    public void GainAnguish(float amount)
    {
        currentAnguish += amount;
        if (currentAnguish > maxAnguish) currentAnguish = maxAnguish;
    }

    public void SpendAnguish(float amount)
    {
        currentAnguish -= amount;
        if (currentAnguish < 0) currentAnguish = 0;
    }
}