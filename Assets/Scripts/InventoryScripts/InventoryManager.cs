using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// A helper class to store an item and its quantity in an inventory slot.
// This is not a MonoBehaviour, just a data container.
[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;

    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }
}

// This component manages the player's inventory, including currency and items.
public class InventoryManager : MonoBehaviour
{
    // Singleton pattern to make it easily accessible from other scripts (e.g., QuestLog).
    public static InventoryManager Instance { get; private set; }

    public int currentGold { get; private set; }
    public List<InventorySlot> inventory = new List<InventorySlot>();

    #region Save System Integration
    // This is the data structure that will be saved to a file.
    [System.Serializable]
    public class SaveData
    {
        public int currentGold;
        public List<string> itemNames;
        public List<int> itemQuantities;

        public SaveData(InventoryManager inventory)
        {
            currentGold = inventory.currentGold;
            // Use LINQ to efficiently get lists of the item names and quantities for saving.
            itemNames = inventory.inventory.Select(slot => slot.item.name).ToList();
            itemQuantities = inventory.inventory.Select(slot => slot.quantity).ToList();
        }
    }

    /// <summary>
    /// Tells the static SaveSystem to save the current state of this inventory.
    /// </summary>
    public void SaveState()
    {
        SaveSystem.SavePlayerInventory(this);
    }

    /// <summary>
    /// Loads the inventory state from a save file.
    /// </summary>
    public void LoadState(ItemDatabase itemDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerInventory();
        if (data != null)
        {
            currentGold = data.currentGold;
            inventory.Clear();
            // Reconstruct the inventory list from the saved names and quantities.
            for (int i = 0; i < data.itemNames.Count; i++)
            {
                Item item = itemDatabase.GetItemByName(data.itemNames[i]);
                if (item != null)
                {
                    inventory.Add(new InventorySlot(item, data.itemQuantities[i]));
                }
            }
            Debug.Log("Player inventory loaded.");
        }
        else
        {
            // Set starting gold for a new game if no save file is found.
            currentGold = 50;
            Debug.Log("No inventory save data found, starting with default gold.");
        }
    }
    #endregion

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    /// <summary>
    /// Adds a specified amount of gold to the player's total.
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount > 0) currentGold += amount;
        // TODO: Fire a UI update event here.
    }

    /// <summary>
    /// Attempts to spend a specified amount of gold. Returns true if successful.
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount > 0 && currentGold >= amount)
        {
            currentGold -= amount;
            // TODO: Fire a UI update event here.
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds a specified quantity of an item to the inventory.
    /// </summary>
    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return;

        // If the item is stackable, try to add to an existing, non-full stack first.
        if (item.isStackable)
        {
            InventorySlot existingSlot = inventory.FirstOrDefault(slot => slot.item == item && slot.quantity < item.maxStackSize);
            if (existingSlot != null)
            {
                existingSlot.AddQuantity(quantity);
                Debug.Log($"Added {quantity} {item.itemName} to existing stack. New total: {existingSlot.quantity}.");
                // TODO: Fire a UI update event here.
                return;
            }
        }

        // If no existing stack was found (or the item is not stackable), add it to a new slot.
        inventory.Add(new InventorySlot(item, quantity));
        Debug.Log($"Added {quantity} {item.itemName} to a new inventory slot.");
        // TODO: Fire a UI update event here.
    }
}