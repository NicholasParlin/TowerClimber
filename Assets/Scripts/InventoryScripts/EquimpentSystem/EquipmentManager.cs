using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuffManager), typeof(PlayerStats), typeof(InventoryManager))]
public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    public event Action OnEquipmentChanged;

    private Dictionary<EquipmentSlot, EquipmentItem> _equippedItems = new Dictionary<EquipmentSlot, EquipmentItem>();

    private BuffManager _buffManager;
    private PlayerStats _playerStats;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }

        _buffManager = GetComponent<BuffManager>();
        _playerStats = GetComponent<PlayerStats>();
        _inventoryManager = GetComponent<InventoryManager>();
    }

    public void Equip(EquipmentItem newItem)
    {
        if (newItem == null) return;

        if (_equippedItems.TryGetValue(newItem.equipmentSlot, out EquipmentItem oldItem))
        {
            Unequip(newItem.equipmentSlot);
        }

        _equippedItems[newItem.equipmentSlot] = newItem;

        foreach (var bonus in newItem.statBonuses)
        {
            Stat targetStat = _playerStats.GetStat(bonus.statToBuff);
            if (targetStat != null)
            {
                // CONSTRUCTOR UPDATED TO NEW 3-ARGUMENT VERSION
                var modifier = new StatModifier(bonus.value, bonus.type, newItem);
                _buffManager.AddModifier(targetStat, modifier);
            }
        }

        _inventoryManager.RemoveItem(newItem);
        OnEquipmentChanged?.Invoke();
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (_equippedItems.TryGetValue(slot, out EquipmentItem itemToUnequip))
        {
            _buffManager.RemoveAllModifiersFromSource(itemToUnequip);
            _equippedItems.Remove(slot);
            _inventoryManager.AddItem(itemToUnequip);
            OnEquipmentChanged?.Invoke();
        }
    }
}