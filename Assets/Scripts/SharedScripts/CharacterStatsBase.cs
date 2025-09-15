using UnityEngine;

// This is the universal base class for all characters, both player and enemies.
// It holds all the core stats and resources.
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

    [Header("Combat Stats")]
    public Stat Armor;
    public Stat MagicResistance;
    public Stat AttackSpeed;
    public Stat MovementSpeed;

    [Header("Primary Resources")]
    public float maxHealth = 100;
    // The 'set' accessor is 'protected' to allow child classes like PlayerStats to modify it.
    public float currentHealth { get; protected set; }
    public float maxMana = 50;
    public float currentMana { get; protected set; }
    public float maxEnergy = 50;
    public float currentEnergy { get; protected set; }

    [Header("Special Resources")]
    public float maxAnguish = 100;
    public float currentAnguish { get; protected set; }

    protected virtual void Awake()
    {
        // Set resources to full on awake. The Save/Load system will override this if a save exists.
        RestoreAllResources();
    }

    /// <summary>
    /// Sets all current resources to their maximum values.
    /// </summary>
    public void RestoreAllResources()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEnergy = maxEnergy;
    }

    /// <summary>
    /// Recalculates the maximum value of primary resources based on core stats.
    /// </summary>
    public void CalculateMaxResources()
    {
        maxHealth = 25 + (Vitality.Value * 5);   // Example formula
        maxMana = 25 + (Wisdom.Value * 5);     // Example formula
        maxEnergy = 25 + (Endurance.Value * 5); // Example formula
    }

    // --- Resource Management ---

    /// <summary>
    /// Reduces current health by a specified amount.
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    /// <summary>
    /// Reduces current mana by a specified amount.
    /// </summary>
    public void SpendMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
    }

    /// <summary>
    /// Reduces current energy by a specified amount.
    /// </summary>
    public void SpendEnergy(float amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0) currentEnergy = 0;
    }

    /// <summary>
    /// Increases current anguish by a specified amount, capped at the maximum.
    /// </summary>
    public void GainAnguish(float amount)
    {
        currentAnguish += amount;
        if (currentAnguish > maxAnguish) currentAnguish = maxAnguish;
    }

    /// <summary>
    /// Reduces current anguish by a specified amount.
    /// </summary>
    public void SpendAnguish(float amount)
    {
        currentAnguish -= amount;
        if (currentAnguish < 0) currentAnguish = 0;
    }
}