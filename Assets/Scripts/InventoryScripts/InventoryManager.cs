using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// A helper class to store an item and its quantity in an inventory slot.
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
    public static InventoryManager Instance { get; private set; }

    // This event will fire whenever the inventory (items or gold) changes, primarily for UI updates.
    public event Action OnInventoryChanged;

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
        Debug.Log("Player inventory saved.");
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
        OnInventoryChanged?.Invoke(); // Fire event to update UI after loading
    }
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    public void AddGold(int amount)
    {
        if (amount > 0) currentGold += amount;
        OnInventoryChanged?.Invoke();
    }

    public bool SpendGold(int amount)
    {
        if (amount > 0 && currentGold >= amount)
        {
            currentGold -= amount;
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return;

        // Fire the global event for the quest system *before* modifying the inventory.
        GameEvents.ReportItemCollected(item.name);

        if (item.isStackable)
        {
            InventorySlot existingSlot = inventory.FirstOrDefault(slot => slot.item == item && slot.quantity < item.maxStackSize);
            if (existingSlot != null)
            {
                existingSlot.AddQuantity(quantity);
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        inventory.Add(new InventorySlot(item, quantity));
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Gets the total quantity of a specific item in the inventory.
    /// This is the new method the quest system needs.
    /// </summary>
    public int GetItemQuantity(string itemName)
    {
        int totalQuantity = 0;
        foreach (var slot in inventory.Where(slot => slot.item.name == itemName))
        {
            totalQuantity += slot.quantity;
        }
        return totalQuantity;
    }
}