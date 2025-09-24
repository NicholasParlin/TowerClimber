using System.Collections.Generic;
using UnityEngine;

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

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment Item")]
public class EquipmentItem : Item
{
    [Header("Equipment Settings")]
    public EquipmentSlot equipmentSlot;
    public List<StatBonus> statBonuses = new List<StatBonus>();

    /// <summary>
    /// Overrides the base Use method. For equipment, "using" it means equipping it,
    /// and then triggering any on-equip gameplay effects.
    /// </summary>
    public override void Use()
    {
        // First, tell the EquipmentManager to equip this item.
        EquipmentManager.Instance.Equip(this);

        // Then, call the base Use() method, which will execute the list of effects.
        base.Use();
    }
}