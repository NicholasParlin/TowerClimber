using System.Collections.Generic;
using UnityEngine;

// A helper class to define a single potential item drop and its chance.
[System.Serializable]
public class LootDrop
{
    public Item item; // The item that can drop
    [Range(0f, 1f)] // A slider in the inspector from 0% to 100%
    public float dropChance;
}

// This ScriptableObject is the template for an enemy's entire loot table.
[CreateAssetMenu(fileName = "New Loot Table", menuName = "Inventory/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootDrop> potentialDrops = new List<LootDrop>();

    /// <summary>
    /// Calculates and returns a list of items that have successfully dropped based on their chances.
    /// </summary>
    /// <returns>A list of Items that should be dropped.</returns>
    public List<Item> GetDroppedItems()
    {
        List<Item> droppedItems = new List<Item>();
        foreach (var drop in potentialDrops)
        {
            // Generate a random number between 0 and 1.
            float randomValue = Random.value;
            // If our random number is less than or equal to the drop chance, the item drops.
            if (randomValue <= drop.dropChance)
            {
                droppedItems.Add(drop.item);
            }
        }
        return droppedItems;
    }
}