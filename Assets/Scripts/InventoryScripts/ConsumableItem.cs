using UnityEngine;

// This is a specialized type of Item for things that can be consumed.
[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable Item")]
public class ConsumableItem : Item
{
    [Header("Consumable Effects")]
    public float healthToRestore = 0;
    public float manaToRestore = 0;
    public float energyToRestore = 0;
    // Future additions could include a list of StatBonus effects for temporary buffs.

    public override void Use()
    {
        // Use the singleton instance for a fast, direct reference.
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("PlayerStats.Instance not found in the scene!");
            return;
        }

        // Apply the restoration effects.
        if (healthToRestore > 0)
        {
            PlayerStats.Instance.RestoreHealth(healthToRestore);
        }
        if (manaToRestore > 0)
        {
            PlayerStats.Instance.RestoreMana(manaToRestore);
        }
        if (energyToRestore > 0)
        {
            PlayerStats.Instance.RestoreEnergy(energyToRestore);
        }

        Debug.Log($"Used {itemName}, restoring {healthToRestore} HP, {manaToRestore} MP, {energyToRestore} EP.");

        // After using the item, remove it from the inventory.
        InventoryManager.Instance.RemoveItem(this);
    }
}