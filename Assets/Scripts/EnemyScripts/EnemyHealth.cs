using UnityEngine;

// This component manages the health and death state of an enemy.
[RequireComponent(typeof(CharacterStatsBase))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Identification")]
    [Tooltip("A unique string ID for this enemy type (e.g., 'WildBoar', 'GoblinScrapper'). This MUST match the ID used in Kill Objectives.")]
    [SerializeField] private string enemyID;

    // References to other components on this enemy.
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

        // Check if the enemy has died.
        if (_stats.currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles the death of the enemy, firing events and returning it to the object pool.
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

        // Return this GameObject to the object pool instead of destroying it.
        // The enemyID should match the pool tag for this to work seamlessly.
        ObjectPooler.Instance.ReturnToPool(enemyID, this.gameObject);
    }

    /// <summary>
    /// Resets the health state of this enemy. Called by the EnemyController when it's reused from the pool.
    /// </summary>
    public void ResetHealth()
    {
        _isDead = false;
        // The stats component will handle restoring the health value to full.
    }
}