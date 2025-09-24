using UnityEngine;

// This is a specialized type of Item for things that can be consumed.
// It no longer needs any unique logic, as all functionality is handled by GameplayEffects.
[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable Item")]
public class ConsumableItem : Item
{
    // The base Item's Use() method is now sufficient.
    // We can add consumable-specific properties here in the future if needed.
}