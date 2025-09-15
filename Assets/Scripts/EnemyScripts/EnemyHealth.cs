using UnityEngine;

// This component manages the health and death state of an enemy.
[RequireComponent(typeof(CharacterStatsBase))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Identification")]
    [Tooltip("A unique string ID for this enemy type (e.g., 'WildBoar', 'GoblinScrapper'). This MUST match the ID used in Kill Objectives.")]
    [SerializeField] private string enemyID;

    // Reference to the Loot Drop component.
    private EnemyLootDrop _lootDrop;
    private CharacterStatsBase _stats;
    private bool _isDead = false;

    private void Awake()
    {
        _stats = GetComponent<CharacterStatsBase>();
        _lootDrop = GetComponent<EnemyLootDrop>(); // Get the loot component.
    }

    /// <summary>
    /// Public method to apply damage to this enemy. This would be called by player attack scripts.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        // Pass the damage to the stats component to handle the health reduction.
        _stats.TakeDamage(amount);
        // In a full game, you might want to show a floating damage number here.

        // Check if the enemy has died.
        if (_stats.currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles the death of the enemy.
    /// </summary>
    private void Die()
    {
        _isDead = true;
        Debug.Log($"{gameObject.name} has died.");

        // Fire the global event to notify the QuestLog that this enemy has been killed.
        GameEvents.ReportEnemyKilled(enemyID);

        // If a loot drop component exists on this enemy, tell it to drop loot.
        if (_lootDrop != null)
        {
            _lootDrop.DropLoot();
        }

        // Here you would trigger a death animation, and then destroy the object after a delay.
        // For example:
        // GetComponent<Animator>().SetTrigger("Die");
        // Destroy(gameObject, 3f); 
    }
}