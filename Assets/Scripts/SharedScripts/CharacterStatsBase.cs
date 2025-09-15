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
    public Stat AttackSpeed;
    public Stat MovementSpeed;

    [Header("Primary Resources")]
    public float maxHealth = 100;
    public float currentHealth { get; protected set; }
    public float maxMana = 50;
    public float currentMana { get; protected set; }
    public float maxEnergy = 50;
    public float currentEnergy { get; protected set; }

    [Header("Special Resources")]
    public float maxAnguish = 100;
    public float currentAnguish { get; private set; }

    protected virtual void Awake()
    {
        // Initialize stats. For now, we assume they will be configured in the Inspector.
        // On game start, ensure resources are calculated based on stats.
        CalculateMaxResources();
        RestoreAllResources();
    }

    /// <summary>
    /// Restores all primary resources to their maximum values.
    /// </summary>
    public void RestoreAllResources()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEnergy = maxEnergy;
    }

    /// <summary>
    /// Recalculates the maximum value of resources based on the character's stats.
    /// </summary>
    public void CalculateMaxResources()
    {
        maxHealth = 25 + (Vitality.Value * 5);
        maxMana = 25 + (Wisdom.Value * 5);
        maxEnergy = 25 + (Endurance.Value * 5);
    }

    // --- Resource Management ---

    /// <summary>
    /// Reduces the character's current health by a specified amount.
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // In a full implementation, you would check for death here.
        // if (currentHealth <= 0) { Die(); }
    }

    /// <summary>
    /// Reduces the character's current mana.
    /// </summary>
    public void SpendMana(float amount)
    {
        if (amount <= 0) return;
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
    }

    /// <summary>
    /// Reduces the character's current energy.
    /// </summary>
    public void SpendEnergy(float amount)
    {
        if (amount <= 0) return;
        currentEnergy -= amount;
        if (currentEnergy < 0) currentEnergy = 0;
    }

    /// <summary>
    /// Increases the character's current Anguish.
    /// </summary>
    public void GainAnguish(float amount)
    {
        if (amount <= 0) return;
        currentAnguish += amount;
        if (currentAnguish > maxAnguish) currentAnguish = maxAnguish;
    }

    /// <summary>
    /// Reduces the character's current Anguish.
    /// </summary>
    public void SpendAnguish(float amount)
    {
        if (amount <= 0) return;
        currentAnguish -= amount;
        if (currentAnguish < 0) currentAnguish = 0;
    }
}