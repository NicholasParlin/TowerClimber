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
    [System.Serializable]
    public class SaveData
    {
        public int currentGold;
        public List<string> itemNames;
        public List<int> itemQuantities;

        public SaveData(InventoryManager inventory)
        {
            currentGold = inventory.currentGold;
            itemNames = inventory.inventory.Select(slot => slot.item.name).ToList();
            itemQuantities = inventory.inventory.Select(slot => slot.quantity).ToList();
        }
    }

    public void SaveState()
    {
        SaveSystem.SavePlayerInventory(this);
    }

    public void LoadState(ItemDatabase itemDatabase)
    {
        SaveData data = SaveSystem.LoadPlayerInventory();
        if (data != null)
        {
            currentGold = data.currentGold;
            inventory.Clear();
            for (int i = 0; i < data.itemNames.Count; i++)
            {
                Item item = itemDatabase.GetItemByName(data.itemNames[i]);
                if (item != null)
                {
                    inventory.Add(new InventorySlot(item, data.itemQuantities[i]));
                }
            }
        }
        else
        {
            currentGold = 50; // Starting gold for a new game.
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
        if (amount <= 0) return;
        currentGold += amount;
        OnInventoryChanged?.Invoke(); // Fire event
    }

    public bool SpendGold(int amount)
    {
        if (amount > 0 && currentGold >= amount)
        {
            currentGold -= amount;
            OnInventoryChanged?.Invoke(); // Fire event
            return true;
        }
        return false;
    }

    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return;

        // Fire the global event for the quest system *before* modifying the inventory.
        // This is important for "collect" quests that might be completed by this action.
        GameEvents.ReportItemCollected(item.name);

        if (item.isStackable)
        {
            InventorySlot existingSlot = inventory.FirstOrDefault(slot => slot.item == item && slot.quantity < item.maxStackSize);
            if (existingSlot != null)
            {
                existingSlot.AddQuantity(quantity);
                OnInventoryChanged?.Invoke(); // Fire UI update event
                return;
            }
        }

        inventory.Add(new InventorySlot(item, quantity));
        OnInventoryChanged?.Invoke(); // Fire UI update event
    }
}