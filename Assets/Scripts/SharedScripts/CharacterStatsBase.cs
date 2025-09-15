using UnityEngine;

// Enum to define the different types of stats for easy reference.
public enum StatType { Strength, Dexterity, Vitality, Intelligence, Wisdom, Endurance, Sense }

// This is the base component for all characters, containing shared stats and resources.
public class CharacterStatsBase : MonoBehaviour
{
    [Header("Primary Stats")]
    public Stat Strength = new Stat(5);
    public Stat Dexterity = new Stat(5);
    public Stat Vitality = new Stat(5);
    public Stat Intelligence = new Stat(5);
    public Stat Wisdom = new Stat(5);
    public Stat Endurance = new Stat(5);
    public Stat Sense = new Stat(5);

    [Header("Secondary Stats")]
    // Base value of 1 represents 100% speed. Buffs/debuffs will modify this.
    public Stat MovementSpeed = new Stat(1);
    public Stat AttackSpeed = new Stat(1);

    [Header("Resources")]
    public float maxHealth = 100;
    public float currentHealth { get; protected set; }

    public float maxMana = 100;
    public float currentMana { get; protected set; }

    public float maxEnergy = 100;
    public float currentEnergy { get; protected set; }

    // Special resource for the Pain-Eater
    public float maxAnguish = 100;
    public float currentAnguish { get; protected set; }

    protected virtual void Awake()
    {
        // Initialize all resources to their maximum values at the start.
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEnergy = maxEnergy;
        currentAnguish = 0; // Anguish starts at zero
    }

    // --- Public methods for managing resources ---

    public virtual void TakeDamage(float damage)
    {
        // Add defense calculation here later if needed
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void SpendMana(float amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
    }

    public void SpendEnergy(float amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    public void SpendHealth(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void SpendAnguish(float amount)
    {
        currentAnguish -= amount;
        currentAnguish = Mathf.Clamp(currentAnguish, 0, maxAnguish);
    }

    public void GainAnguish(float amount)
    {
        currentAnguish += amount;
        currentAnguish = Mathf.Clamp(currentAnguish, 0, maxAnguish);
    }

    protected virtual void Die()
    {
        Debug.Log(transform.name + " has died.");
        // Add death logic here (e.g., play animation, destroy object, etc.)
    }
}