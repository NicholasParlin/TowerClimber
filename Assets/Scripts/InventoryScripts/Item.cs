using UnityEngine;

// An enum to define the different types of items for easy sorting.
public enum ItemType
{
    Consumable,
    Equipment,
    QuestItem,
    Default
}

// This is the abstract base class for all items. We will create specific item types
// that inherit from this.
public abstract class Item : ScriptableObject
{
    [Header("Item Information")]
    public string itemName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public ItemType itemType;
    public bool isStackable = true;
    public int maxStackSize = 99;

    // This method defines what happens when the item is used from the inventory.
    // It is 'virtual' so that specific item types can override it with their own logic.
    public virtual void Use()
    {
        // Default behavior is to do nothing.
        Debug.Log($"Using {itemName}.");
    }
}