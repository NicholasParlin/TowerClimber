using System.Collections.Generic;
using UnityEngine;

// An enum to define the different slots an item can be equipped to.
public enum EquipmentSlot
{
    Head,
    Torso,
    Pants,
    Hands,
    Feet,
    MainHand,
    OffHand,
    Amulet,
    Ring
}

// This is a specialized type of Item for things that can be equipped.
[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment Item")]
public class EquipmentItem : Item
{
    [Header("Equipment Settings")]
    public EquipmentSlot equipmentSlot;
    public List<StatBonus> statBonuses = new List<StatBonus>();

    /// <summary>
    /// Overrides the base Use method. For equipment, "using" it means equipping it.
    /// </summary>
    public override void Use()
    {
        base.Use();
        // Tell the EquipmentManager to equip this item.
        EquipmentManager.Instance.Equip(this);
    }
}