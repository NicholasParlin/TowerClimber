using UnityEngine;

// This component is attached to an enemy and manages its loot drops upon death.
public class EnemyLootDrop : MonoBehaviour
{
    [Header("Loot Configuration")]
    [SerializeField] private LootTable lootTable;

    //reference to a 3D model prefab for a loot bag/item here.
    [SerializeField] private GameObject lootDropPrefab;

    /// <summary>
    /// This method is called by the EnemyHealth script when the enemy dies.
    /// It calculates and spawns the loot.
    /// </summary>
    public void DropLoot()
    {
        if (lootTable == null)
        {
            Debug.LogWarning($"No loot table assigned to {gameObject.name}.");
            return;
        }

        // Get the list of items that successfully dropped.
        var droppedItems = lootTable.GetDroppedItems();

        foreach (var item in droppedItems)
        {
            Debug.Log($"{gameObject.name} dropped: {item.itemName}");
            // Here, you would instantiate a physical representation of the loot in the world.
            // For example: Instantiate(lootDropPrefab, transform.position, Quaternion.identity);
            // The loot drop prefab would then need a script to handle being picked up by the player.
        }
    }
}