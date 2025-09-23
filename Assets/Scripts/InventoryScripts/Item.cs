using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipment,
    QuestItem,
    Default
}

// The Item class is now a pure data container and no longer implements IVirtualizationData.
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

    [Header("Item Economy")]
    public int buyPrice = 10;
    public int sellPrice = 5;

    public virtual void Use()
    {
        Debug.Log($"Using {itemName}.");
    }
}