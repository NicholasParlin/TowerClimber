using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// The InventorySlot class is now a pure data container.
public class InventorySlot
{
    public Item item;
    public int quantity;

    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void AddQuantity(int amount) { quantity += amount; }
    public void RemoveQuantity(int amount) { quantity -= amount; }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
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
    public void SaveState() { SaveSystem.SavePlayerInventory(this); }
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
                if (item != null) { inventory.Add(new InventorySlot(item, data.itemQuantities[i])); }
            }
        }
        else { currentGold = 50; }
        OnInventoryChanged?.Invoke();
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

    public void RemoveItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return;

        InventorySlot slotToRemoveFrom = inventory.FirstOrDefault(slot => slot.item == item);
        if (slotToRemoveFrom != null)
        {
            slotToRemoveFrom.RemoveQuantity(quantity);
            if (slotToRemoveFrom.quantity <= 0)
            {
                inventory.Remove(slotToRemoveFrom);
            }
            OnInventoryChanged?.Invoke();
        }
    }

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