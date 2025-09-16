using UnityEngine;
using System.Collections.Generic;

// This component is attached to an enemy and manages its loot drops upon death using an Object Pooler.
public class EnemyLootDrop : MonoBehaviour
{
    [Header("Loot Configuration")]
    [Tooltip("Assign the LootTable ScriptableObject that defines the potential drops for this enemy.")]
    [SerializeField] private LootTable lootTable;
    [Tooltip("The pool tag for the physical loot drop prefab (e.g., a loot bag). This must match a tag in your ObjectPooler.")]
    [SerializeField] private string lootDropPoolTag = "LootDrop";
    [Tooltip("How far the loot can scatter from the enemy's death position.")]
    [SerializeField] private float dropRadius = 1.5f;


    /// <summary>
    /// This method is called by the EnemyHealth script when the enemy dies.
    /// It calculates the loot and spawns the physical pickups from the object pool.
    /// </summary>
    public void DropLoot()
    {
        if (lootTable == null)
        {
            Debug.LogWarning($"No loot table assigned to {gameObject.name}.");
            return;
        }

        // Get the list of items that have successfully dropped based on their chances.
        List<Item> droppedItems = lootTable.GetDroppedItems();

        foreach (var item in droppedItems)
        {
            // Calculate a random spawn position within the drop radius.
            Vector3 randomOffset = Random.insideUnitSphere * dropRadius;
            randomOffset.y = 0; // Keep the drop on the same horizontal plane.
            Vector3 spawnPosition = transform.position + randomOffset;

            // Get a loot pickup object from the pool instead of instantiating a new one.
            GameObject lootGO = ObjectPooler.Instance.GetFromPool(lootDropPoolTag, spawnPosition, Quaternion.identity);

            if (lootGO != null)
            {
                // Get the LootPickup component and initialize it with the specific item data.
                LootPickup pickup = lootGO.GetComponent<LootPickup>();
                if (pickup != null)
                {
                    pickup.Initialize(item);
                }
            }
        }
    }
}